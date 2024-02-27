using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CircleCollider2D))]
public class QuestPoint : MonoBehaviour
{
    [Header("Quest")]

    [SerializeField]
    private QuestInfo questInfoForPoint; //����Ʈ ����


    private bool playerIsNear = false;

    private string questId; //����Ʈ ���̵�

    private QuestState currentQuestState; //����Ʈ ����



    private void Awake()
    {
        questId = questInfoForPoint.id; //���� ����Ʈ ���̵�� ����Ʈ �������� �������� id�� ���ƾ���
    }

    private void OnEnable()
    {
        GameEventManager.instance.questEvents.onQuestStateChange += QuestStateChange;
        GameEventManager.instance.inputEvents.onSubmitPressed += SubmitPressed;
    }

    private void OnDisable()
    {
        GameEventManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
        GameEventManager.instance.inputEvents.onSubmitPressed -= SubmitPressed;
    }

    //�÷��̾ ��ó�� ������ �ƹ��ϵ� ������ ��ó�� ������ �̺�Ʈ3���� ��� ������
    private void SubmitPressed()
    {
       if(!playerIsNear)
        {
            return;
        }

        GameEventManager.instance.questEvents.StartQuest(questId);
        GameEventManager.instance.questEvents.AdvanceQuest(questId);
        GameEventManager.instance.questEvents.FinishQuest(questId);
    }

    //����Ʈ�� id�� �� ����Ʈ ����Ʈid�� ��ġ�ϴ��� Ȯ��
    private void QuestStateChange(Quest quest)
    {
        if(quest.info.id.Equals(questId))
        {
            currentQuestState = quest.state;
            Debug.Log("quest id : " + questId + " ������Ʈ ����Ʈ ���� : " + currentQuestState);
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
