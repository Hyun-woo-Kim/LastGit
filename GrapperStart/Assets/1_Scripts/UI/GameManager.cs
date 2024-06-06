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

    public int talkIndex;
    public Text UINameText;
    public Text UITalkText;
    public string name;
    public GameObject scanObject;
    public GameObject talkPanel;
    public Image portraitImg; //�ʻ�ȭ
    public bool isAction; //��ȭâ Ȱ��ȭ ���� 


  

    //����Ʈ ���� UI
    public Text UIQuest;

    public static GameManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        button = GameObject.Find("StartButton");

<<<<<<< Updated upstream
        //questManager.CheckQuest(); //���� �������ڸ��� ����Ʈ �̸��� ��������
=======
       
>>>>>>> Stashed changes
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
        Debug.Log("����");
        Application.Quit();
    }

<<<<<<< Updated upstream
    //��ȭ ����
    public void Action(GameObject scanObj)
    {
        scanObject = scanObj;
        name = scanObject.name;
        //UITalkText.text = "�̰��� "+scanObject.name+"�̴�.";
        ObjData objData = scanObject.GetComponent<ObjData>();
        Talk(objData.id, objData.isNPC);


        talkPanel.SetActive(isAction); //��ȭâ Ȱ��ȭ ���¿� ���� ��ȭâ Ȱ��ȭ ����
    }

    //���� ������ UI�� ����ϴ� �Լ�
    void Talk(int id, bool isNPC)
    {
        int questTalkIndex = questManager.GetQuestTalkIndex(id); // ������ obj�� id�� �Ѱ� ����Ʈ id�� ��ȯ���� 

        string talkData = DialogueManager.D_instance.GetTalk(id + questTalkIndex, talkIndex); //id�� ����Ʈ id�� ���ϸ� -> �ش� id�� ���� ������Ʈ�� ���� ����Ʈ�� ��ȭ�� ��ȯ�ϰ� �����

        if (talkData == null) //��ȯ�� ���� null�̸� ���̻� ���� ��簡 �����Ƿ� action���º����� false�� ���� 
        {
            isAction = false;
            talkIndex = 0; //talk�ε����� ������ �� ���ǹǷ� �ʱ�ȭ�ؾ��� 
            Debug.Log(questManager.CheckQuest(id));
            return; //void������ return �Լ� �������� (���� �ڵ�� ������� ����)
        }

        if (isNPC)
        {
            UITalkText.text = talkData.Split(':')[0]; //�����ڷ� ������ ������  0: ��� 1:portraitIndex
                                                      //portraitImg.sprite = DialogueManager.D_instance.GetPortrait(id, int.Parse(talkData.Split(':')[1]));
                                                      //�ʻ�ȭ�� ���̰��� (���� 1)
                                                      //portraitImg.color = new Color(1, 1, 1, 1);
            if (int.Parse(talkData.Split(':')[1]) == 1)
            {
                UINameText.text = "Player";//1�ΰ�� �鼳���� �ʻ�ȭ�� ����Ǿ� ������ �̰���...
            }
            else
            {
                UINameText.text = name; //�������� ���� �̸� UI�� �̸� �����ص� name��� 
            }

        }
        else
        {
            UINameText.text = "";
            UITalkText.text = "";
            UITalkText.text = talkData; //������ �����ڰ� �����Ƿ� �׳� ��°��� 

            //�ʻ�ȭ�� �Ⱥ��̰���(���� 0) 
            //portraitImg.color = new Color(1, 1, 1, 0);
        }

        //���� ������ �������� ���� talkData�� �ε����� �ø�
        isAction = true; //��簡 ���������Ƿ� ��� ����Ǿ���� 
        talkIndex++;
    }
=======
   
>>>>>>> Stashed changes
}
