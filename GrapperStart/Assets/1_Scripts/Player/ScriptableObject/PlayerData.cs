using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Player Data")]
public class PlayerData : ScriptableObject
{
    public int playerHp = 10;
    public void DamagedHp(int amount)
    {
        playerHp -= amount;
    }
}
