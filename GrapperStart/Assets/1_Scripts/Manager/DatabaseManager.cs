using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance; //����Ͽ� ���� �ҷ��´�

    [SerializeField]
    string csv_FIleName;

    //��ȭ ���� 2,3,4���� �����Ͷ� �̷���
    Dictionary<int, Dialogue> dialogueDIc = new Dictionary<int, Dialogue>(); //int���� dialogue�� �ҷ��´�


    public static bool isFinish= false; //������ �Ǿ��� �ȉ��

    private void Awake()
    {
       if(instance == null)
        {
            instance =this; //DatabaseManager��ü�� �������ڽſ�
            DialogueParser theParser = GetComponent<DialogueParser>();
            Dialogue[] dialogues = theParser.Parse(csv_FIleName);
            for(int i = 0; i<dialogues.Length; i++)
            {
                dialogueDIc.Add(i + 1, dialogues[i]);
            }
            isFinish = true;
        }
    }

    //���� ��
    public Dialogue[] GetDialogue(int StartNum, int EndNum)
    {
        List<Dialogue> dialogueList = new List<Dialogue>();

        for(int i = 0; i<=EndNum -StartNum; i++)
        {
            dialogueList.Add(dialogueDIc[StartNum + i]); //��ȭ �� �������� ó������ �ݺ��Ǵ� ����ŭ

        }

        return dialogueList.ToArray();
    }
}
