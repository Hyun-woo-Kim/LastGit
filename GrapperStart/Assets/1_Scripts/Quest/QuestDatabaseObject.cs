using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest Database", menuName ="Quest System/Quests/New Quest Database")]
public class QuestDatabaseObject : ScriptableObject
{
    public QuestObject[] questObjects;

    //ID처리하기 위해
    public void OnValidate()
    {
        for(int index =0; index < questObjects.Length; ++index )
        {
            questObjects[index].data.id = index;
        }
    }


}
