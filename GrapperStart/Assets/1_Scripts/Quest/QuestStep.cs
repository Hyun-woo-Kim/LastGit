    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�ٸ� ��ũ��Ʈ�� ��ӵǱ� ���� ��� �߻� Ŭ���� abstract
public abstract class QuestStep : MonoBehaviour
{
    private bool isFinished = false;

    //����Ʈ �ܰ谡 �Ϸ�Ǿ����� Ȯ��
    protected void FinishQuestStep()
    {
        if(!isFinished)
        {
            isFinished = true;
            //quest���� �̺�Ʈ�� �����Ͽ� �����ϵ���
            Destroy(this.gameObject);
        }
    }
}
