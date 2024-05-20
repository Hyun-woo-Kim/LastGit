using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    Dictionary<int, string[]> DialogueData; //배열로 만든이유 대화내용이 여러 문장일 수 있어서

    private void Awake()
    {
        DialogueData = new Dictionary<int, string[]>();
        GenerateData();
    }

    void GenerateData()
    {
        DialogueData.Add(100, new string[] { "안녕?", "이 곳에 처음 왔구나?" }); //오브젝트 id와 문장입력
    }

    public string GetTalk(int id, int DialogueIndex)
    {
        return DialogueData[id][DialogueIndex];
    }
}
