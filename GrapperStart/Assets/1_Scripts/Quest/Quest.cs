using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest 
{
    public QuestInfo info; //QuestInfo��ũ��Ʈ�� ���� ���� ��������

    public QuestState state; //QuestState ������

    private int currentQuestStepIndex; //����� ���� ����

    public Quest(QuestInfo questInfo)
    {
        this.info = questInfo; //����Ʈ ���������°��� �߰����ְ�
        this.state = QuestState.Requirements_Not_Met; //����Ʈ ���¸� ó������ �ȹ��� ����
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

    //���� �ܰ��� ����Ʈ�� ���������� Ȯ���ϰ� �ƴϸ� �ٽ� ���ư��� �޼���
    private GameObject GetCurrentQuestStepPrefab()
    {
        GameObject questStepPrefab = null; //����Ʈ �������� null��

        if(CurrentStepExists()) //CurrentStepExists�޼��带 �̿��Ͽ� ���� �ܰ谡 �����ϴ��� Ȯ��
        {
            questStepPrefab = info.questStepPrefabs[currentQuestStepIndex]; //ScriptableObject���� �������� �����´�.

        }
        else
        {
            Debug.LogWarning("Quest �ܰ� �������� ���������� �õ������� stepIndex�� ������ ������ϴ�. �̴�" + "���� �ܰ谡 ������ ��Ÿ���ϴ�." +
                " QuestID = " + info.id + ", stepIndex= " + currentQuestStepIndex);
        }
        return questStepPrefab;
    }
}
