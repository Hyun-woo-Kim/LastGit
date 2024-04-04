using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BloodWorkerAttackReady : MonoBehaviour
{
    public abstract IEnumerator InstanRock(BloodState state, GameObject rokcPref, Transform rockPos);
    //public abstract void InstanRock(BloodState state, GameObject rokcPref, Transform rockPos);

    public abstract void RenchAttack(BloodState state, Collider2D[] collider, Animator bwAnim,float attackDelay);



}
