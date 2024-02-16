using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private Dictionary<string, Quest> questMap; //����Ʈ�� �����ϴ� ����

    private void Awake()
    {
        questMap = CreateQuestMap();

        //Quest quest = GetQuestById("���� ����Ʈ �̸�");
        //Debug.Log(quest.info.displayName);
       // Debug.Log(quest.info.levelRequirement);
        //Debug.Log(quest.info.state);
        //Debug.Log(quest.info.currentStepExists());
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
