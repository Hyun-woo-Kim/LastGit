using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager instance { get; private set; }

    public QuestEvents questEvents; //����Ʈ �̺�Ʈ ��ũ��Ʈ ������
    
    void Start()
    {
        
    }


    void Update()
    {
        
    }

    private void Awake()
    {
        questEvents = new QuestEvents();
    }
}
