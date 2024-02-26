using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BW Data")]
public class BwData : ScriptableObject
{

    public float bwHp = 10.0f;
    public void DamagedHp(float amount)
    {
        bwHp -= amount;
    }
}
