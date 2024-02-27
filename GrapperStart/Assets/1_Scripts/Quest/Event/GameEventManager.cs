using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager instance { get; private set; }

    public QuestEvents questEvents; //퀘스트 이벤트 스크립트 가져옴
    public InputEvents inputEvents;
    public MiscEvents miscEvents;
    public CoinEvents coinEvents;
    
    void Start()
    {
        
    }


    void Update()
    {
        
    }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Game Events Manager in the scene.");
        }
        instance = this;

        questEvents = new QuestEvents();
        inputEvents = new InputEvents();
        miscEvents = new MiscEvents();
        coinEvents = new CoinEvents();
    }
}
