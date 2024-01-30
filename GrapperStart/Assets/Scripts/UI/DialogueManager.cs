using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour 
{
    public Text dialogueText;
   // public GameObject nextText;
    public CanvasGroup dialogueGroup; //대화가 시작되면 대화창이 켜지고 , 종료되면 대화창이 꺼지는 것을
                                      //구현하기 위해 CanvasGroup에 있는 Alpha값을 이용 할 것이다. 

    public Button nextButton;


    public Queue<string> sentences;
    //대화가 순차적으로 나오게 하려면 큐 방식을 이용해야 함. 
    //FIFO 방식이며, 먼저 들어간 데이터가 먼저 나오는 방식.

    private string currentSentence; //큐에 데이터 한개를 넣을 것이다.
    public float typingSpeed = 0.1f; //타이핑 속도
    public bool istyping;


    private static DialogueManager _instance = null;

    public static DialogueManager Instance
    {
        get
        {
            return _instance;
        }

    }
 
                
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    void Start()
    {
       
        sentences = new Queue<string>();
    }

   

    public void OnDialogue(string[] lines) //OnDialogue가 호출 될 때마다 큐에 대화를 넣고 대화창에 나오게 해준다
    {
        sentences.Clear(); //혹시 모를 큐에 있을 데이터를 비워준다. 
        foreach(string line in lines) //foreach문을 이용하여 전달 받은 인자들을 큐에 차례로 넣어준다.
        {
            sentences.Enqueue(line);
        }
        dialogueGroup.alpha = 1;
        dialogueGroup.blocksRaycasts = true; //blocksRaycasts가 true라는 것은 마우스 이벤트를 감지

        NextSentence();
    }

    public void NextSentence() //큐의 있는 데이터들을 전부 사용할 때 까지 반복해서 호출
    {
        Debug.Log("버튼");
        if(sentences.Count > 0)
        {
            currentSentence = sentences.Dequeue(); //Dequeue(): 큐에 존재하는 데이터 중 가장 먼저 들어온 데이터 반환
            //큐에서 해당 데이터를 제거한다. 
            istyping = true;
            nextButton.gameObject.SetActive(false);
            StartCoroutine(Typing(currentSentence));

            if (sentences.Count == 0) // 마지막 대사 일 경우
            {
                nextButton.GetComponentInChildren<Text>().text = "나가기";
            }
            else
            {
                nextButton.GetComponentInChildren<Text>().text = "다음";
            }

        }     
        else 
        {

            dialogueGroup.alpha = 0;
            dialogueGroup.blocksRaycasts = false;
        }
    }
    //코루틴을 이용하여 타이핑 효과 
    IEnumerator Typing(string line)
    {
        dialogueText.text = ""; //먼저 dialogueText는 빈 문자열로 초기화 한다. 
        foreach(char letter in line.ToCharArray())//ToCharArray()는 문자열을 Char형 배열로 반환 해준다
        {
            dialogueText.text += letter; //한 글자씩 dialogueText에 더해준다.
            yield return new WaitForSeconds(typingSpeed);
        }

    }

    void Update()
    {
        //dialogueText == currentSentence 이면 대사 한 줄 끝
        if (dialogueText.text.Equals(currentSentence))
        {
            //대사창을 클릭하면 다음 대사로 넘어가게 만들어야 함 
            nextButton.gameObject.SetActive(true);
            istyping = false;
        }
    }

    //public void OnPointerDown(PointerEventData eventData) //스크립트가 붙은 오브젝트에 클릭 터치 하면 호출
    //{
    //    if(istyping == false)
    //    {
    //        NextSentence();
    //    }
        
    //}

    public void NextBtn()
    {
        Debug.Log(sentences.Count);
        NextSentence();

    }
}
