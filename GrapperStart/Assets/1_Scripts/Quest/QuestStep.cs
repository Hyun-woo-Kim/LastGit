    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//다른 스크립트에 상속되기 위해 사용 추상 클래스 abstract
public abstract class QuestStep : MonoBehaviour
{
    private bool isFinished = false;

    //퀘스트 단계가 완료되었는지 확인
    protected void FinishQuestStep()
    {
        if(!isFinished)
        {
            isFinished = true;
            //quest앱이 이벤트를 전송하여 진행하도록
            Destroy(this.gameObject);
        }
    }
}
