using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR;
using static System.Net.Mime.MediaTypeNames;

public class BManAction : MonoBehaviour,Enemies
{
    Animator BManim;
    CapsuleCollider2D collder;
    protected CircleCollider2D childCollider;
    SpriteRenderer bmSpr;
    Rigidbody2D Bmrigid;

    public BMdata bmdata;


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
    private bool isMove;
    private bool isPlayerMissing = false; //플레이어를 놓쳤을 때 bool
    private bool isDied; //죽었을 때
    public float animAimSpeed; //플레이어가 전등을 조준했을 때 애니메이션 속도를 느려지게하는 변수
    public float animSpeed; //원래 애니메이션 속도로 바뀌는 변수



    [Header("##Reaction")]
    private bool isReact;
    public Sprite reactSprite;//플레이어를 찾았을 때 스프라이트
    public Sprite missSprite; //플레이어를 놓쳤을 때 물음표 스프라이트.
    [Header("##Attack")]
    public float nockbackForce; //죽었을 때 플레이어게 넉백 힘 얼마나 줄지
    private bool hasAttacked; //공격했을 때 true
    private bool isPlayerFindCoroutineRunning = false;
    public Transform Punchboxpos;
    public Vector2 PunchboxSize;
    private bool isColliderPlayer;
    private bool isPunch;
    private bool isAttack;
    public float DamagedValue; //플레이어에게 얼마나 데미지를 입을지
    public float basicDamage; //잽 공격 데미지
    public float powerDamage; //강화 공격 데미지
    public float maxAttackDistance = 3.0f; //공격할 때 광선 길이를 말함. 광선 길이 이내에 있어야 공격조건1 만족됨
    public float RepeatCount; //연타공격 변수 
    private bool isAttacking;
    public bool isAtk;
    public Color OutLineEnemycolor = Color.yellow; //아웃라인 기본컬러 
    [Range(0, 16)]
    public int outlineSize; //아웃라인 두께조절
    [Header("##Patrol")]
    public Vector2 patrolDirection; //순찰 방향
    public Vector2 startPosition; //순찰 했을 때 처음 위치
    public float patrolSpeed = 2.0f; //순찰 속도
    public float patrolDistance = 5.0f; // 순찰 거리
    private bool hasReachedStartPosition;
    public LayerMask wallLayer; //벽 레이어.
    private bool isWall; // 벽 충돌시 true
    public float WallrayDistance = 5f; // 벽을 감지할 길이.
    [Header("##Damaged or Dead")]
    public Vector3 lastPlayerPos;
    public GameObject DiedPrefab;
    private bool chaneAttackMon;
    Vector2 InitCollSize;
    private bool isDamaged;
    [Header("##Delay")]
    public float nockbackDelay; //넉백 딜레이
    public float boomMoveDelay; //자폭돌진 딜레이
    public float bmStopDel; //폭발 이펙트 보여주고 bm이 몇초동안 일시정지 시킬건지
    public float bmDestoryDel; //폭발 이펙트 보여주고 bm이 몇초동안 일시정지 시킬건지
    public float AttackToIdleDel; //공격 전 Idle상태 딜레이
    private bool isAtkStop;
   
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
    void Update()
    {
        if (!isPlayerFindCoroutineRunning && isFindPlayer && chaneAttackMon) // 코루틴이 실행 중이 아닌 경우에만 실행
        {
            StartCoroutine(playerFind());
        }

        if(isDied == false) //죽은상태가 아닐 때만
        {
            FindedPlayer(); 
        }
        

        if (hasReachedStartPosition == true)
        {
            Debug.Log("이동");
            PatrolMovement(patrolSpeed, patrolDistance, patrolDirection, startPosition);
        }
    }
    // Update is called once per frame
    public IEnumerator GraplingAtkDamaged()
    {
        StartCoroutine(baseDamaged());

        yield return null;
    }

    
 
  
   

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
                yield return new WaitForSeconds(nockbackDelay);
                BManim.SetTrigger("BmNockBack");
                yield return new WaitForSeconds(0.5f);
                BManim.SetTrigger("BmNockBackToIdle");

                isDamaged = false;
                isMove = true;
                
            }

            else if(bmdata.bmHp <= 0)
            {
                lastPlayerPos = target.transform.position;
                isDied = true;
                StartCoroutine(Died());
            }

        }

        isAtkStop = false;
       
        yield return null;

    }

    public IEnumerator Died()
    {
      
        isFindPlayer = false;
        isMove = false;
        isStand = false;
        isReact = false;
       
        FlipEnemy(target);
        BManim.SetTrigger("BmNockBack");
        yield return new WaitForSeconds(boomMoveDelay);
        BManim.SetTrigger("BmBoomMove");
        float elapsedTime = 0f;
        Vector2 initialPosition = transform.position;

        while (!isColliderPlayer && elapsedTime < 1.0f)
        {
            transform.position = Vector2.Lerp(initialPosition, lastPlayerPos, elapsedTime);
            elapsedTime += Time.deltaTime * moveSpeed;
         
            yield return null;
        }
        if (isColliderPlayer) //자폭돌진 했는데 플레이어와 충돌 했을 경우.
        {
            PlayerControllerRope playerController = FindAnyObjectByType<PlayerControllerRope>();
            if (playerController != null)
            {
                StartCoroutine(playerController.BMSkillMove(transform.position, nockbackForce)); // 적의 위치를 전달
            }

        }
        Transform fourChild = transform.GetChild(5);
        fourChild.gameObject.SetActive(false);
        UpdateOutline(false);
        BManim.SetTrigger("BmNockBack");
        GameObject diePrf = Instantiate(DiedPrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(bmStopDel);
        Destroy(diePrf);
        yield return new WaitForSeconds(bmDestoryDel);
        DestroyBM();

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

   

    void FindedPlayer()
    {
        Debug.Log("aa");
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
            Debug.Log("순찰함수2");
            //필드 순회
        }

        if(isFindPlayer == true && chaneAttackMon == true && isStand )
        {
            Debug.Log("공격함수2");
            HandAttack();
        }       
        PatrolReaction(spriteRenderer);
    }

    bool hasFlipped = false;


    void PatrolMovement(float patrolSpeed, float patrolDis, Vector2 patrolDir, Vector2 starPos)
    {

        bool hasChangedDirection = false;

        WallTurn(); //순찰 시에만 벽을 감지

        if (!hasChangedDirection && Mathf.Abs(transform.position.x - startPosition.x) >= patrolDis && isWall)
        {
            Debug.Log("방향전환");
            patrolDirection *= -1;
            startPosition = transform.position;
            hasChangedDirection = true;
            isWall = false;
        }

        if (patrolDir == Vector2.right)
        {
            Debug.Log("이동1");
            transform.localScale = new Vector3(-1, 1, 1);
            transform.Translate(patrolDir * patrolSpeed * Time.deltaTime);
        }
        else if (patrolDir == Vector2.left)
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.Translate(patrolDir * patrolSpeed * Time.deltaTime);
        }

        BManim.SetBool("BmIdle", false);
        BManim.SetFloat("BmAnimCount", 1.0f);

    }
  
    public void WallTurn()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, patrolDirection, Mathf.Infinity, wallLayer);
        Debug.DrawRay(transform.position, patrolDirection * WallrayDistance, Color.red);
        // Raycast가 벽과 충돌한 경우
        if (hit.collider != null)
        {
            float distance = Mathf.Abs(hit.point.x - transform.position.x);

            if (distance < WallrayDistance)
            {
                isWall = true;
            }
        }

    }
    IEnumerator playerFind()
    {
        gameObject.layer = 9; // Enemy레이어 변경
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

            transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime * followSpeed);
        }

     
    }

   
    void HandAttack()
    {
        isAtk = false;
        UpdateOutline(false);
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
            RepeatCount = 0;
            BManim.SetBool("BmAtk", false);
            
        }

    }
    
    float DistanceToPlayer(Transform playerTransform, Transform enemyTransform)
    {
        return Vector3.Distance(playerTransform.position, enemyTransform.position);
    }


    
    IEnumerator PlayAttackAnimation()
    {
        UpdateOutline(true);
        BManim.SetBool("BmIdle", true); // 이전 스탠딩 상태.
        yield return new WaitForSeconds(AttackToIdleDel);
        BManim.SetBool("BmIdle", false); // 이전 스탠딩 상태 해제.

        if (isDamaged == false && isAttacking == true && isDied == false)
        {
            isMove = false;  
            BManim.SetBool("BmAtk", true); // 공격 애니메이션1
            BManim.SetFloat("BmAtkCount", 1.0f); // 공격 애니메이션2
            
            //UpdateOutline(false);

            isAttacking = false; // 공격 종료
        }
        

    }
    IEnumerator handColliderVisiable()
    {

        childCollider = GetComponentInChildren<CircleCollider2D>();
        // 자식 클래스의 콜라이더가 존재하면 비활성화
        if (childCollider != null)
        {
            childCollider.enabled = true;
            yield return new WaitForSeconds(0.1f);
            childCollider.enabled = false;
        }
    }

    public void handCollider() //애니메이션 1타 및 2타때 이벤트 형식으로 호출함.
    {
        StartCoroutine(handColliderVisiable());
    }

    public void RepeatCountAnim() //애니메이션 2타때만 이벤트 형식으로 호출.
    {
        RepeatCount += 1.0f;
        BoomBerManHand bmHand = FindAnyObjectByType<BoomBerManHand>();
        bmHand.test();
    }

    public void CountSet()
    {
        RepeatCount = 0.0f;
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
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (transform.position.x < _target.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    void PatrolReaction(SpriteRenderer spriteRenderer)
    {   
      
            if (isReact && isStand)
            {
                spriteRenderer.sprite = reactSprite;
            }
            else if (isReact == false || isDied == true)
            {
                spriteRenderer.sprite = null;

            }
        
     
    }
    
    void DestroyBM()
    {
        Destroy(this.gameObject);
    }



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
        BManim.SetBool("BmAtk", false);

        if (!hasReachedStartPosition)
        {
            startPosition = transform.position;
            //Debug.Log("1");
            if(target != null)
            {
                if (target.position.x < transform.position.x)
                {
                    //Debug.Log("2");
                    patrolDirection = Vector2.right;
                }
                else if (target.position.x > transform.position.x)
                {
                    //Debug.Log("3");
                    patrolDirection = Vector2.left;
                }

                BManim.SetBool("BmIdle", true);

                yield return new WaitForSeconds(notFindDelayAnim);

                hasReachedStartPosition = true;
            }
            else
            {
                isMove = false;
                BManim.SetBool("BmIdle", true);
            }
            
        }
        yield return null;
    }

    public IEnumerator EnemyAtkStop()
    {
        isAtkStop = true;
        //StartCoroutine(baseDamaged());
        yield return null;
    }

    public void PlayerToDamaged()
    {
        throw new System.NotImplementedException();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(Findboxpos.position, FindboxSize);//DrawWireCube(pos.position,boxsize)          
        Gizmos.DrawWireCube(Punchboxpos.position, PunchboxSize);//DrawWireCube(pos.position,boxsize)          
    }

}
