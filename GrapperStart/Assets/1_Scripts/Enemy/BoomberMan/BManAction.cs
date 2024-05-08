using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class BManAction : MonoBehaviour,Enemies
{
    Animator BManim;
    CapsuleCollider2D collder;
    SpriteRenderer bmSpr;
    Rigidbody2D Bmrigid;
    public BMdata bmdata;

    Vector2 InitCollSize;
    void Start()
    {
        BManim = GetComponent<Animator>();
        bmSpr = GetComponent<SpriteRenderer>();
        collder = GetComponent<CapsuleCollider2D>();
        Bmrigid = GetComponent<Rigidbody2D>();
        //hand.gameObject.SetActive(false);
        animSpeed = BManim.speed;
        InitCollSize = collder.size;

        UpdateOutline(false);
    }

    // Update is called once per frame

    [Header("##Basic")]
    public Transform Findboxpos;
    public Vector2 FindboxSize;
    public Transform target;
    public bool isFindPlayer;
    public float followSpeed;
    public float moveSpeed;
    public bool isStand = false;
    public float growthRate = 0.9f; //일어나는 속도
    public float notFindDelayAnim; //플레이어를 찾지 못했을 때 스탠딩 상태 애니메이션 딜레이
    public bool isMove;
    public bool isPlayerMissing = false; //플레이어를 놓쳤을 때 bool
    public bool isDied; //죽었을 때
    public float animAimSpeed; //플레이어가 전등을 조준했을 때 애니메이션 속도를 느려지게하는 변수
    public float animSpeed; //원래 애니메이션 속도로 바뀌는 변수

    [Header("##Reaction")]
    public bool isReact;
    public Sprite reactSprite;//플레이어를 찾았을 때 스프라이트
    public Sprite missSprite; //플레이어를 놓쳤을 때 물음표 스프라이트.



    [Header("##Attack")]
    public float nockbackForce; //죽었을 때 플레이어게 넉백 힘 얼마나 줄지
    public bool hasAttacked; //공격했을 때 true
    public bool isPlayerFindCoroutineRunning = false;
    public Transform Punchboxpos;
    public Vector2 PunchboxSize;
    public bool isColliderPlayer; 
    public bool isPunch;
    public bool isAttack;
    public float DamagedValue; //플레이어에게 얼마나 데미지를 입을지
    public float basicDamage; //잽 공격 데미지
    public float powerDamage; //강화 공격 데미지

    [Header("##Patrol")]
    public Vector2 patrolDirection; //순찰 방향
    public Vector2 startPosition; //순찰 했을 때 처음 위치
    public float patrolSpeed = 2.0f; //순찰 속도
    public float patrolDistance = 5.0f; // 순찰 거리
    private bool hasReachedStartPosition;

    public IEnumerator GraplingAtkDamaged(float damage)
    {
        yield return null;
    }

    public bool isAtkStop;
    public bool chaneAttackMon;
    public float rayDistance = 5f; // Ray의 최대 거리
    public LayerMask PlayerLayer;
    public IEnumerator baseDamaged()
    {
        bmdata.DamagedHp(DamagedValue);
        chaneAttackMon = true;
        if(isStand)
        {
            

            if (bmdata.bmHp > 0)
            {
                isMove = false;
                isDamaged = true;

                BManim.SetTrigger("BmNockBack");
                yield return new WaitForSeconds(0.5f);
                BManim.SetTrigger("BmNockBackToIdle");

                isDamaged = false;
                isMove = true;
                isAtkStop = false;
            }

            else if(bmdata.bmHp <= 0)
            {
                Debug.Log("0 미만");
                StartCoroutine(Died());
            }

        }


        yield return null;

    }



    public void PlayerToDamaged()
    {
        ApplyDamageToPlayer(basicDamage);
    }

    public void PlayerToPowerDamaged()
    {
        ApplyDamageToPlayer(powerDamage);
    }
    private void ApplyDamageToPlayer(float damage)
    {
        PlayerControllerRope player = GameObject.Find("Player").GetComponent<PlayerControllerRope>();
        if (player != null)
        {
            PlayerData playerData = player.playerData;
            playerData.DamagedHp(damage);
        }
    }
    public IEnumerator Died()
    {

        Debug.Log("죽음");
        isDied = true;
        FlipEnemy(target);
        BManim.SetTrigger("BmBoomMove");
        Debug.Log(target.position);

        float elapsedTime = 0f;
        Vector2 initialPosition = transform.position;

        while (!isColliderPlayer && elapsedTime < 1.0f)
        {
            transform.position = Vector2.Lerp(initialPosition, target.position, elapsedTime);
            elapsedTime += Time.deltaTime * moveSpeed;

            yield return null;
        }

        if (isColliderPlayer) //isDied상태에서 플레이어와 충돌 시
        {
            PlayerControllerRope playerController = FindAnyObjectByType<PlayerControllerRope>();
            if (playerController != null)
            {
                playerController.BMSkillMove(transform, nockbackForce); // 적의 위치를 전달
            }
            DestroyBM();
        }

    }
    public void SpeedDown()
    {
        followSpeed = 0.1f;
        BManim.speed = animAimSpeed;
    }

    public void EnemySet()
    {
        followSpeed = 0.2f;
        BManim.speed = animSpeed;
    }

    void Update()
    {
        if (!isPlayerFindCoroutineRunning && isFindPlayer && chaneAttackMon) // 코루틴이 실행 중이 아닌 경우에만 실행
        {
            StartCoroutine(playerFind());
        }

        FindedPlayer();

    }

    void FindedPlayer()
    {
        Transform fourChild = transform.GetChild(5);
        SpriteRenderer spriteRenderer = fourChild.GetComponent<SpriteRenderer>();

        isReact = false;
        isFindPlayer = false;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(Findboxpos.transform.position, FindboxSize, 0);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                //Vector2 colliderBm = new Vector2(1.3f, 3.0f);
                //collder.size = colliderBm;
                target = collider.transform;
                hasReachedStartPosition = false;
                isFindPlayer = true;
                isReact = true;
               
              
            }
        }
        if (isFindPlayer && isStand  && isAtk == false)
        {
            isMove = true;
            isPlayerMissing = false;
            if (!hasFlipped)
            {
                FlipEnemy(target);
                hasFlipped = true; // 뒤집었음을 표시
            }
            else if(isMove)
            {
                BmMove();
            }
            
            
            
        }
        else if(isFindPlayer == false && isStand)
        {
            isMove = false;
            isPlayerMissing = true;
            StartCoroutine(NotFindPlayer(spriteRenderer));
            //필드 순회
        }

        if(isFindPlayer == true && chaneAttackMon == true && isStand && !isAttacking)
        {
            HandAttack();
        }       
        PatrolReaction(spriteRenderer);
    }

    bool hasFlipped = false;


    void PatrolMovement(float patrolSpeed, float patrolDis, Vector2 patrolDir, Vector2 starPos)
    {

        bool hasChangedDirection = false;

        if (!hasChangedDirection && Mathf.Abs(transform.position.x - startPosition.x) >= patrolDis)
        {
            Debug.Log("방향전환");
            patrolDirection *= -1;
            startPosition = transform.position;
            hasChangedDirection = true;
        }

        if (patrolDir == Vector2.right)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            transform.Translate(patrolDir * patrolSpeed * Time.deltaTime);
        }
        else if (patrolDir == Vector2.left)
        {
            Debug.Log("왼쪽 턴1");
            transform.localScale = new Vector3(1, 1, 1);
            transform.Translate(patrolDir * patrolSpeed * Time.deltaTime);
        }

        BManim.SetBool("BmIdle", false);
        BManim.SetFloat("BmAnimCount", 1.0f);

    }
    IEnumerator playerFind()
    {
        isPlayerFindCoroutineRunning = true;
        BManim.SetBool("BmFind", true);

        float sizeY = 1.7f;
        float targetSizeY = 2.6f; // 목표 크기

        while (sizeY <= targetSizeY)
        {
            float delta = growthRate * Time.deltaTime;
            sizeY += delta;

            // 크기가 목표 크기를 초과하는 경우 종료
            if (sizeY > 2.5f)
            {
                break;
            }

            Vector2 colliderBm = new Vector2(1.0f, sizeY);
            collder.size = colliderBm;

            BManim.SetFloat("BmAnimCount", -1.0f);

            yield return null;
        }
        BManim.SetBool("BmIdle",true);
        isStand = true;
  
    }//일어서는 메서드



    public bool isAttackReady;
    public bool isAttacking;
    void BmMove()
    {        
        if (!BManim.GetBool("BmAtk"))
        {
            hasFlipped = false;
            if (!hasFlipped)
            {
                FlipEnemy(target);
                hasFlipped = true; // 뒤집었음을 표시
            }
            BManim.SetBool("BmAtk", false);
            BManim.SetBool("BmIdle", false);
            BManim.SetFloat("BmAnimCount", 1.0f);

            Debug.Log("이동중");
            transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime * followSpeed);
        }

     
    }

    public bool isAtk;
    void HandAttack()
    {
        isAtk = false;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(Punchboxpos.position, PunchboxSize, 0);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                isAtk = true;
            }
        }

        float distanceToPlayer = DistanceToPlayer(transform, target);

        // 만약 플레이어와의 거리가 최대 공격 거리 이내에 있다면
        if (distanceToPlayer <= maxAttackDistance && isAtk && isDamaged == false)
        {
            // 적의 공격 애니메이션을 재생
            isAttacking = true;
            StartCoroutine(PlayAttackAnimation());
        }
        else
        {
            BManim.SetBool("BmAtk", false);
            
        }

    }
    public float maxAttackDistance = 3.0f;
    float DistanceToPlayer(Transform playerTransform, Transform enemyTransform)
    {
        return Vector3.Distance(playerTransform.position, enemyTransform.position);
    }

    public bool isDamaged;
    IEnumerator PlayAttackAnimation()
    {
        
        if(isDamaged == false)
        {
            isMove = false;
            UpdateOutline(true);
            BManim.SetBool("BmIdle", true); // 이전 스탠딩 상태 해제.
            yield return new WaitForSeconds(1.0f);
            Debug.Log("공격대기");
            BManim.SetBool("BmIdle", false); // 이전 스탠딩 상태 해제.
            BManim.SetBool("BmAtk", true); // 공격 애니메이션1
            BManim.SetFloat("BmAtkCount", 1.0f); // 공격 애니메이션2
            Debug.Log("공격끝");
            UpdateOutline(false);

            isAttacking = false; // 공격 종료
        }
        

    }


   



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(isDied)
            {
                isColliderPlayer = true;
            }
            
        }
    }
    void FlipEnemy(Transform _target)
    {
        if (transform.position.x > _target.position.x)
        {
            Debug.Log("왼쪽 턴2");
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (transform.position.x < _target.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    void PatrolReaction(SpriteRenderer spriteRenderer)
    {        
        if (isReact  && isStand)
        {
            spriteRenderer.sprite = reactSprite;
        }
        else if (isReact == false && isPlayerMissing == false)
        {
            spriteRenderer.sprite = null;

        }
    }

    public GameObject dieEffPrefab;
    private GameObject dieEff;
    void DestroyBM()
    {
        Destroy(this.gameObject);
        dieEff = Instantiate(dieEffPrefab, transform.position, Quaternion.identity);
        dieEff.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(Findboxpos.position, FindboxSize);//DrawWireCube(pos.position,boxsize)          
        Gizmos.DrawWireCube(Punchboxpos.position, PunchboxSize);//DrawWireCube(pos.position,boxsize)          
    }


    public Color OutLineEnemycolor = Color.yellow;

    [Range(0, 16)]
    public int outlineSize;
    public void UpdateOutline(bool outline)
    {
       
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        bmSpr.GetPropertyBlock(mpb);
        mpb.SetFloat("_Outline", outline ? 1f : 0);
        mpb.SetColor("_OutlineColor", OutLineEnemycolor);
        mpb.SetFloat("_OutlineSize", outlineSize);
        bmSpr.SetPropertyBlock(mpb);

    }

    public IEnumerator NotFindPlayer(SpriteRenderer sprite)
    {
        sprite.sprite = missSprite;


        if (!hasReachedStartPosition)
        {
            startPosition = transform.position;
            Debug.Log("1");
            if (target.position.x < transform.position.x)
            {
                Debug.Log("2");
                patrolDirection = Vector2.right;
            }
            else if (target.position.x > transform.position.x)
            {
                Debug.Log("3");
                patrolDirection = Vector2.left;
            }

            BManim.SetBool("BmIdle", true);
            if (isFindPlayer == false)
            {
                yield return new WaitForSeconds(notFindDelayAnim);
            }
            
            hasReachedStartPosition = true;
        }
        if(isFindPlayer == false)
        {
            PatrolMovement(patrolSpeed, patrolDistance, patrolDirection, startPosition);       
        }

        yield return null;
    }

    public IEnumerator EnemyAtkStop()
    {
        isAtkStop = true;
        //StartCoroutine(baseDamaged());
        yield return null;
    }
}
