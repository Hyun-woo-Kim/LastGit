using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestState 
{
    Requirements_Not_Met, //퀘스트를 안받은 상태

    Can_Start, //퀘스트를 받기만 한 상태

    In_Progress, //퀘스트를 진행중

    Can_Finsh, //퀘스트 끝낼 수 있는 상태

    Finished // 끝낸 상태 (보상 받기)
}
