using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestType
{
    DestroyEnemy,
    AcquireItem,
}


[Serializable]
public class Quest 
{
    public int id;

    public QuestType type;
    public int targetID; //적 ID

    public int count; //적을 몇명 죽여라 할 때 사용하는 변수
    public int completedCount;

    public string title;
    public string description;
}
