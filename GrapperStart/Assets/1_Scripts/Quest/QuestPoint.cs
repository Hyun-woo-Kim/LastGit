using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CircleCollider2D))]
public class QuestPoint : MonoBehaviour
{
    [Header("Quest")]

    [SerializeField]
    private QuestInfo questInfoForPoint; //퀘스트 정보


    private bool playerIsNear = false;

    private string questId; //퀘스트 아이디

    private QuestState currentQuestState; //퀘스트 상태



    private void Awake()
    {
        questId = questInfoForPoint.id; //여기 퀘스트 아이디와 퀘스트 정보에서 가져오는 id가 같아야함
    }

    private void OnEnable()
    {
        GameEventManager.instance.questEvents.onQuestStateChange += QuestStateChange;
        GameEventManager.instance.inputEvents
    }

    private void OnDisable()
    {
        GameEventManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
    }

    //퀘스트의 id가 이 퀘스트 포인트id와 일치하는지 확인
    private void QuestStateChange(Quest quest)
    {
        if(quest.info.id.Equals(questId))
        {
            currentQuestState = quest.state;
            Debug.Log("quest id : " + questId + " 업데이트 퀘스트 상태 : " + currentQuestState);
        }
    }


    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if(otherCollider.CompareTag("Player"))
        {
            playerIsNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D otherCollider)
    {
        if (otherCollider.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }
}
