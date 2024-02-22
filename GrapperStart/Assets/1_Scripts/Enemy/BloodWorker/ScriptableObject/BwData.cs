using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BW Data")]
public class BwData : ScriptableObject
{

    public int bwHp = 10;
    public void DamagedHp(int amount)
    {
        bwHp -= amount;
    }
}
