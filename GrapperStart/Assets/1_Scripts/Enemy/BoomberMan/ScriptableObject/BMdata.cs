using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BM Data")]
public class BMdata : ScriptableObject
{
    public float bmHp = 10.0f;
    public void DamagedHp(float amount)
    {
        bmHp -= amount;
    }
}
