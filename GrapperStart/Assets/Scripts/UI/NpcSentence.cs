using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcSentence : MonoBehaviour
{
    public string[] sentences; //대사 

    private void OnMouseDown() //npc가 클릭을 당하면 
    {
        if (DialogueManager.Instance .dialogueGroup.alpha == 0)
        {
            Debug.Log(DialogueManager.Instance.sentences.Count);
            DialogueManager.Instance.OnDialogue(sentences);
        }

        //else
        //{
        //    DialogueManager.Instance.NextSentence();
        //}
    }

   
}
