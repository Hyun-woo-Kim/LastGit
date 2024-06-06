using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
public enum PpPaze
{
    STATE_None,
    STATE_Basic,
    STATE_PazeOne,
    STATE_PazeTwo,
    STATE_PazeThree,
    STATE_PazeFour,
}
public class ProphetPowderAction : MonoBehaviour
{
    // 근거리 사용 무기: 척력
    //원거리 사용 무기: 인력, 용언
    public PpPaze ppState;
    SpriteRenderer PpSpriteRenderer;
    Transform playerObject;
    PlayerControllerRope playerController;
    Grapling grapling;

    private Transform playerTransform;

    [Header("##TelePort")]
    public GameObject TelePortEffect;
    private GameObject TpEffectPrefab;
    public float TelePortDel; // 이펙트 보여줄 딜레이. 이 시간이 끝나면 순간이동
    [Header("##First Move")]
    public bool isMoving;
    public float chargeSpeed = 5f;
    public float moveDistance = 5.0f; // 이동할 거리

    [Header("##Chaged")]
    public bool isRush = false;
    public float chargeMaxDistance = 5f; // 돌진 가능한 최대 거리
    public float chargeDistace = 5f; // 돌진 조건 거리.
    public float isChargeDel = 1f; // 돌진 딜레이
    public float UnChargeTime;
    public float NotChargeTime;
    private Vector2 lastPlayerPos;
    public bool isMove;
    public bool isChargeReady;
    public bool isChaging;

    [Header("##ZaKoomAtk")]
    public GameObject BombEffect;
    private GameObject bombEffectPrefab;
    private IObjectPool<ProPhetWeapon> _Pool;
    public float ZaKoomCreateDel; //몇 초 뒤에 자쿰 돌을 생성할건지 ,이 초가 지나면 자쿰 돌 생성
    public float ZaKoomEffectDel; //몇 초 뒤에 자쿰 돌 소환 이펙트를 보여줄건지, 이 초가 지나면 자쿰 돌 소환 이펙트 생성
    private IObjectPool<PpZaKoom> _ZaKoomPool; // 자쿰 돌을 위한 오브젝트 풀 추가
    public bool isCreateZakoom;
    public GameObject ZaKoomObj;
    private GameObject ZaKoomPrefab;
    public GameObject CreateEffect;
    private GameObject CtEffectPrefab;
    public bool isZaKoomActive = false; // 자쿰 돌 활성화 중인지 확인하는 플래그

    [Header("##GraplingNockBackAtk")]
    public float nockbackForce;
    public bool isGraplingCooldown;
    public float graplingCooldownTimer; // 쿨타임 타이머
    public float graplingCooldownDuration = 5f; // 쿨타임 지속 시간
    public bool isAttackStart;
    public LayerMask playerLayer;
    public float ShootDistance;

    [Header("##DragonWeaponAtk")]
    public Transform bounsing;
    public Vector3 bounsingSize;
    public bool isAttack = false;
    public float meleeAttackForce;
    public GameObject DragonWeapon;
    private GameObject DragonWeaponPrefab;
    public int activeDragonWeapons = 0; // 활성화된 용언 오브젝트 수를 추적하기 위한 변수
    public int maxDragonWeapons = 8; // 최대 용언 오브젝트 수
    public bool isSpawningDragonWeapon = false; // 용언 소환 중인지 확인하는 플래그
    private bool spawnInCircle = true;

    private void Awake()
    {
        _Pool = new ObjectPool<ProPhetWeapon>(CreateWeapon, OnGetWeapon, OnReleaseWeapon, OnDestroyWeapon, maxSize: 6);
        _ZaKoomPool = new ObjectPool<PpZaKoom>(CreateZaKoom, OnGetZaKoom, OnReleaseZaKoom, OnDestroyZaKoom, maxSize: 1); // 자쿰 돌 풀 초기화
    }
    void Start()
    {
        this.ppState = PpPaze.STATE_None;
        playerController = FindAnyObjectByType<PlayerControllerRope>();
        grapling = FindAnyObjectByType<Grapling>();
        playerObject = GameObject.Find("Player").transform;
        PpSpriteRenderer = GetComponent<SpriteRenderer>();

        isAttackStart = false;

        isMoving = false;

        if (ppState == PpPaze.STATE_None)
        {
            StartCoroutine(FadeInSprite());
        }
    }

    IEnumerator FadeInSprite()
    {
        PpDirection();

        isAttackStart = false;

        Color color = PpSpriteRenderer.color;
        color.a = 0;
        PpSpriteRenderer.color = color;
        if (TpEffectPrefab == null)
        {
            TpEffectPrefab = Instantiate(TelePortEffect, transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(TelePortDel);
        Destroy(TpEffectPrefab);

        lastPlayerPos = playerObject.transform.position;
        while (color.a < 1)
        {
            color.a += Time.deltaTime;
            PpSpriteRenderer.color = color;
            yield return null;
        }

        color.a = 1;
        PpSpriteRenderer.color = color;

        this.ppState = PpPaze.STATE_Basic;
        Debug.Log("State changed to STATE_Basic");
        isRush = true;
    }
   

    IEnumerator PpMove()
    {
        if (isMoving)
        {
            yield break;
        }

        isMoving = true;

        float elapsedTime = 0f;
        Vector2 initialPosition = (Vector2)transform.position; // 명시적으로 캐스팅하여 Vector2로 변환

        // Calculate the target position only on the x-axis
        Vector2 direction = new Vector2((lastPlayerPos.x - initialPosition.x), 0).normalized;
        Vector2 targetPosition = initialPosition + direction * moveDistance;

        while (elapsedTime < 1.0f)
        {
            transform.position = Vector2.Lerp(initialPosition, targetPosition, elapsedTime * chargeSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition; // Ensure the final position is exact

        this.ppState = PpPaze.STATE_PazeOne;
        isRush = true;
        isMoving = false;
    }



    void Update()
    {

        GraplingPlayerNockBack(); // 1. 플레이어가 그래플링을 선지자 파우더에게 했을 때만 실행됨
        UpdateGraplingCooldown(); // 2. 플레이어를 밀치고 나서만 실행되며, 쿨타임 메서드임.

        switch (this.ppState)
        {
            case PpPaze.STATE_Basic:
                if(grapling.isAttatch == false)
                {
                    StartCoroutine(PpMove());
                }
               
                break;
            case PpPaze.STATE_PazeOne:
                if (isChargeReady == false && grapling.isAttatch == false)
                {
                    StartCoroutine(ChargedStop());
                }
                else if (isRush == true && isChargeReady == true && grapling.isAttatch == false )
                {
                    StartCoroutine(ChargeToPlayer());
                }
              

                break;
                // 필요하다면 다른 상태 추가
        }
    }


    private PpZaKoom CreateZaKoom()
    {
        PpZaKoom zakoom = Instantiate(ZaKoomObj).GetComponent<PpZaKoom>();
        zakoom.SetManagedPool(_ZaKoomPool);
        return zakoom;
    }

    private void OnGetZaKoom(PpZaKoom zakoom)
    {
        zakoom.gameObject.SetActive(true);
        isZaKoomActive = true;
    }

    private void OnReleaseZaKoom(PpZaKoom zakoom)
    {
        zakoom.gameObject.SetActive(false);
        isZaKoomActive = false;
          isCreateZakoom = false;
    }

    private void OnDestroyZaKoom(PpZaKoom zakoom)
    {
        Destroy(zakoom.gameObject);
    }

    IEnumerator ZaKoomCreate()
    {
        if (isZaKoomActive)
        {
            Debug.Log("자쿰 돌이 이미 활성화 중입니다.");
            yield break;
        }

        isAttackStart = false;
        yield return new WaitForSeconds(ZaKoomEffectDel);
        if (CtEffectPrefab == null)
        {
            Debug.Log("자쿰 돌 소환 이펙트");
            CtEffectPrefab = Instantiate(CreateEffect, transform.GetChild(0).position, Quaternion.identity);
        }

        yield return new WaitForSeconds(ZaKoomCreateDel);
        Destroy(CtEffectPrefab);
        Debug.Log("자쿰 돌 소환 이펙트 삭제");
        if (!isZaKoomActive)
        {
            Debug.Log("자쿰 돌 소환");
            var zakoom = _ZaKoomPool.Get();
            zakoom.transform.position = transform.GetChild(0).position;
            isZaKoomActive = true; // 자쿰 돌이 활성화된 상태로 설정

            StartCoroutine(ZaKoomDestroy(zakoom));
        }
        isCreateZakoom = true;
        yield return null;
    }



    IEnumerator ZaKoomDestroy(PpZaKoom zakoom)
    {
        yield return new WaitForSeconds(1f);

        // 폭탄 이펙트 생성
        if (bombEffectPrefab == null)
        {
            bombEffectPrefab = Instantiate(BombEffect, zakoom.transform.position, Quaternion.identity);
        }

        // 자쿰 오브젝트 비활성화
        _ZaKoomPool.Release(zakoom);
        isZaKoomActive = false;
        // 1초 후 폭탄 이펙트 제거
        yield return new WaitForSeconds(1f);
        Destroy(bombEffectPrefab);
    }


    void GraplingPlayerNockBack() //기본공격3: 적이 그래플링 스킬 사용하면 밀쳐내기 (쿨타임 존재) 
    {
        if (isGraplingCooldown)
        {
            Debug.Log("쿨타임 중입니다.");
            return; // 아무것도 실행안하고 반환
        }
        //밀쳐내기 코드 구현
        Grapling grapling = FindAnyObjectByType<Grapling>();
        if (grapling.isLerping)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, Mathf.Infinity, playerLayer);
            Debug.DrawRay(transform.position, Vector2.right * ShootDistance, Color.blue);
            if (hit.collider != null)
            {
                float playerdistance = Mathf.Abs(hit.point.x - transform.position.x);

                if (playerdistance < ShootDistance)
                {
                    PlayerControllerRope playerController = FindAnyObjectByType<PlayerControllerRope>();
                    if (playerController != null)
                    {
                        StartCoroutine(GraplingKnockback(playerController, nockbackForce));

                    }
                }
            }
        }
        else
        {
            Debug.Log("적이 그래플링 하지 않음");
        }


        //DragonShoot(); //밀쳐낸 적에게 용언 발사.
    }
    IEnumerator GraplingKnockback(PlayerControllerRope playerController, float nockbackForce)
    {
        lastPlayerPos = playerObject.transform.position;
        yield return playerController.BMSkillMove(transform, nockbackForce);
        //DragonShoot();
        StartGraplingCooldown();
    }
    void StartGraplingCooldown()
    {
        isGraplingCooldown = true; // 쿨타임 시작
        graplingCooldownTimer = graplingCooldownDuration; // 타이머 초기화
    }

    void UpdateGraplingCooldown()
    {
        if (isGraplingCooldown)
        {
            graplingCooldownTimer -= Time.deltaTime; // 타이머 감소
            if (graplingCooldownTimer <= 0)
            {
                isGraplingCooldown = false; // 쿨타임 종료
                graplingCooldownTimer = 0;
            }
        }
    }


    IEnumerator ChargedStop()
    {
        isChargeReady = false;
        PpDirection();
        Debug.Log("돌진 준비");
        yield return new WaitForSeconds(isChargeDel);
        Debug.Log("돌진 준비 끝");
        isChargeReady = true;
        isRush = true;
        isMove = true;
    }

    IEnumerator ChargeToPlayer()
    {
        lastPlayerPos = playerObject.transform.position;
        yield return new WaitForSeconds(0.1f);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, PpDir, chargeDistace, playerLayer);
        Debug.DrawRay(transform.position, PpDir * chargeDistace, Color.red);

        float playerPos = Vector2.Distance(transform.position, lastPlayerPos);
        if (isChaging)
        {
            if (playerPos < chargeMaxDistance)
            {
                Debug.Log("거리 도달했으므로 돌진 정지" + playerPos);
                isMove = false;
               
            }
            MeleeAttack();
        }
        if (hit.collider != null && isMove == true)
        {
            isChaging = true;
            transform.position += (Vector3)PpDir * chargeSpeed * Time.deltaTime;
        }
        else if (hit.collider != null)
        {
            UnChargeTime = 0;
        }
        else if (hit.collider == null)
        {
            if (isChaging == true)
            {
                UnChargeTime += Time.deltaTime;
                if (UnChargeTime > NotChargeTime)
                {
                    isChargeReady = false;
                    isRush = false;
                    UnChargeTime = 0;
                    yield return new WaitForSeconds(5f); // 5초 후 다시 돌진 준비
                    StartCoroutine(ChargedStop());
                }
            }
        }

       
    }


    void MeleeAttack()
    {
        if (isGraplingCooldown)
        {
            Debug.Log("쿨타임 중입니다.");
            return;
        }

        Debug.Log("척력공격 애니메이션 재생");

        isAttack = false;
        Collider2D[] coliderMelee = Physics2D.OverlapBoxAll(bounsing.position, bounsingSize, 0);

        foreach (Collider2D collider in coliderMelee)
        {
            if (collider.CompareTag("Player"))
            {
                isAttack = true;
            }
        }


        if (playerController != null && isAttack)
        {
            Debug.Log("공격 범위에 있음 -> 넉백");
            StartCoroutine(GraplingKnockback(playerController, meleeAttackForce));
        }
        else if (isAttack == false && isMove == false)
        {
            if (isCreateZakoom == false && !isZaKoomActive && isSpawningDragonWeapon) // 자쿰 돌이 활성화되어 있는지 확인
            {

                StartCoroutine(ZaKoomCreate());
            }

            if (!isSpawningDragonWeapon)
            {
                isAttackStart = true;
                isSpawningDragonWeapon = true;
                StartCoroutine(DragonWeaponSpawner());
            }



            Debug.Log("공격 범위에 없음");
        }

    }


    void DragonWeaponFunc()
    {
        if (activeDragonWeapons + 8 <= maxDragonWeapons)
        {
            Debug.Log("용언 생성");

            Vector3 spawnPosition = (Vector3)lastPlayerPos;
            if (spawnInCircle)
            {
                SpawnInCircle(spawnPosition);
            }
            else
            {
                SpawnInSquare(spawnPosition);
            }
        }
        else
        {
            Debug.Log("최대 용언 개수에 도달");
            ReleaseDragonWeapons();
        }
    }

    void SpawnInCircle(Vector3 spawnPosition)
    {
        float radius = 1f; // 원의 반지름
        int weaponCount = 8; // 한 번에 생성할 무기 개수

        for (int i = 0; i < weaponCount; i++)
        {
            float angle = i * Mathf.PI * 2f / weaponCount; // 각 무기의 각도
            float x = spawnPosition.x + Mathf.Cos(angle) * radius;
            float y = spawnPosition.y + Mathf.Sin(angle) * radius;

            Vector3 position = new Vector3(x, y, 0f);
            var weapon = _Pool.Get();
            weapon.transform.position = position;
            activeDragonWeapons++;
        }
        spawnInCircle = false; // 다음 번에는 사각형으로 스폰
    }

    void SpawnInSquare(Vector3 spawnPosition)
    {
        float halfSize = 1.0f; // 사각형의 반 크기
        int weaponCount = 8; // 한 번에 생성할 무기 개수
        Vector3[] offsets = new Vector3[]
        {
            new Vector3(-halfSize, halfSize, 0),
            new Vector3(0, halfSize, 0),
            new Vector3(halfSize, halfSize, 0),
            new Vector3(-halfSize, 0, 0),
            new Vector3(halfSize, 0, 0),
            new Vector3(-halfSize, -halfSize, 0),
            new Vector3(0, -halfSize, 0),
            new Vector3(halfSize, -halfSize, 0)
        };

        for (int i = 0; i < weaponCount; i++)
        {
            Vector3 position = spawnPosition + offsets[i];
            var weapon = _Pool.Get();
            weapon.transform.position = position;
            activeDragonWeapons++;
        }
        spawnInCircle = true; // 다음 번에는 원형으로 스폰
    }

    void ReleaseDragonWeapons()
    {
        for (int i = 0; i < 8; i++)
        {
            var weapon = FindActiveDragonWeapon(); // 활성화된 용언 오브젝트를 찾아서 릴리즈
            if (weapon != null)
            {
                _Pool.Release(weapon);
                activeDragonWeapons--;
            }
        }
    }

    ProPhetWeapon FindActiveDragonWeapon()
    {
        
        ProPhetWeapon[] weapons = FindObjectsOfType<ProPhetWeapon>();
        foreach (var weapon in weapons)
        {
            if (weapon.gameObject.activeInHierarchy)
            {
                return weapon;
            }
        }
        return null;
    }

    IEnumerator DragonWeaponSpawner()
    {
        yield return new WaitForSeconds(1.0f);
        while (true)
        {
            DragonWeaponFunc();
            yield return new WaitForSeconds(2.0f); // 2초 간격으로 8개씩 생성
        }
    }

    private ProPhetWeapon CreateWeapon()
    {
        ProPhetWeapon weapon = Instantiate(DragonWeapon).GetComponent<ProPhetWeapon>();
        weapon.SetManagedPool(_Pool);
        return weapon;
    }

    private void OnGetWeapon(ProPhetWeapon weapon)
    {
        weapon.gameObject.SetActive(true);
    }

    private void OnReleaseWeapon(ProPhetWeapon weapon)
    {
        weapon.gameObject.SetActive(false);
    }

    private void OnDestroyWeapon(ProPhetWeapon weapon)
    {
        Destroy(weapon.gameObject);
    }
    public Vector2 PpDir;

    void PpDirection()
    {
        playerTransform = playerObject.transform;
        if (playerTransform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
            PpDir = Vector2.right;
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
            PpDir = Vector2.left;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(bounsing.position, bounsingSize);
    }
}
