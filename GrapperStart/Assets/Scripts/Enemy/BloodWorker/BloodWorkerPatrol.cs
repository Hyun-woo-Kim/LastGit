using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodWorkerPatrol : BloodWorkerState
{





  

    public override void Initialize(BloodState state)
    {

       

    }

    protected  override void PatrolMovement(BloodState state, float patrolSpeed, float patrolDis, Vector2 patrolDir, Vector2 starPos)
    {

        Debug.Log("Patrol startPosition " + starPos);
        Debug.Log("Patrol patrolDirection " + patrolDir);
        
        //�̵� �ִϸ��̼� �߰�. 

        if (patrolDir == Vector2.right)
        {       
            transform.Translate(Vector2.right * patrolSpeed * Time.deltaTime);
        }
        else if (patrolDir == Vector2.left )
        {
           
            transform.Translate(Vector2.left * patrolSpeed * Time.deltaTime);
        }

    

    }

    protected override IEnumerator PatrolStop(BloodState state)
    {
        state = BloodState.STATE_STOP;
        yield return new WaitForSeconds(0.1f);
        Debug.Log("���� �� ����");
    }

}
