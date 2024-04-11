using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BloodState
{
    STATE_PATROL,
    STATE_ROCKATTACK,
    STATE_STOP,
    STATE_FOLLOW,
    STATE_DIE,
}


public class BloodWorkerAction : MonoBehaviour, Enemies
{
    public BloodState bloodState;
    private BloodWorkerState myState;
    private BloodWorkerAttackReady myAttackState;
    public BwData bwData;
    Animator bloodWorkerAnim;
    CapsuleCollider2D capsuleColl;
    SpriteRenderer bwSpr;
    Enemies enemies;



    public bool isWall;
    public float damagedDelay;
    public float attackDelay;


    [Header("##PaTrol")]
    public Vector2 patrolDirection;
    protected Vector2 startPosition;
    public float patrolSpeed = 2.0f;
    public float patrolDistance = 10.0f; // 순찰 거리
    [Header("##Attack")]
    public Transform patrolPos;
    public Vector2 patrolBoxSize;
    public Transform renchAttackPos;
    public Vector2 renchAttackSize;
    public bool isTargetPlayer;
    public Transform target;
    public float followSpeed;
    public GameObject rockPref;
    public Transform rockPos;
    [Header("##Bw Team Interaction")]
    public bool hasThrownRock;
    public bool isMyTeam;
    public bool isGraplingDamaged;
    public bool isBasicDamaged;
    [Header("##Is Reaction")]
    public bool isReact;
    public Sprite reactSprite;
    public float animAimSpeed; //전등 조준 시 애니메이션 속도 줄이는 코드.
    public float animSpeed; //전등 조준 시 애니메이션 속도 줄이는 코드.
    void Start()
    {
        capsuleColl = GetComponent<CapsuleCollider2D>();
        bloodWorkerAnim = GetComponent<Animator>();
        bwSpr = GetComponent<SpriteRenderer>();
        enemies = GetComponentInParent<Enemies>();


        //기본적으로 시작하면 순찰 상태로 시작.
        this.bloodState = BloodState.STATE_STOP;
        setActionType(bloodState);


        //순찰 기능 초기값 설정.
        startPosition = transform.position;
        patrolDirection = Vector2.left;

        animSpeed = bloodWorkerAnim.speed;

        isNotFindPlayer = false;

    }

    public void EnemySet()//플레이어가 전등 조준하지 않았을 때 초기값
    {
        patrolSpeed = 2.0f;
        bloodWorkerAnim.speed = animSpeed;
    }
    public void SpeedDown() //플레이어가 전등 조준시 속도 낮추는 함수
    {
        patrolSpeed = 0.5f;
        followSpeed = 0.1f;
        bloodWorkerAnim.speed = animAimSpeed;
    }

    public IEnumerator GraplingAtkDamaged(float damage) //BW가 그래플링 스킬에 맞았을 때 호출 하는 함수
    {
        // 데미지 처리 로직
        isGraplingDamaged = true;
        bwSpr.color = Color.red;
        bwData.DamagedHp(damage);
        bloodWorkerAnim.SetTrigger("BWKnockBack");
        yield return new WaitForSeconds(damagedDelay);
        bwSpr.color = Color.white;
        isGraplingDamaged = false;
    }
    public IEnumerator baseDamaged()  //BW가 플레이어로 부터 기본공격에 맞았을 때 호출 하는 함수
    {
        isBasicDamaged = true;
        bwSpr.color = Color.red;
        bwData.DamagedHp(1);
        bloodWorkerAnim.SetTrigger("BWKnockBack");
        yield return new WaitForSeconds(damagedDelay);
        bwSpr.color = Color.white;
        if (bwData.bwHp <= float.Epsilon)
        {
            this.bloodState = BloodState.STATE_DIE;
            bloodWorkerAnim.SetTrigger("BWDie");
            Debug.Log("1");
            StartCoroutine(Died());
        }
        isBasicDamaged = false;
    }

    public IEnumerator Died()
    {

        yield return new WaitForSeconds(0.5f);
        bloodWorkerAnim.enabled = false;
        Destroy(this.gameObject);

    }




    public void setActionType(BloodState _bloodState)
    {
    

        Transform renchChild = transform.GetChild(5);
   

        switch (_bloodState)
        {
            case BloodState.STATE_PATROL:
                if (myState == null)
                {
                    renchChild.gameObject.SetActive(false);
                    PatrolMovement(patrolSpeed, patrolDirection);
                   // myState.Patrol(_bloodState, patrolSpeed, patrolDistance, patrolDirection, startPosition);
                }
                break;
            case BloodState.STATE_STOP:
                //myState.Stop(_bloodState);
                renchChild.gameObject.SetActive(false);
                StartCoroutine(PatTrolTurn());
                break;
            case BloodState.STATE_ROCKATTACK:            
                  StartCoroutine(RockDelayMove());
                 isNotFindPlayer = false;
                break;
            case BloodState.STATE_FOLLOW: //추격상태일때
               
                if (isTargetPlayer) //순찰범위에 플레이어 있으면
                {
                    isNotFindPlayer = false;
                    FollowPlayer();
                }
                else//순찰범위에 플레이어 없으면
                {
                    isNotFindPlayer = true;
                    this.bloodState = BloodState.STATE_STOP;
                    setActionType(BloodState.STATE_STOP);
                }

                break;

        }
    }
    void PatrolMovement(float patrolSpeed, Vector2 patrolDir)
    {
       
        transform.Translate(patrolDir * patrolSpeed * Time.deltaTime);


        bloodWorkerAnim.SetBool("BWStop", false); //걷는 애니메이션 재생

    }
    public bool isRockDelayMoveRunning;
    public float RockCreateDelay;
    IEnumerator RockDelayMove()
    {
 

        if (isRockDelayMoveRunning)
        {
            yield break;
        }

       
        isRockDelayMoveRunning = true;
        isMove = false;
        FlipEnemy();
        InstanRock(rockPref, rockPos, attackDelay); //투사체 던지기.
        bloodWorkerAnim.SetBool("BWStop", true);
        yield return new WaitForSeconds(RockCreateDelay);
        isMove = true;
        bloodWorkerAnim.SetBool("BWStop", false);
        this.bloodState = BloodState.STATE_FOLLOW;

        // 코루틴이 종료되었으므로 플래그를 다시 false로 설정합니다.
        isRockDelayMoveRunning = false;
    }

    void Update()
    {

        Transform thirdChild = transform.GetChild(4);
        SpriteRenderer spriteRenderer = thirdChild.GetComponent<SpriteRenderer>();

        PatrolReaction(spriteRenderer);

        switch (bloodState)
        {
            case BloodState.STATE_PATROL:
                if ((Mathf.Abs(transform.position.x - startPosition.x) >= patrolDistance || isWall))
                {
                    this.bloodState = BloodState.STATE_STOP;
                    setActionType(BloodState.STATE_STOP);
                    isWall = false;
                }
                else
                {
                    PatrolRange();
                    setActionType(BloodState.STATE_PATROL); // 중복 호출 제거
                }
                break;
            case BloodState.STATE_STOP:
                //PatrolRange();
                break;
            case BloodState.STATE_ROCKATTACK:
                PatrolRange();
                setActionType(BloodState.STATE_ROCKATTACK);
                break;
            case BloodState.STATE_FOLLOW:
                PatrolRange();
                setActionType(BloodState.STATE_FOLLOW);
                break;
        }
    }
    public float bwStopDelay;
    Vector3 walllocalScale;


    //정지 후 순찰 상태 구현
    IEnumerator PatTrolTurn()
    {
        if (this.bloodState == BloodState.STATE_STOP) 
        {
            
            bloodWorkerAnim.SetBool("BWStop", true);
            yield return new WaitForSeconds(bwStopDelay); //정지 후 기다리는 시간.

            //patrolDirection *= -1;
            startPosition.x = transform.position.x;
            FlipEnemy();
            //walllocalScale = transform.localScale;
            //walllocalScale.x *= -1;
            //transform.localScale = walllocalScale;
            Debug.Log("플립");
            this.bloodState = BloodState.STATE_PATROL;
            setActionType(BloodState.STATE_PATROL);
        }
   

    }

    public void PlayerToDamaged() //인터페이스 Enemies를 통해 구현해야 할 메서드.
    {
        Debug.Log("플레이어 타격");
        PlayerControllerRope player = GameObject.Find("Player").GetComponent<PlayerControllerRope>();
        PlayerData playerData = player.playerData;
        playerData.DamagedHp(1);
    }

    void FollowPlayer()
    {
      
        //렌치들고 추격하는 애니메이션 필요.
        if(isMove)
        {
            FlipEnemy();
            transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime * followSpeed);
            bloodWorkerAnim.SetBool("BWStop", false);
        }
       
    }

    void PatrolRange()
    {
        isReact = false;
        isTargetPlayer = false; //적 감지 확인 x



        Collider2D[] colliders = Physics2D.OverlapBoxAll(patrolPos.position, patrolBoxSize, 0);


        foreach (Collider2D collider in colliders) //피의노동자의 시야.
        {
            if (collider.CompareTag("Player")) //적 감지 
            {

                target = collider.transform;
                isReact = true;
                isTargetPlayer = true; //적 감지 확인

                RenchAttack();


            }

        }
        if ((isTargetPlayer && bloodState == BloodState.STATE_PATROL))//손을 이용하여 돌멩이 던진다. 손 활성화, 돌멩이 투척 애니메이션 활성화, 돌멩이 투척 상태로 변경.
        {
            this.bloodState = BloodState.STATE_ROCKATTACK; //돌멩이 투척상태
            return;
        }


    }
    private Dictionary<GameObject, Coroutine> rockCoroutines = new Dictionary<GameObject, Coroutine>();

    public void InstanRock(GameObject rockPref, Transform rockPos, float _attackDelay)
    {
        if (rockCoroutines.Count == 0) // 이미 돌이 생성되어 있는지 확인
        {

            // 돌 생성
           
            Vector3 rockVec = rockPos.position;
            bloodWorkerAnim.SetTrigger("BWRockAttack");
            GameObject rockInstance = Instantiate(rockPref, rockVec, Quaternion.identity);

            // 코루틴 시작
            Coroutine coroutine = StartCoroutine(RockDestroyCoroutine(rockInstance, _attackDelay));

            // 딕셔너리에 돌과 코루틴 추가
            rockCoroutines.Add(rockInstance, coroutine);
        }
    }

    private IEnumerator RockDestroyCoroutine(GameObject rockInstance, float _attackDelay)
    {
        // 지정된 시간이 지나면 돌 제거
        yield return new WaitForSeconds(_attackDelay);
        Destroy(rockInstance);

        // 딕셔너리에서 해당 돌의 코루틴 제거
        if (rockCoroutines.ContainsKey(rockInstance))
        {
            rockCoroutines.Remove(rockInstance);
        }
    }

    private Dictionary<Transform, Coroutine> playerAttackCoroutines = new Dictionary<Transform, Coroutine>();
    public bool isMove;

    public void RenchAttack()
    {
        Collider2D[] coliderRench = Physics2D.OverlapBoxAll(renchAttackPos.position, renchAttackSize, 0);
        isMove = true;

        foreach (Collider2D renchCollider in coliderRench)
        {
            if (renchCollider.CompareTag("Player"))
            {
               
                FlipEnemy();
                isMove = false;
                // 해당 플레이어의 코루틴을 시작
                if (!playerAttackCoroutines.ContainsKey(target))
                {
                    Coroutine coroutine = StartCoroutine(AttackAnimDelayCoroutine(target, attackDelay));
                    playerAttackCoroutines.Add(target, coroutine);
                }
            }
        }
    }

    IEnumerator AttackAnimDelayCoroutine(Transform playerTransform, float _attackDelay)
    {
        Transform renchChild = transform.GetChild(5);
        renchChild.gameObject.SetActive(true);
        bloodWorkerAnim.SetTrigger("BWRenchAtk");
        bloodWorkerAnim.SetBool("BWStop",true);
        yield return new WaitForSeconds(_attackDelay);
        renchChild.gameObject.SetActive(false);
        // 해당 플레이어의 코루틴을 제거
        if (playerAttackCoroutines.ContainsKey(playerTransform))
        {
            playerAttackCoroutines.Remove(playerTransform);
        }
    }
    void FlipEnemy()
    {
       if(this.bloodState == BloodState.STATE_FOLLOW || this.bloodState == BloodState.STATE_ROCKATTACK)
        {
            if (transform.position.x > target.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);

            }
            else if (transform.position.x < target.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);

            }
        }
       else if(this.bloodState == BloodState.STATE_PATROL || this.bloodState == BloodState.STATE_STOP)
        {
            patrolDirection *= -1;

            Debug.Log("출력");
            if(patrolDirection == Vector2.right)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (patrolDirection == Vector2.left)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
       
    }
    void PatrolReaction(SpriteRenderer spriteRenderer)
    {
        if (isReact && isNotFindPlayer == false)
        {
            spriteRenderer.sprite = reactSprite;

        }
        else if(isReact == false && isNotFindPlayer == true)
        {
            spriteRenderer.sprite = missSprite;
        }
        else if (isReact == false && isNotFindPlayer == false)
        {
            spriteRenderer.sprite = null;

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == ("Wall"))
        {
            isWall = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == ("Wall"))
        {
            isWall = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(patrolPos.position, patrolBoxSize);//DrawWireCube(pos.position,boxsize)      
        Gizmos.DrawWireCube(renchAttackPos.position, renchAttackSize);//DrawWireCube(pos.position,boxsize)      
    }

    public Color color = Color.white;

    [Range(0, 16)]
    public int outlineSize = 1;

    public void UpdateOutline(bool outline)
    {
        Debug.Log("노란선");
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        bwSpr.GetPropertyBlock(mpb);
        mpb.SetFloat("_Outline", outline ? 1f : 0);
        mpb.SetColor("_OutlineColor", color);
        mpb.SetFloat("_OutlineSize", outlineSize);
        bwSpr.SetPropertyBlock(mpb);
    }

    public Sprite missSprite;
    public bool isNotFindPlayer;

    public IEnumerator NotFindPlayer(SpriteRenderer sprite)
    {
       
        yield return null;

    }
}