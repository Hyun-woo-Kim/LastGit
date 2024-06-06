using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    public int questId;
    Dictionary<int, QuestData> questList;
    public int questActionIndex; //퀘스트 npc대화 순서
    public Text QuestUI;
    public GameObject[] questObject;

    private void Awake()
    {
        questList = new Dictionary<int, QuestData>();
        GenerateData();
    }

    void GenerateData() //데이터 생성
    {
        //생성자 이용 (string name, int[] npcid)
        questList.Add(10, new QuestData("누군가를 구해라", new int[] { 1000, 2000 }));
        questList.Add(20, new QuestData("아이템 찾기", new int[] { 2000, 300 })); //순서대로 퀘스트 진행
        questList.Add(30, new QuestData("퀘스트 모두 클리어!", new int[] { 0 }));
    }

    public int GetQuestTalkIndex(int id) // Npc Id를 받아 퀘스트 번호를 반환하는 함수 
    {
        return questId + questActionIndex;
    }

    public string CheckQuest() //오버로딩 
    {
        //return Quest Name
        return questList[questId].questName;
    }


    public string CheckQuest(int id) //npc id
    {

        //Next Talk Target //매개변수로 받은 id가 
        if (id == questList[questId].NpcId[questActionIndex])
        {
            questActionIndex++;
        }

        //퀘스트 UI 키기
        QuestUI.gameObject.SetActive(true);

        //Control Quest Object
        ControlObject();
        //퀘스트 UI
        QuestUI.text = questList[questId].questName;
        //Talk complete & Next Quest //퀘스트 리스트에 있는 NpcId(퀘스트에 참여하는 )
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
                    //10번 퀘스트를 끝마치면 열쇠 등장    

                    questObject[0].SetActive(true);
                    QuestUI.gameObject.SetActive(false);
                }

                break;

            case 20:
                if (questActionIndex == 1)
                {

                    //20번 퀘스트에서 1번째 순서가 끝나고 -> 열쇠를 먹으면 열쇠 사라짐 
                    //QuestUI.text = questList[questId].questName;
                    questObject[0].SetActive(false);
                }
                break;
        }
    }
}
