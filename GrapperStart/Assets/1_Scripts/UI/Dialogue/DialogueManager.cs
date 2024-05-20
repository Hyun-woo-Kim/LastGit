using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    Dictionary<int, string[]> DialogueData; //�迭�� �������� ��ȭ������ ���� ������ �� �־

    private void Awake()
    {
        DialogueData = new Dictionary<int, string[]>();
        GenerateData();
    }

    void GenerateData()
    {
        DialogueData.Add(100, new string[] { "�ȳ�?", "�� ���� ó�� �Ա���?" }); //������Ʈ id�� �����Է�
    }

    public string GetTalk(int id, int DialogueIndex)
    {
        return DialogueData[id][DialogueIndex];
    }
}
