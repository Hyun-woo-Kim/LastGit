using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    void Update()
    {
        
            FindedPlayer();
        
        
    }
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
    public IEnumerator baseDamaged()
    {
        chaneAttackMon = true;
        if (bmdata.bmHp >= 0)
        {
            bmdata.DamagedHp(DamagedValue);
            isMove = false;
            if (isStand)
            {
                BManim.SetBool("BmAtk", false);
                BManim.SetFloat("BmAnimCount", 0.0f);

                yield return new WaitForSeconds(0.1f);
                //BManim.SetBool("BmAtk", false);
              
                isMove = true;
            }
            isAtkStop = false;
        }

        else
        {
            StartCoroutine(Died());
        }

     
      
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
        FlipEnemy(target.transform);
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
                if (!isPlayerFindCoroutineRunning && isFindPlayer && chaneAttackMon) // 코루틴이 실행 중이 아닌 경우에만 실행
                {
                    StartCoroutine(playerFind());
                }
                if(isStand)
                {
                    PunchCollider();
                }
            }
        }
        if(isFindPlayer && isStand  && isPunch == false)
        {
            isMove = true;
            isPlayerMissing = false;
            BmMove();
        }
        else if(isFindPlayer == false && isStand)
        {
            isMove = false;
            isPlayerMissing = true;
            StartCoroutine(NotFindPlayer(spriteRenderer));
            //필드 순회
        }
        PatrolReaction(spriteRenderer);
    }
 

    

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
            transform.localScale = new Vector3(1, 1, 1);
            transform.Translate(patrolDir * patrolSpeed * Time.deltaTime);
        }



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


    void TeamEnemy()
    {
        //GameObject[] allies = GameObject.FindGameObjectsWithTag("Enemy");
        //foreach (GameObject ally in allies)
        //{
        //    // 아군 스크립트가 부착되어 있는지 확인
        //    BloodWorkerAction allyScript = ally.GetComponent<BloodWorkerAction>();
        //    if (allyScript != null)
        //    {
        //        // 아군 스크립트가 적대적인 상태인지 확인
        //        if (allyScript.isBasicDamaged || allyScript.isGraplingDamaged)
        //        {
        //            isTeamDamage = true;
        //            StartCoroutine(Find());
        //            Debug.Log("아군이 공격 받음");
        //            // 아군이 공격을 받았을 때의 처리 수행
        //        }
        //        else if(!allyScript.isBasicDamaged || !allyScript.isGraplingDamaged)
        //        {
        //            //isFindEnemy = false;
        //            Debug.Log("아군이 공격 받지 않음");
        //        }
        //    }
        //    else
        //    {

        //    }
        //}
    }
   
    void BmMove()
    {
         
            FlipEnemy(target);
            transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime * followSpeed);
            if (!BManim.GetBool("BmAtk"))
            {
                 BManim.SetBool("BmIdle", false);
                 BManim.SetFloat("BmAnimCount", 1.0f);
            }
        

    }

    private Dictionary<Transform, Coroutine> bmAttackCroutine = new Dictionary<Transform, Coroutine>();
    void PunchCollider()
    {
        Transform bmHand = transform.GetChild(3);
        isPunch = false;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(Punchboxpos.transform.position, PunchboxSize, 0);
        foreach (Collider2D collider in colliders)
        {
            if(collider.CompareTag("Player"))
            {
                if(isStand)
                {
                    isPunch = true;
                    
                }

                if (!bmAttackCroutine.ContainsKey(target))
                {
                    if(isHandAttack == false)
                    {
                        Coroutine coroutine = StartCoroutine(delayAttack(target, puhchAttackDelay)); //코루틴 호출과 동시에 아웃라인 보여줌
                        bmAttackCroutine.Add(target, coroutine);
                    }
                 
                
                   
                }

            }
        }



        if (isPunch == false)
        {
            isMove = true;
            UpdateOutline(false);
            bmHand.gameObject.SetActive(false);
            //BmMove();
            BManim.SetBool("BmAtk", false); //공격 범위에 벗어났으므로 공격애니메이션 해제.
            //UpdateOutline(false);
        }
      
    }

    public bool isHandAttack;
    public float Outlinedelay;
    public float puhchAttackDelay;
    IEnumerator delayAttack(Transform playerTransform, float _attackDelay)
    {

        BManim.SetBool("BmIdle", true); // 아웃라인 보여준 상태에서 스탠딩 상태 유지
        UpdateOutline(true);
        yield return new WaitForSecondsRealtime(Outlinedelay); // 딜레이 걸고

        if (isAtkStop == false)
        {
            BManim.SetBool("BmIdle", false); // 이전 스탠딩 상태 해제.
            BManim.SetBool("BmAtk", true); // 공격 애니메이션1
            BManim.SetFloat("BmAtkCount", 1.0f); // 공격 애니메이션2
        }
        
        yield return new WaitForSeconds(_attackDelay); // 공격 애니메이션 발동 전에 추가된 딜레이
        UpdateOutline(false);

        if (bmAttackCroutine.ContainsKey(playerTransform)) // 다시 초기화.
        {
            
            bmAttackCroutine.Remove(playerTransform);
        }

        // 공격 애니메이션을 코루틴 밖에서 호출
     
       
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

        BManim.SetBool("BmIdle", false);
        BManim.SetFloat("BmAnimCount", 1.0f);
        PatrolMovement(patrolSpeed, patrolDistance, patrolDirection, startPosition);
        yield return null;
    }

    public IEnumerator EnemyAtkStop()
    {
        isAtkStop = true;
        StartCoroutine(baseDamaged());
        yield return null;
    }
}
