using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BloodWorkerState : MonoBehaviour
{
  

    public virtual void Patrol(BloodState state, float patrolSpeed, float patrolDis, Vector2 patrolDir, Vector2 starPos)
    {
        PatrolMovement(state, patrolSpeed, patrolDis, patrolDir, starPos);
    }

    protected abstract void PatrolMovement(BloodState state,float patrolSpeed, float patrolDis,Vector2 patrolDir,Vector2 starPos);



   public virtual void Stop(BloodState state)
    {
        StartCoroutine(PatrolStop(state));
    }

    protected abstract IEnumerator PatrolStop(BloodState state);

   

}
