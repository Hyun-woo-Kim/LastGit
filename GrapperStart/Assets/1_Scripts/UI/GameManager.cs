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
    public Image portraitImg; //초상화
    public bool isAction; //대화창 활성화 상태 


  

    //퀘스트 관련 UI
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
        //questManager.CheckQuest(); //게임 시작하자마자 퀘스트 이름을 가져오기
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
        Debug.Log("종료");
        Application.Quit();
    }

<<<<<<< Updated upstream
    //대화 관련
    public void Action(GameObject scanObj)
    {
        scanObject = scanObj;
        name = scanObject.name;
        //UITalkText.text = "이것은 "+scanObject.name+"이다.";
        ObjData objData = scanObject.GetComponent<ObjData>();
        Talk(objData.id, objData.isNPC);


        talkPanel.SetActive(isAction); //대화창 활성화 상태에 따라 대화창 활성화 변경
    }

    //실제 대사들을 UI에 출력하는 함수
    void Talk(int id, bool isNPC)
    {
        int questTalkIndex = questManager.GetQuestTalkIndex(id); // 조사한 obj의 id를 넘겨 퀘스트 id를 반환받음 

        string talkData = DialogueManager.D_instance.GetTalk(id + questTalkIndex, talkIndex); //id에 퀘스트 id를 더하면 -> 해당 id를 가진 오브젝트가 가진 퀘스트의 대화를 반환하게 만들기

        if (talkData == null) //반환된 것이 null이면 더이상 남은 대사가 없으므로 action상태변수를 false로 설정 
        {
            isAction = false;
            talkIndex = 0; //talk인덱스는 다음에 또 사용되므로 초기화해야함 
            Debug.Log(questManager.CheckQuest(id));
            return; //void에서의 return 함수 강제종료 (밑의 코드는 실행되지 않음)
        }

        if (isNPC)
        {
            UITalkText.text = talkData.Split(':')[0]; //구분자로 문장을 나눠줌  0: 대사 1:portraitIndex
                                                      //portraitImg.sprite = DialogueManager.D_instance.GetPortrait(id, int.Parse(talkData.Split(':')[1]));
                                                      //초상화를 보이게함 (투명도 1)
                                                      //portraitImg.color = new Color(1, 1, 1, 1);
            if (int.Parse(talkData.Split(':')[1]) == 1)
            {
                UINameText.text = "Player";//1인경우 백설공주 초상화가 저장되어 무조건 이거임...
            }
            else
            {
                UINameText.text = name; //나머지일 때는 이름 UI에 미리 저장해둔 name출력 
            }

        }
        else
        {
            UINameText.text = "";
            UITalkText.text = "";
            UITalkText.text = talkData; //별도의 구분자가 없으므로 그냥 출력가능 

            //초상화를 안보이게함(투명도 0) 
            //portraitImg.color = new Color(1, 1, 1, 0);
        }

        //다음 문장을 가져오기 위해 talkData의 인덱스를 늘림
        isAction = true; //대사가 남아있으므로 계속 진행되어야함 
        talkIndex++;
    }
=======
   
>>>>>>> Stashed changes
}
