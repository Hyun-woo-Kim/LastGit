using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Image image;
    private GameObject button;
    public GameObject soundop;

    public DialogueManager dialogueManager;
    public GameObject dialoguePanel;
    public Text dialogueText;
    public GameObject scanObject;
    public bool isAction;
    public int DialogueIndex;

    private void Start()
    {
        button = GameObject.Find("StartButton");
    }

    public void Fadebutton()
    {
        button.SetActive(false);
        StartCoroutine(FadeCoroutine());
    }

    public void soundopTrue()
    {
        soundop.SetActive(true);
    }

   public void soundopFalse()
    {
        soundop.SetActive(false);
    }
    public float fadeDur;
    private IEnumerator FadeCoroutine()
    {
        float fadeCount = 0;
        while (fadeCount < fadeDur)
        {
            fadeCount += 0.01f;
            yield return new WaitForSeconds(0.01f);
            image.color = new Color(0, 0, 0, fadeCount);
        }

        SceneManager.LoadScene("TestScene");
    }

    public void QuitSceneChange()
    {
        Debug.Log("종료");
        Application.Quit();
    }

    //대화 관련
    public void Action(GameObject scanobj)
    {
        if (isAction)
        {
            isAction = false;
        }
        else
        {
            isAction = true;
            scanObject = scanobj;
            ObjData objData = scanObject.GetComponent<ObjData>();
            Talk(objData.id, objData.isNpc);
        }

        dialoguePanel.SetActive(isAction);
    }

    void Talk(int id, bool isNpc)
    {
       string dialogueData = dialogueManager.GetTalk(id, DialogueIndex);
       
        if(isNpc)
        {
            dialogueText.text = dialogueData;
        }
        else
        {
            dialogueText.text = dialogueData;
        }
    }
}
