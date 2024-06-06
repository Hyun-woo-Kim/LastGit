using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEditor.PlayerSettings;
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
    public float moveSpeed;
    public float moveDistance = 5.0f; // 이동할 거리

    [Header("##Chaged")]
    public bool isRush = false;
    public float chargeMaxDistance = 5f; // 돌진 가능한 최대 거리
    public float chargeDistace = 5f; // 돌진 조건 거리.
    public float isChargeDel = 1f; // 돌진 딜레이
    private Vector2 lastPlayerPos;
    public bool isChaging;

    


    [Header("##GraplingNockBackAtk")]
    public float nockbackForce;
    public bool isGraplingCooldown;
    public float graplingCooldownTimer; // 쿨타임 타이머
    public float graplingCooldownDuration = 5f; // 쿨타임 지속 시간
    public bool isAttackStart;
    public LayerMask playerLayer;
    public float ShootDistance;

    [Header("##DragonWeaponAtk")]
    private IObjectPool<ProPhetWeapon> _Pool;
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
            transform.position = Vector2.Lerp(initialPosition, targetPosition, elapsedTime * moveSpeed);
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

       
        UpdateChargingCooldown();

        switch (this.ppState)
        {
            case PpPaze.STATE_Basic:
                if(grapling.isAttatch == false)
                {
                    StartCoroutine(PpMove());
                }
               
                break;
            case PpPaze.STATE_PazeOne:
                if (isRush == true && grapling.isAttatch == false && !isChaging)
                {
                    StartCoroutine(ChargeToPlayer());
                }
                else if (!isRush && isDragonAtkReady)
                {
                    MeleeAttack();
                }

                break;
                // 필요하다면 다른 상태 추가
        }


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

    public bool isChargingCooldown;
    public float chargingCoolTimer = 5f; // 쿨타임 타이머
    public float chargingCoolTimerDur;// 쿨타임 지속 시간
    void StartChargingCooldown()
    {
        isChargingCooldown = true; // 쿨타임 시작
        chargingCoolTimer = chargingCoolTimerDur; // 타이머 초기화
    }

    void UpdateChargingCooldown()
    {
        if (isChargingCooldown)
        {
            chargingCoolTimer -= Time.deltaTime; // 타이머 감소
            if (chargingCoolTimer <= 0)
            {
                isChargingCooldown = false; // 쿨타임 종료
                chargingCoolTimer = 0;
                isRush = true;
            }
        }
    }

    public bool isDragonAtkReady;
    IEnumerator ChargeToPlayer()
    {
        if (isChargingCooldown)
        {
            Debug.Log("돌진 쿨타임 중입니다.");
            yield break;
        }
        isChaging = true; // 돌진 시작
        chargeSpeed = 0.0f;
        //돌진 준비 애니메이션 재생. 
        Debug.Log("돌진 준비 애니메이션 재생");
        yield return new WaitForSeconds(2.0f);
        Debug.Log("돌진 애니메이션 재생");
    
        isDragonAtkReady = false;
        chargeSpeed = 0.5f;
        float elapsedTime = 0.0f;

        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = currentPosition + (Vector3)PpDir;

        while (elapsedTime < chargeSpeed)
        {
            isChaging = true;
            transform.position = Vector3.Lerp(currentPosition, targetPosition, elapsedTime / chargeSpeed);
            elapsedTime += Time.deltaTime;



            yield return null;
        }

        isRush = false;
        isDragonAtkReady = true;
        transform.position = targetPosition;
        isChaging = false; // 돌진 해제
        StartChargingCooldown();
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
        else if (isAttack == false && isDragonAtkReady == true)
        {
            lastPlayerPos = playerObject.transform.position;

            if (isSpawningDragonWeapon == false)
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
        yield return new WaitForSeconds(3.0f);
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
