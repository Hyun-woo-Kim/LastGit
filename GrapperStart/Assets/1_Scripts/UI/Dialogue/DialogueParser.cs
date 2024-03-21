using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueParser : MonoBehaviour
{
    public Dialogue[] Parse(string CSVFileName)
    {
        List<Dialogue> dialogueList = new List<Dialogue>(); //��� ����Ʈ ����
        TextAsset csvData = Resources.Load<TextAsset>(CSVFileName); //CSV���� ������

        string[] data = csvData.text.Split(new char[] { '\n' }); //csv���Ͽ��� ���ͷ� ������ �����´�

        for (int i = 1; i < data.Length;)
        {
            string[] row = data[i].Split(new char[] { ',' }); //, ������ �ɰ��� row�� ���� �ִ´�

            //Debug.Log(row[0]);
            //Debug.Log(row[1]);
            //Debug.Log(row[2]);
            Dialogue dialogue = new Dialogue(); //��� ����Ʈ ����

            dialogue.name = row[1];

            List<string> contextList = new List<string>();

            do
            {
                contextList.Add(row[2]);
                if (++i < data.Length)
                {
                    row = data[i].Split(new char[] { ',' });
                }
                else
                {
                    break;
                }
            } while (row[0].ToString() == ""); //ID������ �����̸� 

            dialogue.contexts = contextList.ToArray(); //�迭�� �ٲٱ�

            dialogueList.Add(dialogue);

        }

        return dialogueList.ToArray(); //dialogueList�� �迭�� �ٲ���
    }

}

  
