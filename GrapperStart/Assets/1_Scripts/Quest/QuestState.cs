using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestState 
{
    Requirements_Not_Met, //����Ʈ�� �ȹ��� ����

    Can_Start, //����Ʈ�� �ޱ⸸ �� ����

    In_Progress, //����Ʈ�� ������

    Can_Finsh, //����Ʈ ���� �� �ִ� ����

    Finished // ���� ���� (���� �ޱ�)
}
