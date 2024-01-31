using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BloodState
{
    STATE_PATROL,
    STATE_ATTACK,
    STATE_STOP,
    
}
public class BloodWorkerAction : MonoBehaviour
{
    public BloodState bloodState;
    private BloodWorkerState myState;

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

        this.bloodState = BloodState.STATE_PATROL; //기본적으로 시작하면 순찰 상태로 시작.

        setActionType(bloodState);

        myState = gameObject.AddComponent<BloodWorkerPatrol>();
        myState.Initialize(bloodState);

        //순찰 기능 초기값 설정.
        startPosition = transform.position;
        patrolDirection = Vector2.right;

      
        
    }

    public GameObject rockPref;
    public Transform rockPos;
    public void setActionType(BloodState _bloodState)
    {
       

        Component c = gameObject.GetComponent<BloodWorkerState>();

        if (c != null)
        {
            Destroy(c);
        }

        switch (_bloodState)
        {
            case BloodState.STATE_PATROL:
                myState = gameObject.AddComponent<BloodWorkerPatrol>();     
                myState.Patrol(_bloodState,patrolSpeed,patrolDistance,patrolDirection,startPosition);
                break;
            case BloodState.STATE_STOP:
                myState = gameObject.AddComponent<BloodWorkerPatrol>();
                myState.Stop(_bloodState);
                StartCoroutine(PatTrolTurn()); 
                break;
            case BloodState.STATE_ATTACK:
                myState = gameObject.AddComponent<BloodWorkerPatrol>();
                myState.SlingAttack(_bloodState, rockPref, rockPos);
                break;

        }
    }
    IEnumerator PatTrolTurn()
    {
        bloodWorkerAnim.SetBool("EnemyStop", true);
           yield return new WaitForSeconds(2.0f);
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
                PatrolRange(); //순찰 상태에서 순찰 범위 기능을 추가한다. 
                if (Mathf.Abs(transform.position.x - startPosition.x) >= patrolDistance || isWall)                     
                {
                    this.bloodState = BloodState.STATE_STOP;
                    setActionType(BloodState.STATE_STOP);
                    isWall = false;
                }
                setActionType(BloodState.STATE_PATROL);

                break;
            case BloodState.STATE_STOP:
                PatrolRange();//정지상태에서도 순찰 범위 기능을 추가한다. 
                break;
            case BloodState.STATE_ATTACK:
                PatrolRange(); //순찰 범위가 있어야 공격상태를 확인 할 수 있다.         
                    setActionType(BloodState.STATE_ATTACK);
                    //슬링 애니메이션 추가
                
                
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
            this.bloodState = BloodState.STATE_ATTACK;
        }
        else if(isTargetPlayer == false)
        {
            this.bloodState = BloodState.STATE_PATROL;
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
            Debug.Log("턴");
            isWall = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(patrolPos.position, patrolBoxSize);//DrawWireCube(pos.position,boxsize)      
    }
}
