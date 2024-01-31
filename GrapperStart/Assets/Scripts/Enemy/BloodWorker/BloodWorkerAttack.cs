using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodWorkerAttack : BloodWorkerState 
{
    public override void Initialize(BloodState state)
    {
        // BloodWorkerAttack에서만 필요한 초기화 로직을 구현
    }

    protected override void PatrolMovement(BloodState state, float patrolSpeed, float patrolDis, Vector2 patrolDir, Vector2 starPos)
    {




    }

    protected override IEnumerator PatrolStop(BloodState state)
    {
        yield return null;
    }



    public override void SlingAttack(BloodState state,GameObject rockPref,Transform rockPos)
    {
        Debug.Log("돌멩이 생성");
        Vector3 rockVec = rockPos.position;
        Instantiate(rockPref, rockVec, Quaternion.identity);
    }
}
