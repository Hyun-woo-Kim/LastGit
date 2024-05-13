using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodWorkerPatrol : BloodWorkerState
{


    protected  override void PatrolMovement(BloodState state, float patrolSpeed, float patrolDis, Vector2 patrolDir, Vector2 starPos)
    {

    
        
        //이동 애니메이션 추가. 

        //if (patrolDir == Vector2.right)
        //{
        //    transform.localScale = new Vector3(-1, 1, 1);
        //    transform.Translate(Vector2.right * patrolSpeed * Time.deltaTime);
        //}
        //else if (patrolDir == Vector2.left )
        //{
        //    transform.localScale = new Vector3(1, 1, 1);
        //    transform.Translate(Vector2.left * patrolSpeed * Time.deltaTime);
        //}

    

    }

    protected override IEnumerator PatrolStop(BloodState state)
    {
        yield return new WaitForSeconds(0.1f);
    }

}
