using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestManager : MonoBehaviour
{
    private static QuestManager instance;
    //퀘스트 매니저에 대한 인스턴스를 반환해 주는 프로퍼티
    public static QuestManager Instance => instance;

    public QuestDatabaseObject questDatabase;

    //퀘스트가 완료되었다는걸 알려준다.
    public event Action<QuestObject> OnCompletedQuest;

    private void Awake()
    {
        instance = this; //싱글톤 초기화
    }

    //킬관련 퀘스트 타입에 어떤 적을 처치, 아이템 획득했는데 어떤 아이템인지
    public void ProcessQuest(QuestType type, int targetId)
    {

        foreach (QuestObject questObject in questDatabase.questObjects)
        {
            //3가지 모두가 충족하면 내가 퀘스트를 진행중이라는 것을 확인
            if (questObject.status == QuestStatus.Accepted
                && questObject.data.type == type && questObject.data.targetID == targetId)
            {
                questObject.data.completedCount++;

                if(questObject.data.completedCount >= questObject.data.count)
                {
                    questObject.status = QuestStatus.Completed;
                    OnCompletedQuest?.Invoke(questObject); //어떠한 퀘스트가 완료되었다라는뜻
                }
            }
        }
    }



}
