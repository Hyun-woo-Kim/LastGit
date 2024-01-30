using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BloodWorkerState : MonoBehaviour
{
  

    public abstract void Initialize(BloodState state);
    
    public virtual void Patrol(BloodState state)
    {
        PatrolMovement(state);
    }

    protected abstract void PatrolMovement(BloodState state);

}
