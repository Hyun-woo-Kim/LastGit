using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodWorkerAttack : BloodWorkerAttackReady
{

  

    public override void InstanRock(BloodState state, GameObject rockPref, Transform rockPos)
    {    
         Vector3 rockVec = rockPos.position;
         Instantiate(rockPref, rockVec, Quaternion.identity);
    }

    
}
