using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestInfo" , menuName = "ScriptableObject/QuestInfo", order =1)]
public class QuestInfo : ScriptableObject
{
    [field : SerializeField] public string id { get; private set; }

    [Header("General")]
    public string displayName; //퀘스트 설명 이름 정도

    [Header("Steps")]
    public GameObject[] questStepPrefabs;

    [Header("Reward")]
    public int reward;





    private void OnValidate()
    {
        #if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
        #endif

    }
}
