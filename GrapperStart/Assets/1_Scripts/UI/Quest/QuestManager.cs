using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public int questId;
    Dictionary<int, QuestData> questList;
    public int questActionIndex; //����Ʈ npc��ȭ ����
    public Text QuestUI;
    public GameObject[] questObject;

    private void Awake()
    {
        questList = new Dictionary<int, QuestData>();
        GenerateData();
    }

    void GenerateData() //������ ����
    {
        //������ �̿� (string name, int[] npcid)
        questList.Add(10, new QuestData("�������� ���ض�", new int[] { 1000, 2000 }));
        questList.Add(20, new QuestData("������ ã��", new int[] { 2000, 300 })); //������� ����Ʈ ����
        questList.Add(30, new QuestData("����Ʈ ��� Ŭ����!", new int[] { 0 }));
    }

    public int GetQuestTalkIndex(int id) // Npc Id�� �޾� ����Ʈ ��ȣ�� ��ȯ�ϴ� �Լ� 
    {
        return questId + questActionIndex;
    }

    public string CheckQuest() //�����ε� 
    {
        //return Quest Name
        return questList[questId].questName;
    }


    public string CheckQuest(int id) //npc id
    {

        //Next Talk Target //�Ű������� ���� id�� 
        if (id == questList[questId].NpcId[questActionIndex])
        {
            questActionIndex++;
        }

        //����Ʈ UI Ű��
        QuestUI.gameObject.SetActive(true);

        //Control Quest Object
        ControlObject();
        //����Ʈ UI
        QuestUI.text = questList[questId].questName;
        //Talk complete & Next Quest //����Ʈ ����Ʈ�� �ִ� NpcId(����Ʈ�� �����ϴ� )
        if (questActionIndex == questList[questId].NpcId.Length)
        {
            NextQuest();
            QuestUI.text = questList[questId].questName;
        }


        //return Quest Name
        return questList[questId].questName;
    }



    void NextQuest()
    {
        questId += 10;
        questActionIndex = 0;

    }

    void ControlObject()
    {
        switch (questId)
        {
            case 10:
                if (questActionIndex == 2)
                {
                    //10�� ����Ʈ�� ����ġ�� ���� ����    

                    questObject[0].SetActive(true);
                    QuestUI.gameObject.SetActive(false);
                }

                break;

            case 20:
                if (questActionIndex == 1)
                {

                    //20�� ����Ʈ���� 1��° ������ ������ -> ���踦 ������ ���� ����� 
                    //QuestUI.text = questList[questId].questName;
                    questObject[0].SetActive(false);
                }
                break;
        }
    }
}
