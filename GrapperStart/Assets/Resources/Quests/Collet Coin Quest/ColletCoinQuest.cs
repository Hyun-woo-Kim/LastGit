using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollectCoinsQuest : QuestStep
{
    private int coinsCollected = 0;
    private int coinsToComplete = 5;

    private void Start()
    {
        UpdateState();
    }

    private void OnEnable()
    {
        GameEventManager.instance.miscEvents.onCoinCollected += CoinCollected;
    }

    private void OnDisable()
    {
        GameEventManager.instance.miscEvents.onCoinCollected -= CoinCollected;
    }

    private void CoinCollected()
    {
        if (coinsCollected < coinsToComplete)
        {
            coinsCollected++;
            UpdateState();
        }

        if (coinsCollected >= coinsToComplete)
        {
            FinishQuestStep();
        }
    }

    private void UpdateState()
    {
        string state = coinsCollected.ToString();
        string status = "Collected " + coinsCollected + " / " + coinsToComplete + " coins.";
        //ChangeState(state, status);
    }
}

//    protected override void SetQuestStepState(string state)
//    {
//        this.coinsCollected = System.Int32.Parse(state);
//        UpdateState();
//    }
//}
