using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueParser : MonoBehaviour
{
    public Dialogue[] Parse(string CSVFileName)
    {
        List<Dialogue> dialogueList = new List<Dialogue>(); //대사 리스트 생성
        TextAsset csvData = Resources.Load<TextAsset>(CSVFileName); //CSV파일 가져옴

        string[] data = csvData.text.Split(new char[] { '\n' }); //csv파일에서 엔터로 나눠서 가져온다

        for (int i = 1; i < data.Length;)
        {
            string[] row = data[i].Split(new char[] { ',' }); //, 단위로 쪼개서 row에 집어 넣는다

            Dialogue dialogue = new Dialogue(); //대사 리스트 생성

            dialogue.name = row[1];
            Debug.Log(row[1]);

            List<string> contextList = new List<string>();

            do
            {
                contextList.Add(row[2]);
                Debug.Log(row[2]);
                if(++i < data.Length)
                {
                    row = data[i].Split(new char[] { ',' });
                }
                else
                {
                    break;
                }
            } while (row[0].ToString() == ""); //ID내용이 공백이면 



        }

        return dialogueList.ToArray(); //dialogueList을 배열로 바꿔줌
    }

    private void Start()
    {
        Parse("Dialogue");
    }
}
