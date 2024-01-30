using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BloodState
{
    STATE_PATROL,
    STATE_ATTACK,
    
}
public class BloodWorkerAction : MonoBehaviour
{
    BloodState bloodState;
    private BloodWorkerState myState;

    


    

    void Start()
    {
        this.bloodState = BloodState.STATE_PATROL; //기본적으로 순찰 상태로 변경.
        setActionType(bloodState);
        myState = gameObject.AddComponent<BloodWorkerPatrol>();
        myState.Initialize(bloodState); 
    }

    public void setActionType(BloodState _bloodState)
    {
        

        Component c = gameObject.GetComponent<BloodWorkerState>();

        if (c != null)
        {
            Destroy(c);
        }

        switch (_bloodState)
        {
            case BloodState.STATE_PATROL:
                myState = gameObject.AddComponent<BloodWorkerPatrol>();
                
                myState.Patrol(_bloodState);
              
                break;
           
        }
    }


 
    void Update()
    {
        switch (bloodState)
        {
            case BloodState.STATE_PATROL:
             
                setActionType(BloodState.STATE_PATROL);
                break;
        }
    }
}
