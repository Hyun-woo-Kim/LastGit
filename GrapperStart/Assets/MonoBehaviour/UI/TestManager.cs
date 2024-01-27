using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestManager : MonoBehaviour
{
    public Image Panel;
    public GameObject AllOption;

    public GameObject OptionBGUI;
    public GameObject KeyOptionBGUI;
    public GameObject SoundeOptionBGUI;
    public GameObject OptionUI;

    // private GameObject button;

    float time = 0.0f;
    float F_time = 1.0f;

    //public UIManager uiManager;
    private void Start()
    {
       
    }

    private bool isOption = false;
    private void Update()
    {
        if (isOption == false)
        {
            return;
        }
            if (Input.GetKeyDown(KeyCode.Escape) ) //누르면
        {
            if(isOption == true)
            {
                allOptionOff();
            }
        }
    }
    public void Fade()
    {
        //button.SetActive(false);
        Debug.Log("시작 버튼");
        StartCoroutine(FadeFlow());
    }

    IEnumerator FadeFlow() 
    {
      
        //페이드 아웃 밝은화면->어두운화면
        Panel.gameObject.SetActive(true);
        time = 0f;
        Color alpha = Panel.color;
        while(alpha.a <1.0f)
        {
            time += Time.deltaTime / F_time;
            alpha.a = Mathf.Lerp(0, 1, time);
            Panel.color = alpha;
            yield return null;
        }
        SceneManager.LoadScene("TestScene");

    }

  
    public void optionBTN()
    {
        isOption = true;
        AllOption.SetActive(true);
        OptionBGUI.SetActive(true);
        OptionUI.SetActive(true);
        KeyOptionBGUI.SetActive(false);
        SoundeOptionBGUI.SetActive(false);
    }

    public void KeyOptionBTN()
    {
        OptionBGUI.SetActive(false);
        KeyOptionBGUI.SetActive(true);// 키옵션 배경이미지만 보이게.
        SoundeOptionBGUI.SetActive(false);
    }

    public void SoundeOptionBTN()
    {
        SoundeOptionBGUI.SetActive(true); //사운드옵션 배경이미지만 보이게.
        KeyOptionBGUI.SetActive(false);
        OptionBGUI.SetActive(false);
    }

    void allOptionOff()
    {
        OptionBGUI.SetActive(false);
        KeyOptionBGUI.SetActive(false);
        SoundeOptionBGUI.SetActive(false);
        OptionUI.SetActive(false);
        AllOption.SetActive(false);
        isOption = false;
    }

    public void QuitSceneChange()
    {
        Debug.Log("종료");
        Application.Quit();
    }
}
