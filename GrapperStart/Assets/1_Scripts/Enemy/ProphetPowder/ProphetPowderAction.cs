using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;
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
public class ProphetPowderAction : MonoBehaviour,Enemies
{
    // 근거리 사용 무기: 척력
    //원거리 사용 무기: 인력, 용언
    public PpPaze ppState;
    SpriteRenderer PpSpriteRenderer;
    Transform playerObject;
    Animator PpAnim;
    PlayerControllerRope playerController;
    Grapling grapling;
    private Transform playerTransform;

    private IObjectPool<PpZakoom> _zacomPool;
    private IObjectPool<PpBoom> _explosionPool;
    private IObjectPool<ProPhetWeapon> _Pool;

    [Header("##순간이동")]
    public GameObject TelePortEffect;
    private GameObject TpEffectPrefab;
    public float TelePortDel; // 이펙트 보여줄 딜레이. 이 시간이 끝나면 순간이동
    [Header("##기본 이동")]
    public bool isMoving;
    public float moveSpeed;
    public float moveDistance = 5.0f; // 이동할 거리

    [Header("##돌진 관련 변수")]
    public bool isRush = false;
    public float chargeMaxDistance = 5f; // 돌진 가능한 최대 거리
    public float chargeDistace = 5f; // 돌진 조건 거리.
    public float isChargeDel = 1f; // 돌진 딜레이
    private Vector2 lastPlayerPos;
    public bool isChaging;
    public float chargeSpeed = 5f;
    public bool isChargingCooldown;
    public float chargingCoolTimer = 5f; // 쿨타임 타이머
    public float chargingCoolTimerDur;// 쿨타임 지속 시간
    [Header("##적이 그래플링하여 날라올 때 필요한 변수")]
    public float nockbackForce;
    public bool isGraplingCooldown;
    public float graplingCooldownTimer; // 쿨타임 타이머
    public float graplingCooldownDuration = 5f; // 쿨타임 지속 시간

    public LayerMask playerLayer;
    public float ShootDistance;

    [Header("##용언 공격을 하기 위한 변수와 필요 오브젝트")]
    public Transform bounsing;
    public Vector3 bounsingSize;
    public bool isAttack = false;
    public float meleeAttackForce;
    public GameObject DragonWeapon;
    private GameObject DragonWeaponPrefab;
    public GameObject ZacomPrefab;
    public GameObject ExplosionPrefab;
    public int activeDragonWeapons = 0; // 활성화된 용언 오브젝트 수를 추적하기 위한 변수
    public bool isSpawningDragonWeapon = false; // 용언 소환 중인지 확인하는 플래그
    public bool spawnShapeCircle = true;
    public bool isDragonAtkReady;

    [Header("##용언 패턴들의 사이즈와 갯수")]
    public float CircleRadius;
    public int CircleCount;
    public float squareSize;
    public int squareCount;
    [Header("##용언,자쿰 돌 , 자쿰 돌 폭탄 이펙트 딜레이")]
    public float WeaponLifeTime;
    public float ZakoomLifeTime;
    public float BoomEffLifeTime;
    public bool isDragonAttackCooldown = false;
    public float dragonAttackCooldownDuration = 10f;
    public float dragonAttackCooldownTimer = 0f;


    private void Awake()
    {
        _zacomPool = new UnityEngine.Pool.ObjectPool<PpZakoom>(CreateZacom, OnGetZacom, OnReleaseZacom, OnDestroyZacom, maxSize: 10);
        _explosionPool = new UnityEngine.Pool.ObjectPool<PpBoom>(CreateExplosion, OnGetExplosion, OnReleaseExplosion, OnDestroyExplosion, maxSize: 10);
        _Pool = new UnityEngine.Pool.ObjectPool<ProPhetWeapon>(CreateWeapon, OnGetWeapon, OnReleaseWeapon, OnDestroyWeapon, maxSize: 6); // 명시적으로 UnityEngine.Pool 네임스페이스를 지정
    }
    private PpZakoom CreateZacom()
    {
        PpZakoom zacom = Instantiate(ZacomPrefab).GetComponent<PpZakoom>();
        zacom.SetManagedPool(_zacomPool);
        return zacom;
    }

    private void OnGetZacom(PpZakoom zacom)
    {
        zacom.gameObject.SetActive(true);
    }

    private void OnReleaseZacom(PpZakoom zacom)
    {
        zacom.gameObject.SetActive(false);
    }

    private void OnDestroyZacom(PpZakoom zacom)
    {
        Destroy(zacom.gameObject);
    }

    private PpBoom CreateExplosion()
    {
        PpBoom explosion = Instantiate(ExplosionPrefab).GetComponent<PpBoom>();
        explosion.SetManagedPool(_explosionPool);
        return explosion;
    }

    private void OnGetExplosion(PpBoom explosion)
    {
        explosion.gameObject.SetActive(true);
    }

    private void OnReleaseExplosion(PpBoom explosion)
    {
        explosion.gameObject.SetActive(false);
    }

    private void OnDestroyExplosion(PpBoom explosion)
    {
        Destroy(explosion.gameObject);
    }
    void Start()
    {
        this.ppState = PpPaze.STATE_None;
        playerController = FindAnyObjectByType<PlayerControllerRope>();
        grapling = FindAnyObjectByType<Grapling>();
        playerObject = GameObject.Find("Player").transform;
        PpSpriteRenderer = GetComponent<SpriteRenderer>();
        PpAnim = GetComponent<Animator>();

        PpAnim.enabled = false;
        isMoving = false;

        if (ppState == PpPaze.STATE_None)
        {
            StartCoroutine(FadeInSprite());
        }
    }

    IEnumerator FadeInSprite()
    {
        Color color = PpSpriteRenderer.color;
        color.a = 0;
        PpSpriteRenderer.color = color;
        if (TpEffectPrefab == null)
        {
            TpEffectPrefab = Instantiate(TelePortEffect, transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(TelePortDel);
        Destroy(TpEffectPrefab);

        PpAnim.enabled = true;
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
        PpAnim.Play("PP_Move");
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
        PpAnim.Play("PP_Idle");
    }

    void Set() //페이즈 넘어갈 때 초기화 하는 함수.
    {
        isRush = false;
        isMoving = false;
        isChaging = false;
        isDragonAtkReady = false;
        isMeleeAttack = false;
        isSpawningDragonWeapon = false;
        lastPlayerPos = Vector2.zero;
        Debug.Log("초기화");
    }

    void Update()
    {

        GraplingPlayerNockBack(); // 1. 플레이어가 그래플링을 선지자 파우더에게 했을 때만 실행됨
        
        UpdateGraplingCooldown(); // 2. 플레이어를 밀치고 나서만 실행되며, 쿨타임 메서드임.
        UpdateDragonAttackCooldown();
        UpdateMeleeAttackCooldown();

        switch (this.ppState)
        {
            case PpPaze.STATE_Basic:
                if(grapling.isAttatch == false)
                {
                    StartCoroutine(PpMove());
                }
               
                break;
            case PpPaze.STATE_PazeOne:
                if (isRush == true && grapling.isAttatch == false && isChaging == false)
                {
                    StartCoroutine(ChargeToPlayer());
                }
                else if (isRush ==false && isMeleeAttack == true)
                {
                    MeleeAttack();
                }
                break;
            case PpPaze.STATE_PazeTwo:          
                Debug.Log("2페이즈");
                break;
                // 필요하다면 다른 상태 추가
        }

        PpDirection();
    }



    void GraplingPlayerNockBack() //기본공격3: 적이 그래플링 스킬 사용하면 밀쳐내기 (쿨타임 존재) 
    {
        if (isGraplingCooldown)
        {
            return; // 아무것도 실행안하고 반환
        }
        //밀쳐내기 코드 구현
        
        if (grapling.isLerping)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, Mathf.Infinity, playerLayer);
            Debug.DrawRay(transform.position, Vector2.right * ShootDistance, Color.blue);
            if (hit.collider != null)
            {
                float playerdistance = Mathf.Abs(hit.point.x - transform.position.x);

                if (playerdistance < ShootDistance)
                {
                    StartCoroutine(GraplingKnockback(playerController, nockbackForce, "Grapling"));
                }
            }
        }
        else
        {
            Debug.Log("적이 그래플링 하지 않음");
        }

    }
    public bool isKnockbackInProgress = false;
    IEnumerator GraplingKnockback(PlayerControllerRope playerController, float nockbackForce,string attackWay)
    {
        PpAnim.SetTrigger("PPNockBackSkill");
        lastPlayerPos = playerObject.transform.position;
        yield return playerController.BMSkillMove(transform, nockbackForce);
        PpAnim.SetBool("PPMeleeAtk", true);
        //DragonShoot();
        if (attackWay == "Grapling")
        {
            Debug.Log("그래플링 넉백");
            StartGraplingCooldown();
        }
        else if (attackWay == "MeleeAttack")
        {
            Debug.Log("척력 넉백");
            StartMeleeAttackCooldown();
        }

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

    void UpdateDragonAttackCooldown()
    {
        if (isDragonAttackCooldown)
        {
            dragonAttackCooldownTimer -= Time.deltaTime; // 쿨타임 타이머 감소
            if (dragonAttackCooldownTimer <= 0)
            {
                spawnShapeCircle = !spawnShapeCircle;
                isDragonAttackCooldown = false; // 쿨타임 종료
                dragonAttackCooldownTimer = 0;
            }
        }
    }

    // 용언 공격 쿨타임 시작
    void StartDragonAttackCooldown()
    {
        isDragonAttackCooldown = true; // 쿨타임 시작
        dragonAttackCooldownTimer = dragonAttackCooldownDuration; // 쿨타임 타이머 설정
    }

    public bool isMeleCooldown;
    public float MeleeCoolTimer; // 쿨타임 타이머
    public float MeleeCoolTimerDur;// 쿨타임 지속 시간
    void StartMeleeAttackCooldown()
    {
        isMeleCooldown = true; // 쿨타임 시작
        MeleeCoolTimer = MeleeCoolTimerDur; // 타이머 초기화
    }
    void UpdateMeleeAttackCooldown()
    {
        if (isMeleCooldown)
        {
            MeleeCoolTimer -= Time.deltaTime; // 타이머 감소
            if (MeleeCoolTimer <= 0)
            {
                isMeleCooldown = false; // 쿨타임 종료
                MeleeCoolTimer = 0;
                isKnockbackInProgress = false;
            }
        }
    }

    IEnumerator ChargeToPlayer()
    {
        if (isChargingCooldown)
        {
            Debug.Log("돌진 쿨타임 중입니다.");
            yield break;
        }
        isChaging = true; // 돌진 시작
        //돌진 준비 애니메이션 재생. 
        yield return new WaitForSeconds(2.0f);
        PpAnim.Play("PP_Idle");
        isDragonAtkReady = false; //돌진 중 용언 소환 못하게.false 처리


        float elapsedTime = 0f;
        Vector2 initialPosition = (Vector2)transform.position; // 명시적으로 캐스팅하여 Vector2로 변환

        Vector2 targetPosition = lastPlayerPos;
        PpAnim.Play("PP_Charging");
        while (elapsedTime < chargeDistace)
        {
            targetPosition = lastPlayerPos;

            float newX = Mathf.Lerp(initialPosition.x, targetPosition.x, elapsedTime * chargeSpeed);

            transform.position = new Vector3(newX, transform.position.y, transform.position.z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        PpAnim.Play("PP_Idle");
        yield return new WaitForSeconds(2.0f);
        isRush = false; //척력을 위한 bool변수 셋팅 1     
        isMeleeAttack = true;//척력을 위한 bool변수 셋팅 2
    }

    void ChagingSet()//돌진을 위한 bool변수 셋팅
    {
        isRush = true; //
        isChaging = false; // 돌진 해제
    }

    public bool isMeleeAttack;
    void MeleeAttack()
    {
        if (isMeleCooldown)
        {
            Debug.Log("용언 쿨타임 중입니다.");
            return;
        }
        isAttack = false;
        isDragonAtkReady = true;

        Collider2D[] coliderMelee = Physics2D.OverlapBoxAll(bounsing.position, bounsingSize, 0);

        foreach (Collider2D collider in coliderMelee)
        {
            if (collider.CompareTag("Player"))
            {     
                isAttack = true;
            }
        }

        if(isAttack == false && isDragonAtk == false)
        {
          
            PpAnim.SetBool("PPMeleeAtk", true);
        }

        if (isAttack && isMeleeAttack == true && !isKnockbackInProgress)
        {
            Debug.Log("척력 공격");
            StartCoroutine(GraplingKnockback(playerController, meleeAttackForce,"MeleeAttack"));
        }
        else if (isAttack == false && isDragonAtkReady == true)
        {
            lastPlayerPos = playerObject.transform.position;
            if (isSpawningDragonWeapon == false && isDragonAttackCooldown == false)
            {
                isSpawningDragonWeapon = true;
                DragonWeaponFunc(lastPlayerPos); //용언 공격 메서드 호출
            }

        }


    }


    void DragonWeaponFunc(Vector3 playerPos)
    {

        Vector3 spawnPosition = (Vector3)playerPos;

        if (spawnShapeCircle)
        {
            SpawnInCircle(spawnPosition);
        }
        else
        {
            SpawnInSquare(spawnPosition);
        }
    }

    public bool isDragonAtk;
    void SpawnInCircle(Vector3 spawnPosition)
    {


        isDragonAtk = true;
        for (int i = 0; i < CircleCount; i++)
        {
            PpAnim.SetTrigger("PPLongAtk");

            float angle = i * Mathf.PI * 2f / CircleCount; // 각 무기의 각도
            float x = spawnPosition.x + Mathf.Cos(angle) * CircleRadius;
            float y = spawnPosition.y + Mathf.Sin(angle) * CircleRadius;

            Vector3 position = new Vector3(x, y, 0f);
            var weapon = _Pool.Get();
            weapon.transform.position = position;
            activeDragonWeapons++;

            StartCoroutine(SpawnZacomAndExplosion(position));
        }
       

    }


    IEnumerator SpawnZacomAndExplosion(Vector3 position)
    {
        isDragonAtk = false;
        yield return new WaitForSeconds(WeaponLifeTime); //이 시간 뒤에 용언 비활성화  
        ReleaseDragonWeapons(); //용언 비활성화
        isDragonAtk = true;
        PpAnim.SetTrigger("PPLongAtk");
        var zacom = _zacomPool.Get(); //자쿰 풀링으로 생성 후
        zacom.transform.position = position; //용언 위치를 자쿰 위치와 통일 시키고

        yield return new WaitForSeconds(ZakoomLifeTime); //0.5초 뒤
        PpAnim.SetTrigger("PPLongAtk");
        var explosion = _explosionPool.Get(); //자쿰 폭발 이펙트 생성 후
        explosion.transform.position = position; //용언 위치를 자쿰 폭발 위치와 통일 시키고

        zacom.ReturnToPool(); //자쿰 비활성화 - 이때는 자쿰 폭발 이펙트 보여지는 상태
        yield return new WaitForSeconds(BoomEffLifeTime); // 0.2초 뒤
        explosion.ReturnToPool(); //자쿰 폭탄 이펙트 비활성화

        isDragonAtk = false;
        isSpawningDragonWeapon = false;


        StartDragonAttackCooldown();
    }

    void SpawnInSquare(Vector3 spawnPosition)
    {
        
        Vector3[] offsets = new Vector3[]
        {
            new Vector3(-squareSize, squareSize, 0),
            new Vector3(0, squareSize, 0),
            new Vector3(squareSize, squareSize, 0),
            new Vector3(-squareSize, 0, 0),
            new Vector3(squareSize, 0, 0),
            new Vector3(-squareSize, -squareSize, 0),
            new Vector3(0, -squareSize, 0),
            new Vector3(squareSize, -squareSize, 0)
        };

        for (int i = 0; i < squareCount; i++)
        {
            PpAnim.SetTrigger("PPLongAtk");

            Vector3 position = spawnPosition + offsets[i];
            var weapon = _Pool.Get();
            weapon.transform.position = position;
            activeDragonWeapons++;

            StartCoroutine(SpawnZacomAndExplosion(position));
        }


    
        


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

    public bool isChargePlayer;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(isChaging)
            {
                isChargePlayer = true;
            }
         
        }
    }
    void PpDirection()
    {
        playerTransform = playerObject.transform;
        if (playerTransform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            PpDir = Vector2.right;
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
            PpDir = Vector2.left;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(bounsing.position, bounsingSize);
    }

    IEnumerator Enemies.GraplingAtkDamaged()
    {
        throw new System.NotImplementedException();
    }

    public PPData ppdata;
    IEnumerator Enemies.baseDamaged()
    {
        ppdata.DamagedHp(10.0f);

        yield return null;

        if (ppdata.ppHP <= float.Epsilon)
        {

        }
    }

    void Enemies.PlayerToDamaged()
    {
        throw new System.NotImplementedException();
    }

    IEnumerator Enemies.Died()
    {
        throw new System.NotImplementedException();
    }

    void Enemies.SpeedDown()
    {
        throw new System.NotImplementedException();
    }

    void Enemies.EnemySet()
    {
        throw new System.NotImplementedException();
    }

    IEnumerator Enemies.NotFindPlayer(SpriteRenderer sprite)
    {
        throw new System.NotImplementedException();
    }

    IEnumerator Enemies.EnemyAtkStop()
    {
        yield return null;
    }

    void Enemies.UpdateOutline(bool outline)
    {
        throw new System.NotImplementedException();
    }
}
