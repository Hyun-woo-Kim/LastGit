using System.Collections;
using UnityEngine;

public enum BloodState
{
    STATE_PATROL,
    STATE_ROCKATTACK,
    STATE_STOP,
    STATE_FOLLOW,

}


public class BloodWorkerAction : MonoBehaviour, Enemies
{
    public BloodState bloodState;
    private BloodWorkerState myState;
    private BloodWorkerAttackReady myAttackState;
    public BwData bwData;
    Animator bloodWorkerAnim;
    CapsuleCollider2D capsuleColl;
    Enemies enemies;


    public bool isWall;


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
    public float followSpeed = 1.0f;
    public GameObject rockPref;
    public Transform rockPos;
    [Header("##BwInteraction")]
    public bool isTeamEnemy;
    public Transform playerTarget;
    [Header("##IsReaction")]
    public bool isReact;
    public Sprite reactSprite;

    public IEnumerator GraplingAtkDamaged()
    {
        // 데미지 처리 로직
        Debug.Log("데미지를 입음");
        bwData.bwHp--;
        yield return null;
    }

    void Start()
    {
        capsuleColl = GetComponent<CapsuleCollider2D>();
        bloodWorkerAnim = GetComponent<Animator>();
        enemies = GetComponentInParent<Enemies>();


        //기본적으로 시작하면 순찰 상태로 시작.
        this.bloodState = BloodState.STATE_PATROL;
        setActionType(bloodState);
        myState = gameObject.AddComponent<BloodWorkerPatrol>();


        //순찰 기능 초기값 설정.
        startPosition = transform.position;
        patrolDirection = Vector2.right;



    }


    public void setActionType(BloodState _bloodState)
    {
        Component bwState = gameObject.GetComponent<BloodWorkerState>();
        Component bwAttackReady = gameObject.GetComponent<BloodWorkerAttackReady>();
        Transform renchChild = transform.GetChild(4);
        Transform handChild = transform.GetChild(5);
        if (bwState != null)
        {
            Destroy(bwState);
        }
        else if (bwAttackReady != null)
        {
            Destroy(bwAttackReady);
        }

        switch (_bloodState)
        {
            case BloodState.STATE_PATROL:
                if (myState == null)
                {
                    myState = gameObject.AddComponent<BloodWorkerPatrol>();
                    handChild.gameObject.SetActive(false);
                    renchChild.gameObject.SetActive(false);
                    myState.Patrol(_bloodState, patrolSpeed, patrolDistance, patrolDirection, startPosition);
                }
                break;
            case BloodState.STATE_STOP:
                myState = gameObject.AddComponent<BloodWorkerPatrol>();
                myState.Stop(_bloodState);
                handChild.gameObject.SetActive(false);
                renchChild.gameObject.SetActive(false);
                StartCoroutine(PatTrolTurn());
                break;
            case BloodState.STATE_ROCKATTACK:
                if (myAttackState == null)
                {
                    myAttackState = gameObject.AddComponent<BloodWorkerAttack>();

                    handChild.gameObject.SetActive(true);

                    StartCoroutine(myAttackState.InstanRock(bloodState, rockPref, rockPos)); //투사체 던지기.        
                    this.bloodState = BloodState.STATE_FOLLOW;
                }
                break;
            case BloodState.STATE_FOLLOW:
                FollowPlayer();
                Collider2D[] coliderRench = Physics2D.OverlapBoxAll(renchAttackPos.position, renchAttackSize, 0);
                myAttackState = gameObject.AddComponent<BloodWorkerAttack>();

                renchChild.gameObject.SetActive(true);
                myAttackState.RenchAttack(bloodState, coliderRench, bloodWorkerAnim);
                break;

        }
    }

    void Update()
    {
        switch (bloodState)
        {
            case BloodState.STATE_PATROL:
                if (Mathf.Abs(transform.position.x - startPosition.x) >= patrolDistance || isWall)
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
                setActionType(BloodState.STATE_ROCKATTACK);
                break;
            case BloodState.STATE_FOLLOW:
                PatrolRange();
                setActionType(BloodState.STATE_FOLLOW);
                break;
        }
    }
    IEnumerator PatTrolTurn()
    {
        bloodWorkerAnim.SetBool("EnemyStop", true);
        yield return new WaitForSeconds(2.0f); //정지 후 기다리는 시간.
        bloodWorkerAnim.SetBool("EnemyStop", false);
        this.bloodState = BloodState.STATE_PATROL;


        patrolDirection *= -1;
        startPosition.x = transform.position.x;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;

        setActionType(BloodState.STATE_PATROL);
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
        FlipEnemy(target);
        //렌치들고 추격하는 애니메이션 필요.
        transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime * followSpeed);
    }

    public Transform teamEnemy;
    void PatrolRange()
    {
        isReact = false;
        isTargetPlayer = false; //적 감지 확인 x
        isTeamEnemy = false; //적 감지 확인 x

        Transform thirdChild = transform.GetChild(3);
        SpriteRenderer spriteRenderer = thirdChild.GetComponent<SpriteRenderer>();

        Collider2D[] colliders = Physics2D.OverlapBoxAll(patrolPos.position, patrolBoxSize, 0);


        foreach (Collider2D collider in colliders) //피의노동자의 시야.
        {
            if (collider.CompareTag("Player")) //적 감지 
            {
                target = collider.transform;
                isReact = true;
                isTargetPlayer = true; //적 감지 확인
            }

        }
        if (isTargetPlayer && bloodState == BloodState.STATE_PATROL)//손을 이용하여 돌멩이 던진다. 손 활성화, 돌멩이 투척 애니메이션 활성화, 돌멩이 투척 상태로 변경.
        {
            FlipEnemy(target);
            bloodWorkerAnim.SetTrigger("RockAttack");
            this.bloodState = BloodState.STATE_ROCKATTACK; //돌멩이 투척상태

        }
        else if (isTargetPlayer == false)
        {
            this.bloodState = BloodState.STATE_PATROL;
        }

        PatrolReaction(spriteRenderer);
    }



    void FlipEnemy(Transform target)
    {
        Debug.Log("방향뒤집기");
        if (transform.position.x > target.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (transform.position.x < target.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
    void PatrolReaction(SpriteRenderer spriteRenderer)
    {
        if (isReact)
        {
            spriteRenderer.sprite = reactSprite;

        }
        else if (isReact == false)
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


}
