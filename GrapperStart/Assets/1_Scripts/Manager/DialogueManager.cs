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

    bool isDialogue = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowDialogue()
    {
        text_dialgue.text = "";
        text_name.text = "";

        SettingUI(true);
    }

    void SettingUI(bool flag)
    {
        dialogueBar.SetActive(flag);
        nameBar.SetActive(flag);
    }
}
