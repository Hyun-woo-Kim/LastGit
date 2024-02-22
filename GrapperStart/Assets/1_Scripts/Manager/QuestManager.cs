using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<string, Quest> questMap; //����Ʈ�� �����ϴ� ����

    private void Awake()
    {
        questMap = CreateQuestMap();
    }

    //Ȱ��ȭ
    private void OnEnable()
    {
        GameEventManager.instance.questEvents.onStartQuest += StartQuest;
        GameEventManager.instance.questEvents.onAdvanceQuest += AdvanceQuest;
        GameEventManager.instance.questEvents.onFinishQuest += FinishQuest;
    }
    
    //��Ȱ��ȭ
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

    //�������� �ҷ�����
    private void StartQuest(string id)
    {
        Debug.Log("start Quest" + id);
    }

    //������
    private void AdvanceQuest(string id)
    {
        Debug.Log("Advance Quest" + id);
    }

    //����Ʈ ��
    private void FinishQuest(string id)
    {
        Debug.Log("Finish Quest" + id);
    }

    private Dictionary<string, Quest> CreateQuestMap()
    {
        //�ڻ�/���ҽ�/����Ʈ ���� �Ʒ��� ��� QuestInfo ��ũ���� ������ ��ü�� �ε�
        QuestInfo[] allQuests = Resources.LoadAll<QuestInfo>("Quests");

        //����Ʈ���� �����
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
