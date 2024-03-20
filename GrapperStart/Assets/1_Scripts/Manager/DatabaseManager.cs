using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance; //상속하여 쉽게 불러온다

    [SerializeField]
    string csv_FIleName;

    //대화 내용 2,3,4줄을 꺼내와라 이런식
    Dictionary<int, Dialogue> dialogueDIc = new Dictionary<int, Dialogue>(); //int값의 dialogue를 불러온다


    public static bool isFinish= false; //저장이 되었냐 안됬냐

    private void Awake()
    {
       if(instance == null)
        {
            instance =this; //DatabaseManager전체를 가져옴자신에
            DialogueParser theParser = GetComponent<DialogueParser>();
            Dialogue[] dialogues = theParser.Parse(csv_FIleName);
            for(int i = 0; i<dialogues.Length; i++)
            {
                dialogueDIc.Add(i + 1, dialogues[i]);
            }
            isFinish = true;
        }
    }

    //시작 끝
    public Dialogue[] GetDialogue(int StartNum, int EndNum)
    {
        List<Dialogue> dialogueList = new List<Dialogue>();

        for(int i = 0; i<=EndNum -StartNum; i++)
        {
            dialogueList.Add(dialogueDIc[StartNum + i]); //대화 줄 꺼내오기 처음부터 반복되는 수만큼

        }

        return dialogueList.ToArray();
    }
}
