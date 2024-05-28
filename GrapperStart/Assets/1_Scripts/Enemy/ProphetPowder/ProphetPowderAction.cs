using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PpPaze
{
    STATE_PATROL,
    STATE_ROCKATTACK,
    STATE_STOP,
    STATE_FOLLOW,
    STATE_DIE,
}
public class ProphetPowderAction : MonoBehaviour
{
    // 근거리 사용 무기: 척력
    //원거리 사용 무기: 인력, 용언
    SpriteRenderer PpSpriteRenderer;
    void Start()
    {
        isAttackStart = false;
        PpSpriteRenderer = GetComponent<SpriteRenderer>();
        //맵 한 가운데에서 순간이동을 하며 등장
        StartCoroutine(FadeInSprite());
    }

    public bool isAttackStart;

    public GameObject TelePortEffect;
    private GameObject TpEffectPrefab;
    public float TelePortDel; //이펙트 보여줄 딜레이. 이 시간이 끝나면 순간이동
    IEnumerator FadeInSprite()
    {
        isAttackStart = false;

        Color color = PpSpriteRenderer.color;
        color.a = 0;
        PpSpriteRenderer.color = color;
        if(TpEffectPrefab == null)
        {
             TpEffectPrefab = Instantiate(TelePortEffect, transform.position, Quaternion.identity);
        }
      
        yield return new WaitForSeconds(TelePortDel);
        Destroy(TpEffectPrefab);
        while (color.a < 1)
        {
            color.a += Time.deltaTime;
            PpSpriteRenderer.color = color;
            yield return null;
        }

        color.a = 1;
        PpSpriteRenderer.color = color;

        isAttackStart = true;
    }
    void Update()
    {
        if(isAttackStart)
        {
            DragonShoot();
        }

        GraplingPlayerNockBack();
        UpdateGraplingCooldown();

    }

    public LayerMask playerLayer;
    public float ShootDistance;
    public float projectileSpeed;


    public GameObject DragonWeapon;
    private GameObject DragonWeaponPrefab;
    void DragonShoot() //기본공격1 : 용언 발사
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, Mathf.Infinity, playerLayer);
        Debug.DrawRay(transform.position, Vector2.right * ShootDistance, Color.red);

        if (hit.collider != null)
        {
            float playerdistance = Mathf.Abs(hit.point.x - transform.position.x);

            if (playerdistance < ShootDistance)
            {
                //용언 발사
                if(DragonWeaponPrefab == null)
                {
                    DragonWeaponPrefab = Instantiate(DragonWeapon, transform.GetChild(1).position, Quaternion.identity);
                    Rigidbody2D rb = DragonWeaponPrefab.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.velocity = Vector2.right * projectileSpeed;
                    }
                }
               
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
        if(CtEffectPrefab == null)
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
                        StartCoroutine(GraplingKnockback(playerController));
                        
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
    IEnumerator GraplingKnockback(PlayerControllerRope playerController)
    {
        yield return playerController.BMSkillMove(transform, nockbackForce);
        yield return new WaitForSeconds(0.5f);
        DragonShoot();
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
}
