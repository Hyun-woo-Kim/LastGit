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

    


    

    void Start()
    {
        this.bloodState = BloodState.STATE_PATROL; //기본적으로 시작하면 순찰 상태로 시작.

        setActionType(bloodState);

        myState = gameObject.AddComponent<BloodWorkerPatrol>();
        myState.Initialize(bloodState);

        //순찰 기능 초기값 설정.
        startPosition = transform.position;
        patrolDirection = Vector2.right;

      
        
    }


    public float patrolSpeed = 2.0f;
    public float patrolDistance = 10.0f; // 순찰 거리

    public Vector2 patrolDirection;
    protected Vector2 startPosition;
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

        }
    }
    IEnumerator PatTrolTurn()
    {
        yield return new WaitForSeconds(2.0f);
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
                PatrolReaction(); //PATROL상태일 때는 반응 기능을 추가한다.
                if (Mathf.Abs(transform.position.x - startPosition.x) >= patrolDistance)
                {
                    this.bloodState = BloodState.STATE_STOP;
                    setActionType(BloodState.STATE_STOP);
                }            
                setActionType(BloodState.STATE_PATROL);
                break;

        }
    }

    public GameObject ReactionPref;

    public Transform patrolPos;
    public Vector2 patrolBoxSize;

    public bool isReact;
    public Sprite reactSprite;
    void PatrolReaction()
    {
        Transform thirdChild = transform.GetChild(3);
        SpriteRenderer spriteRenderer = thirdChild.GetComponent<SpriteRenderer>();

        Collider2D[] colliders = Physics2D.OverlapBoxAll(patrolPos.position, patrolBoxSize, 0);

        isReact = false;
        spriteRenderer.sprite = null;


        foreach (Collider2D collider in colliders)
        {
            if(collider.CompareTag("Player"))
            {
                isReact = true;
                spriteRenderer.sprite = reactSprite;
            }
        }
    }

    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(patrolPos.position, patrolBoxSize);//DrawWireCube(pos.position,boxsize)      
    }
}
