using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    void Start()
    {
        this.ppState = PpPaze.STATE_None;
        playerObject = GameObject.Find("Player").transform;
        isAttackStart = false;
        PpSpriteRenderer = GetComponent<SpriteRenderer>();
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
    public float moveDistance = 5.0f; // 이동할 거리

    IEnumerator PpMove()
    {
        if (isMoving)
        {
            yield break;
        }

        isMoving = true;

        float elapsedTime = 0f;
        Vector2 initialPosition = (Vector2)transform.position; // 명시적으로 캐스팅하여 Vector2로 변환

        // Calculate the target position
        Vector2 direction = (lastPlayerPos - initialPosition).normalized;
        Vector2 targetPosition = initialPosition + direction * moveDistance;

        while (elapsedTime < 1.0f)
        {
            transform.position = Vector2.Lerp(initialPosition, targetPosition, elapsedTime);
            elapsedTime += Time.deltaTime * chargeSpeed;
            yield return null;
        }
        transform.position = targetPosition;

        this.ppState = PpPaze.STATE_PazeOne;
        isRush = true;
        isMoving = false;
    }

    public bool isAttackStart;
    public bool isMoving;

    public GameObject TelePortEffect;
    private GameObject TpEffectPrefab;
    public float TelePortDel; // 이펙트 보여줄 딜레이. 이 시간이 끝나면 순간이동

    void Update()
    {

        //GraplingPlayerNockBack(); // 1. 플레이어가 그래플링을 선지자 파우더에게 했을 때만 실행됨
        //UpdateGraplingCooldown(); // 2. 플레이어를 밀치고 나서만 실행되며, 쿨타임 메서드임.

        switch (this.ppState)
        {
            case PpPaze.STATE_Basic:
                StartCoroutine(PpMove());
                break;
            case PpPaze.STATE_PazeOne:
                if (isChargeReady == false)
                {
                    StartCoroutine(ChargedStop());
                }
                else if (isRush == true && isChargeReady == true)
                {
                    StartCoroutine(ChargeToPlayer());
                }
                break;
                // 필요하다면 다른 상태 추가
        }
    }

    public LayerMask playerLayer;
    public float ShootDistance;
    public float projectileSpeed;


    public GameObject DragonWeapon;
    private GameObject DragonWeaponPrefab;
    void DragonShoot() //기본공격1 : 용언 발사
    {
        PpDirection();

        RaycastHit2D hit = Physics2D.Raycast(transform.position, PpDir, Mathf.Infinity, playerLayer);
        Debug.DrawRay(transform.position, PpDir * ShootDistance, Color.red);

        if (hit.collider != null)
        {
            float playerdistance = Mathf.Abs(hit.point.x - transform.position.x);

            if (playerdistance < ShootDistance)
            {
                //용언 소환
                //if(DragonWeaponPrefab == null)
                //{

                //}

                if (isCreateZakoom == false)
                {
                    StartCoroutine(ZaKoomCreate());
                }

            }
        }
    }

    public bool isCreateZakoom;
    public GameObject ZaKoomObj;
    private GameObject ZaKoomPrefab;

    public GameObject CreateEffect;
    private GameObject CtEffectPrefab;

    public float ZaKoomCreateDel; //몇 초 뒤에 자쿰 돌을 생성할건지 ,이 초가 지나면 자쿰 돌 생성
    public float ZaKoomEffectDel; //몇 초 뒤에 자쿰 돌 소환 이펙트를 보여줄건지, 이 초가 지나면 자쿰 돌 소환 이펙트 생성
    IEnumerator ZaKoomCreate() //기본공격2 : 자쿰 돌 소환 .. 폭파?
    {
        isAttackStart = false; //false로 설정하여 이 코루틴 진입 시 프레임 1번만 돌고 Update문에서 호출 x -> true로 설정 시 용언 발사 가능.
        yield return new WaitForSeconds(ZaKoomEffectDel);
        if (CtEffectPrefab == null)
        {
            Debug.Log("자쿰 돌 소환 이펙트");
            CtEffectPrefab = Instantiate(CreateEffect, transform.GetChild(0).position, Quaternion.identity);

        }

        yield return new WaitForSeconds(ZaKoomCreateDel);
        Destroy(CtEffectPrefab);
        Debug.Log("자쿰 돌 소환 이펙트 삭제");
        if (ZaKoomPrefab == null)
        {
            Debug.Log("자쿰 돌 소환");
            ZaKoomPrefab = Instantiate(ZaKoomObj, transform.GetChild(0).position, Quaternion.identity);
        }

        isCreateZakoom = true;
        //자쿰 돌 소환
        yield return null;
    }


    public float nockbackForce;
    public bool isGraplingCooldown;
    public float graplingCooldownTimer; // 쿨타임 타이머
    public float graplingCooldownDuration = 5f; // 쿨타임 지속 시간
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


    public float chargeSpeed = 5f;
    private Transform playerTransform;
    public bool isRush = false;

    public float chargeMaxDistance = 5f; // 돌진 가능한 최대 거리
    public float chargeDistace = 5f; // 돌진 조건 거리.

    public float isChargeDel = 1f; // 돌진 딜레이

    public float UnChargeTime;
    public float NotChargeTime;
    private Vector2 lastPlayerPos;
    public bool isMove;
    public bool isChargeReady;

    IEnumerator ChargedStop()
    {
        isChargeReady = false;
        PpDirection();
        Debug.Log("돌진 준비");
        //돌진 준비 애니메이션 재생 코드 여기다. 
        yield return new WaitForSeconds(isChargeDel);
        Debug.Log("돌진 준비 끝");
        isChargeReady = true;
        isRush = true;
    }

    IEnumerator ChargeToPlayer()
    {
        yield return new WaitForSeconds(0.1f);


        Debug.Log("돌진 시작");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, PpDir, chargeDistace, playerLayer);
        Debug.DrawRay(transform.position, PpDir * chargeDistace, Color.red);

        float playerdistance = Mathf.Abs(lastPlayerPos.x - transform.position.x);

        if (hit.collider != null)
        {
            if (playerdistance > chargeMaxDistance)
            {
                isMove = true;
                transform.position += (Vector3)PpDir * chargeSpeed * Time.deltaTime;
            }
            else
            {
                Debug.Log("돌진 중지");
            }
        }

        else if (hit.collider == null)
        {
            if (isMove)
            {
                UnChargeTime += Time.deltaTime;
                if (UnChargeTime > NotChargeTime)
                {
                    //기본 상태 애니메이션 
                    isRush = false;
                    isMove = false;
                    UnChargeTime = 0;

                    Debug.Log("돌진 ");
                    Invoke("FindPlayer", 3.0f);
                }
            }

            Debug.Log("hit충돌x");
        }
        // 코루틴 종료 시 플래그 해제
    }

    public Transform bounsing;
    public Vector3 bounsingSize;
    public bool isAttack = false;
    public float meleeAttackForce;
    void MeleeAttack()
    {
        if (isGraplingCooldown)
        {
            Debug.Log("쿨타임 중입니다.");
            return; // 아무것도 실행안하고 반환
        }

        isAttack = false;
        Collider2D[] coliderMelee = Physics2D.OverlapBoxAll(bounsing.position, bounsingSize, 0);

        foreach (Collider2D collider in coliderMelee)
        {
            if (collider.CompareTag("Player"))
            {
                isAttack = true;
            }
        }

        if (isAttack)
        {
            //척력공격 애니메이션 재생
            Debug.Log("공격 범위에 있음 -> 넉백");
            PlayerControllerRope playerController = FindAnyObjectByType<PlayerControllerRope>();
            if (playerController != null)
            {
                StartCoroutine(GraplingKnockback(playerController, meleeAttackForce));



            }

        }
        else
        {
            Debug.Log("공격 범위에 없음");
        }
    }




    public Vector2 PpDir;
    void PpDirection()
    {
        playerTransform = playerObject.transform;
        if (playerTransform.transform.position.x > transform.position.x)
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

        Gizmos.DrawWireCube(bounsing.position, bounsingSize);//DrawWireCube(pos.position,boxsize)      
    }
}
