using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillEnemyStep : QuestStep  
{
    private int killEnemy = 0; //���� óġ�� ���� ��

    private int killEnemyComplete = 0; //óġ ��ǥ

    ////Ȱ��ȭ
    //private void OnEnable()
    //{
    //    //���͸� óġ���� �� �Ŵ����� ������ ������ ������ ���� óġ�� �ø���
    //}

    ////��Ȱ��ȭ
    //private void OnDisable()
    //{

    //}

    private void KillEnemy()
    {
        if(killEnemy < killEnemyComplete)
        {
            killEnemy++;
        }

        if (killEnemy >= killEnemyComplete)
        {
            FinishQuestStep();
        }
    }
}
