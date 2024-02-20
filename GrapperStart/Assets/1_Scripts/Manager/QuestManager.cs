using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<string, Quest> questMap; //퀘스트에 매핑하는 사전

    private void Awake()
    {
        questMap = CreateQuestMap();
    }

    //활성화
    private void OnEnable()
    {
        GameEventManager.instance.questEvents.onStartQuest += StartQuest;
        GameEventManager.instance.questEvents.onAdvanceQuest += AdvanceQuest;
        GameEventManager.instance.questEvents.onFinishQuest += FinishQuest;
    }
    
    //비활성화
    private void OnDisable()
    {
        GameEventManager.instance.questEvents.onStartQuest -= StartQuest;
        GameEventManager.instance.questEvents.onAdvanceQuest -= AdvanceQuest;
        GameEventManager.instance.questEvents.onFinishQuest -= FinishQuest;
    }

    private void Start()
    {
        foreach(Quest quest in questMap.Values)
        {
            GameEventManager.instance.questEvents.QuestStateChange(quest);
        }
    }

    //시작한테 불러오는
    private void StartQuest(string id)
    {
        Debug.Log("start Quest" + id);
    }

    //진행중
    private void AdvanceQuest(string id)
    {
        Debug.Log("Advance Quest" + id);
    }

    //퀘스트 끝
    private void FinishQuest(string id)
    {
        Debug.Log("Finish Quest" + id);
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
