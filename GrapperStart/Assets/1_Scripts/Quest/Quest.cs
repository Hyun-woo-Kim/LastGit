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
    public int targetID; //�� ID

    public int count; //���� ��� �׿��� �� �� ����ϴ� ����
    public int completedCount;

    public string title;
    public string description;
}
