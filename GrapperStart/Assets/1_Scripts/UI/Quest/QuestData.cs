using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestData : MonoBehaviour
{
    public string questName; //����Ʈ ��
    public int[] NpcId; // ����Ʈ�� ���õ� NPC id ����




    //�� ������
    // public QuestData() { }

    //������
    public QuestData(string name, int[] npcid)
    {
        questName = name;
        NpcId = npcid;
    }
}
