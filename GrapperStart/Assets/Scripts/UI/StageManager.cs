using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public GameObject PauseUI; //일시정지창

    public GameObject QuitUI; //나가시겠습니까 창 

    public GameObject OptionUI; //기본옵션,컨트롤옵션,사운드옵션 창


    public GameObject Ingame_OptionBGUI; //옵션이미지
    public GameObject Ingame_KeyOptionBGUI;//컨트롤옵션이미지
    public GameObject Ingame_SoundeOptionBGUI;//사운드옵션이미지 


    public bool paused = false;
    public bool OptionOn = false;
    private void Awake()
    {
        StartCoroutine(FADE());
    }
    void Start()
    {
        PauseUI.SetActive(false);     

        QuitUI.SetActive(false);

        OptionUI.SetActive(false);

        Ingame_OptionBGUI.SetActive(false);
        Ingame_KeyOptionBGUI.SetActive(false);
        Ingame_SoundeOptionBGUI.SetActive(false);
    }

    public Image Pause_Panel;
    void Update()
    {

       if (blockInput)
            return;
        //if (OptionOn)
        //{
        //    if(Input.GetKeyDown(KeyCode.Escape))
        //    {
        //        Debug.Log("aa");
        //        PauseUI.SetActive(true);
        //        // OptionOn 변수가 true일 때 UI 요소들을 비활성화
        //        OptionAlloff();
        //    }
           
        //}
        if (Input.GetKeyDown(KeyCode.Escape)) //누르면
        {
            paused = !paused; // paused 변수 토글
            if (paused)
            {
                Pause_Panel.color = new Color(0, 0, 0, 0.5f);
                PauseUI.SetActive(true); // 일시정지 메뉴 활성화
                Time.timeScale = 0.0f; // 시간 멈춤

            }
            else
            {
                Pause_Panel.color = new Color(0, 0, 0, 0);
                OptionUI.SetActive(false);
                Ingame_OptionBGUI.SetActive(false);
                PauseUI.SetActive(false); // 일시정지 메뉴 비활성화
                Time.timeScale = 1f; // 시간 원상 복귀
            }
        }


    }

    void OptionAlloff()
    {
        Ingame_OptionBGUI.SetActive(false);
        Ingame_KeyOptionBGUI.SetActive(false);
        Ingame_SoundeOptionBGUI.SetActive(false);
        OptionUI.SetActive(false);

    }

    //public void resume()
    //{
    //    paused = false;
    //    pauseui.setactive(false);
    //    time.timescale = 1f;
    //}
    public void goTime() //paused 가 false여야됨.
    {
        Debug.Log("goTime");
        paused = false;
        OptionOn = false;
        Pause_Panel.color = new Color(0, 0, 0, 0);
        PauseUI.SetActive(false); // 일시정지 메뉴 비활성화
        Time.timeScale = 1f; // 시간 원상 복귀

        //blockInput = true;
    }
    //public void notTime()
    //{
    //    Debug.Log("notTime");
    //    OptionOn = false;
    //    Pause_Panel.color = new Color(0, 0, 0, 0.5f);
    //    PauseUI.SetActive(true); // 일시정지 메뉴 활성화
    //    Time.timeScale = 0.0f; // 시간 멈춤

    //    blockInput = false; //입력 허용 
    //}


    public Image Panel;
    float time = 0.0f;
    float F_time = 1.0f;
    private IEnumerator FADE()
    {

        //float fadein = 2.0f;
        //while (fadein > 0.0f)
        //{
        //    fadein -= 0.05f;
        //    yield return new WaitForSeconds(0.01f);
        //    image.color = new Color(0, 0, 0, fadein);
        //}
       
        //페이드 인 : 어두운화면->밝은화면
        Color alpha = Panel.color;
        time = 0f;
       // yield return new WaitForSeconds(1f);

        //페이드 인 어두운화면->밝은 화면
        while (alpha.a > 0.0f)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(1, 0, time);
            Panel.color = alpha;
            yield return null;
        }
        Panel.gameObject.SetActive(false);
        yield return null;
    }


 

    public void quitNo()
    {      
        QuitUI.SetActive(false);
        goTime();
    }

    public bool blockInput = false;
   

    public void Option() //옵션 버튼을 누르면 
    {
        Debug.Log("옵션 클릭");
        PauseUI.SetActive(false);
        OptionOn = true;

        Ingame_OptionBGUI.SetActive(true);
        OptionUI.SetActive(true);
        Ingame_KeyOptionBGUI.SetActive(false);

        blockInput = false;
    }
    public void OptionOption()
    {
        Debug.Log("adad 옵션 클릭");
        Ingame_OptionBGUI.SetActive(true);

        Ingame_KeyOptionBGUI.SetActive(false);
        Ingame_SoundeOptionBGUI.SetActive(false);


    }
    public void KeyOption()
    {
        Debug.Log("조작키 옵션 클릭");
        Ingame_OptionBGUI.SetActive(false);
        Ingame_KeyOptionBGUI.SetActive(true);// 키옵션 배경이미지만 보이게.
        Ingame_SoundeOptionBGUI.SetActive(false);

       
    }

    public void SoundeOption()
    {
        Debug.Log("사운드옵션 클릭");
        Ingame_SoundeOptionBGUI.SetActive(true); //사운드옵션 배경이미지만 보이게.
        Ingame_KeyOptionBGUI.SetActive(false);
        Ingame_OptionBGUI.SetActive(false);
        
    }

    public void QuitYesorNo()
    {
        QuitUI.SetActive(true);
        PauseUI.SetActive(false);
    }
 

    public void Quit()
    {
        Application.Quit();

    }

    //public bool fading = false;
    //private IEnumerator FadeACoroutine()
    //{

    //        fading = true;
    //        float fadeCount = 0.5f;  
    //        yield return new WaitForSecondsRealtime(0.1f);

    //        image.color = new Color(0, 0, 0, 0.5f);



    //}

    //private IEnumerator FadeInCoroutine()
    //{

    //        fading = false;
    //        float fadeCount = 0.00f;
    //        yield return new WaitForSecondsRealtime(0.1f);

    //        image.color = new Color(0, 0, 0, 0);

    //}




    //public void SoundOption()
    //{
    //    blockInput = true;
    //    PauseUI.SetActive(false);
    //    SoundOptionUI.SetActive(true);

    //}
    //public void CloseSoundOption()
    //{
    //    blockInput = false;
    //    SoundOptionUI.SetActive(false);
    //    PauseUI.SetActive(true);
    //}


}
