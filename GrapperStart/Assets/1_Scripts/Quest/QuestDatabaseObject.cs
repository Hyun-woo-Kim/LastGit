using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest Database", menuName ="Quest System/Quests/New Quest Database")]
public class QuestDatabaseObject : ScriptableObject
{
    public QuestObject[] questObjects;

    //IDó���ϱ� ����
    public void OnValidate()
    {
        for(int index =0; index < questObjects.Length; ++index )
        {
            questObjects[index].data.id = index;
        }
    }


}
