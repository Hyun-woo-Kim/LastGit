using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<string, Quest> questMap; //퀘스트에 매핑하는 사전

    private void Awake()
    {
        questMap = CreateQuestMap();

        //Quest quest = GetQuestById("게임 퀘스트 이름");
        //Debug.Log(quest.info.displayName);
       // Debug.Log(quest.info.levelRequirement);
        //Debug.Log(quest.info.state);
        //Debug.Log(quest.info.currentStepExists());
    }

    private Dictionary<string, Quest> CreateQuestMap()
    {
        //자산/리소스/퀘스트 폴더 아래의 모든 QuestInfo 스크립팅 가능한 개체를 로드
        QuestInfo[] allQuests = Resources.LoadAll<QuestInfo>("Quests");

        //퀘스트맵을 만든다
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach(QuestInfo questInfo in allQuests)
        {
            if(idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning("Duplicate ID found when creating quest map: " + questInfo.id);

            }
            idToQuestMap.Add(questInfo.id, new Quest(questInfo));
        }
        return idToQuestMap;
    }

    private Quest GetQuestById(string id)
    {
        Quest quest = questMap[id];
        if(quest == null)
        {
            Debug.LogError("ID not found in the Quest Map: " + id);
        }
        return quest;
    }
}
