using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest 
{
    public QuestInfo info; //QuestInfo스크립트에 대한 내용 가져오기

    public QuestState state; //QuestState 가져옴

    private int currentQuestStepIndex; //비공개 엔드 생성

    public Quest(QuestInfo questInfo)
    {
        this.info = questInfo; //퀘스트 정보가져온것을 추가해주고
        this.state = QuestState.Requirements_Not_Met; //퀘스트 상태를 처음에는 안받은 상태
        this.currentQuestStepIndex = 0; // 0
    }

    //
    public bool CurrentStepExists()
    {
        return (currentQuestStepIndex < info.questStepPrefabs.Length);
    }

    //
    public void InstantiateCurrentQuestStep(Transform parentTransform)
    {
        GameObject questStepPrefab = GetCurrentQuestStepPrefab();
        if(questStepPrefab != null)
        {
            Object.Instantiate<GameObject>(questStepPrefab, parentTransform);
        }
    }

    //현재 단계의 퀘스트가 존재하지는 확인하고 아니면 다시 돌아가는 메서드
    private GameObject GetCurrentQuestStepPrefab()
    {
        GameObject questStepPrefab = null; //퀘스트 프리팹을 null로

        if(CurrentStepExists()) //CurrentStepExists메서드를 이용하여 현재 단계가 존재하는지 확인
        {
            questStepPrefab = info.questStepPrefabs[currentQuestStepIndex]; //ScriptableObject에서 조립식을 가져온다.

        }
        else
        {
            Debug.LogWarning("Quest 단계 프리팹을 가져오려고 시도했지만 stepIndex가 범위를 벗어났습니다. 이는" + "현재 단계가 없음을 나타냅니다." +
                " QuestID = " + info.id + ", stepIndex= " + currentQuestStepIndex);
        }
        return questStepPrefab;
    }
}
