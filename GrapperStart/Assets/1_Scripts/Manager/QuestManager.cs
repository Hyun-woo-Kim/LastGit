using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestManager : MonoBehaviour
{
    private static QuestManager instance;
    //����Ʈ �Ŵ����� ���� �ν��Ͻ��� ��ȯ�� �ִ� ������Ƽ
    public static QuestManager Instance => instance;

    public QuestDatabaseObject questDatabase;

    //����Ʈ�� �Ϸ�Ǿ��ٴ°� �˷��ش�.
    public event Action<QuestObject> OnCompletedQuest;

    private void Awake()
    {
        instance = this; //�̱��� �ʱ�ȭ
    }

    //ų���� ����Ʈ Ÿ�Կ� � ���� óġ, ������ ȹ���ߴµ� � ����������
    //public void ProcessQuest(QuestType type, int targetId)
    //{

    //    foreach (QuestObject questObject in questDatabase.questObjects)
    //    {
    //        if (questObject.status == QuestStatus.Accepted 
    //            && questObject.data.type == type && UQ);
    //    }
    //}



}
