using System;

public class QuestEvents 
{
    public event Action<string> onStartQuest;

    public void StartQuest(string id)
    {
        if(onStartQuest != null)
        {
            onStartQuest(id);
        }
    }

    public event Action<string> onAdvanceQuest;

    public void AdvanceQuest(string id)
    {
        if (onAdvanceQuest != null)
        {
            onAdvanceQuest(id);
        }
    }

    public event Action<string> onFinishQuest;

    public void FinishQuest(string id)
    {
        if (onFinishQuest != null)
        {
            onFinishQuest(id);
        }
    }


    //문자열 대신 전체Quest개체를 전달하려는 이벤트 ㅅ
    public event Action<Quest> onQuestStateChange;
    //게임매니저 스크립트에 연결애햐함
    public void QuestStateChange(Quest quest)
    {
        if (onQuestStateChange != null)
        {
            onQuestStateChange(quest);
        }
    }
}
