using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject dialogueBar;
    [SerializeField] GameObject nameBar;

    [SerializeField] TMP_Text text_dialgue;
    [SerializeField] TMP_Text text_name;

    Dialogue[] dialogues;

    bool isDialogue = false;

    Interaction interaction;
    // Start is called before the first frame update
    void Start()
    {
        interaction = FindObjectOfType<Interaction>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowDialogue(Dialogue[] P_dialogues)
    {
        text_dialgue.text = "";
        text_name.text = "";
        dialogues = P_dialogues;

        SettingUI(true);
    }

    void SettingUI(bool flag)
    {
        dialogueBar.SetActive(flag);
        nameBar.SetActive(flag);
    }
}
