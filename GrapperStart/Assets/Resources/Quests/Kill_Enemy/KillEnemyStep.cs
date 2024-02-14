using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillEnemyStep : QuestStep  
{
    private int killEnemy = 0; //현재 처치한 몬스터 수

    private int killEnemyComplete = 0; //처치 목표

    ////활성화
    //private void OnEnable()
    //{
    //    //몬스터를 처치했을 때 매니저에 설정한 정보를 가져와 몬스터 처치를 올린다
    //}

    ////비활성화
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
