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
    public float patrolDistance = 10.0f; // ���� �Ÿ�

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

        this.bloodState = BloodState.STATE_PATROL; //�⺻������ �����ϸ� ���� ���·� ����.

        setActionType(bloodState);

        myState = gameObject.AddComponent<BloodWorkerPatrol>();
        myState.Initialize(bloodState);

        //���� ��� �ʱⰪ ����.
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
                PatrolRange(); //���� ���¿��� ���� ���� ����� �߰��Ѵ�. 
                if (Mathf.Abs(transform.position.x - startPosition.x) >= patrolDistance || isWall)                     
                {
                    this.bloodState = BloodState.STATE_STOP;
                    setActionType(BloodState.STATE_STOP);
                    isWall = false;
                }
                setActionType(BloodState.STATE_PATROL);

                break;
            case BloodState.STATE_STOP:
                PatrolRange();//�������¿����� ���� ���� ����� �߰��Ѵ�. 
                break;
            case BloodState.STATE_ATTACK:
                PatrolRange(); //���� ������ �־�� ���ݻ��¸� Ȯ�� �� �� �ִ�.         
                    setActionType(BloodState.STATE_ATTACK);
                    //���� �ִϸ��̼� �߰�
                
                
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

        foreach (Collider2D collider in colliders) //���� ���� 
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
            Debug.Log("��");
            isWall = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(patrolPos.position, patrolBoxSize);//DrawWireCube(pos.position,boxsize)      
    }
}
