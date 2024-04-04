using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Player Data")]
public class PlayerData : ScriptableObject
{
    public float playerHp = 10;
    public float curSpeed = 8;

    public void DamagedHp(float amount)
    {
        playerHp -= amount;
    }
}
