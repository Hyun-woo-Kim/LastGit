using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] GameObject dialogueBar;
    [SerializeField] GameObject nameBar;

    [SerializeField] TMP_Text text_dialogue;
    [SerializeField] TMP_Text text_name;

    Dialogue[] dialogues;

    bool isDialogue = false;

    bool isNext = false;    // Ư�� Ű �Է� ��⸦ ���� ����
    int dialogueCnt = 0;    // ��ȭ ī��Ʈ. �� ĳ���Ͱ� �� ���ϸ� 1 ����
    int contextCnt = 0; 	// ��� ī��Ʈ. �� ĳ���Ͱ� ���� ��縦 �� �� �ִ�.

    Interaction interaction;
    // Start is called before the first frame update
    void Start()
    {
        interaction = FindObjectOfType<Interaction>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (isDialogue)
        {
            if (isNext)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    isNext = false;
                    text_dialogue.text = "";

                    ++contextCnt;
                    StartCoroutine(TypeWriter());
                }
            }
        }
    }

    public void ShowDialogue(Dialogue[] P_dialogues)
    {
        isDialogue = true;
        text_dialogue.text = "";
        text_name.text = "";
        dialogues = P_dialogues;

       

        StartCoroutine(TypeWriter());	// ��ȣ�ۿ� ���ÿ� �ؽ�Ʈ ��� �ڷ�ƾ ����
    }

    void SettingUI(bool flag)
    {
        dialogueBar.SetActive(flag);
        nameBar.SetActive(flag);
    }

    // �ؽ�Ʈ ��� �ڷ�ƾ
    IEnumerator TypeWriter()
    {
        SettingUI(true);    // ���â �̹����� ����.

        string t_ReplaceText = dialogues[dialogueCnt].contexts[contextCnt];   // Ư�����ڸ� ,�� ġȯ
        t_ReplaceText = t_ReplaceText.Replace("`", ",");    // backtick�� comma�� ��ȯ

        text_dialogue.text = t_ReplaceText;

        isNext = true;

        yield return null;   // ���� ��縦 ��� �����ϵ���
    }

    void EndDialogue()
    {
        isDialogue = false;
        contextCnt = 0;
        dialogueCnt = 0;
        dialogues = null;
        isNext = false;
        SettingUI(false);
    }
}
