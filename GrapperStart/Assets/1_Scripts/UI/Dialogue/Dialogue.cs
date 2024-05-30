using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //Dialogue�� ����ȭ �Ͽ� �ٸ������� �ҷ��� �ٲ� �� �ְ�
public class Dialogue 
{
    [Tooltip("��� ġ�� ĳ���� �̸�")]
    public string name;

    [Tooltip("��� ����")]
    public string[] contexts;

}

[System.Serializable]
public class DialogueEvent
{
    public string name;

    public Vector2 line;
    public Dialogue[] dialogues;
}

