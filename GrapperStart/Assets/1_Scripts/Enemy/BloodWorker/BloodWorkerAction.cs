using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BloodState
{
    STATE_PATROL,
    STATE_ROCKATTACK,
    STATE_STOP,
    STATE_RENCHATTACK,
    
}
public class BloodWorkerAction : MonoBehaviour
{
    public BloodState bloodState;
    private BloodWorkerState myState;
    private BloodWorkerAttackReady myAttackState;

    Animator bloodWorkerAnim;
    CapsuleCollider2D capsuleColl;



    public bool isWall;
    public float patrolSpeed = 2.0f;
    public float patrolDistance = 10.0f; // 순찰 거리

    public Vector2 patrolDirection;
    protected Vector2 startPosition;

    public Transform patrolPos;
    public Vector2 patrolBoxSize;

    public bool isReact;
    public Sprite reactSprite;

    void Start()
    {
        capsuleColl = GetComponent<CapsuleCollider2D>();
        bloodWorkerAnim = GetComponent<Animator>();



        //기본적으로 시작하면 순찰 상태로 시작.
        this.bloodState = BloodState.STATE_PATROL; 
        setActionType(bloodState);
        myState = gameObject.AddComponent<BloodWorkerPatrol>();


        //순찰 기능 초기값 설정.
        startPosition = transform.position;
        patrolDirection = Vector2.right;

      
        
    }

    public GameObject rockPref;
    public Transform rockPos;
    public void setActionType(BloodState _bloodState)
    {
        Component bwState = gameObject.GetComponent<BloodWorkerState>();
        Component bwAttackReady = gameObject.GetComponent<BloodWorkerAttackReady>();
 
        if (bwState != null)
        {
            Destroy(bwState);
        }
        else if(bwAttackReady != null)
        {
            Destroy(bwAttackReady);
        }

        switch (_bloodState)
        {
            case BloodState.STATE_PATROL:
                if(myState == null)
                {
                    myState = gameObject.AddComponent<BloodWorkerPatrol>();
                    myState.Patrol(_bloodState, patrolSpeed, patrolDistance, patrolDirection, startPosition);
                }
              
                break;
            case BloodState.STATE_STOP:
                myState = gameObject.AddComponent<BloodWorkerPatrol>();
                myState.Stop(_bloodState);
                StartCoroutine(PatTrolTurn()); 
                break;
            case BloodState.STATE_ROCKATTACK:
                if (myAttackState == null)
                {
                    myAttackState = gameObject.AddComponent<BloodWorkerAttack>();
                    myAttackState.InstanRock(bloodState, rockPref, rockPos);
                    Debug.Log("돌멩이 함수 호출aa");
                    isTargetPlayer = false;
                    this.bloodState = BloodState.STATE_RENCHATTACK;
                }
                
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
                PatrolRange();
                break;
            case BloodState.STATE_ROCKATTACK:
                setActionType(BloodState.STATE_ROCKATTACK);
                break;
        }
    }

    public bool isTargetPlayer;
    void PatrolRange()
    {
        isReact = false;
        isTargetPlayer = false;

        Transform thirdChild = transform.GetChild(3);
        SpriteRenderer spriteRenderer = thirdChild.GetComponent<SpriteRenderer>();
        Collider2D[] colliders = Physics2D.OverlapBoxAll(patrolPos.position, patrolBoxSize, 0);

        foreach (Collider2D collider in colliders) //순찰 범위 
        {
            if (collider.CompareTag("Player"))
            {
                isReact = true;
                isTargetPlayer = true;
            }
        }
        if (isTargetPlayer)
        {
            Transform handChild = transform.GetChild(5);
            handChild.gameObject.SetActive(true);
            bloodWorkerAnim.SetTrigger("RockAttack");
       
            this.bloodState = BloodState.STATE_ROCKATTACK;
           
        }
        else if(isTargetPlayer == false )
        {
            Transform handChild = transform.GetChild(5);
            handChild.gameObject.SetActive(false);
            //렌치 공격 상태 저장 해야함.
        }
        PatrolReaction(spriteRenderer);
    }

    
    void PatrolReaction(SpriteRenderer spriteRenderer )
    {
       if(isReact)
        {
            spriteRenderer.sprite = reactSprite;
          
        }
       else if(isReact == false)
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
    }
}
