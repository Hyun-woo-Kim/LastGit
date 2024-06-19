using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PP Data")]
public class PPData : ScriptableObject
{
    public float ppHP = 100.0f;
    public void DamagedHp(float amount)
    {
        ppHP -= amount;
    }
}
