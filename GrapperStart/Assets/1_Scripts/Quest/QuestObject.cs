using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestStatus
{
    None, //기본
    Accepted, //퀘스트 받음
    Completed, //퀘스트 성공
    Rewareded, //보상까지 받음
}


[CreateAssetMenu(fileName ="New Quest", menuName ="Quest System/Quests/New Quest")]
public class QuestObject : ScriptableObject
{
    public Quest data = new Quest();
    public QuestStatus status;
}
