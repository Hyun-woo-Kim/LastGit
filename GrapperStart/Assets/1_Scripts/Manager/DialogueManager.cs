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

    bool isNext = false;    // 특정 키 입력 대기를 위한 변수
    int dialogueCnt = 0;    // 대화 카운트. 한 캐릭터가 다 말하면 1 증가
    int contextCnt = 0; 	// 대사 카운트. 한 캐릭터가 여러 대사를 할 수 있다.

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

       

        StartCoroutine(TypeWriter());	// 상호작용 동시에 텍스트 출력 코루틴 시작
    }

    void SettingUI(bool flag)
    {
        dialogueBar.SetActive(flag);
        nameBar.SetActive(flag);
    }

    // 텍스트 출력 코루틴
    IEnumerator TypeWriter()
    {
        SettingUI(true);    // 대사창 이미지를 띄운다.

        string t_ReplaceText = dialogues[dialogueCnt].contexts[contextCnt];   // 특수문자를 ,로 치환
        t_ReplaceText = t_ReplaceText.Replace("`", ",");    // backtick을 comma로 변환

        text_dialogue.text = t_ReplaceText;

        isNext = true;

        yield return null;   // 다음 대사를 출력 가능하도록
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
