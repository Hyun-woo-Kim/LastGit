using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BM Data")]
public class BMdata : ScriptableObject
{
    public float bmHp;
    public void DamagedHp(float amount)
    {
        bmHp = bmHp -amount;   
    }
}
