using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UISettings : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerDownHandler,IPointerUpHandler
{
    public TMP_Text  text; // 텍스트 참조
    public Button button; // 텍스트 참조
    public UIManager uiManager; // TextSettings 스크립터블 오브젝트 참조


    private ColorBlock normalColorBlock;
    private Color highlightedColor;
    private float originalAlpha;

    private float textoriginalAlpha;


    void Start()
    {
        button = GetComponent<Button>();
        text.fontSize = uiManager.defaultSize; // 시작 시 기본 크기로 설정
        textoriginalAlpha = text.alpha;


        if (SceneManager.GetActiveScene().name == "TestScene")
        {
            Transform secondChild = transform.GetChild(1);
            uiManager.secondChildImage = secondChild.GetComponent<Image>();

            if (uiManager.secondChildImage != null)
            {
                uiManager.secondChildImage.gameObject.SetActive(false); // 초기에는 비활성화
            }
            Debug.Log("ada");
        }


    }

    public void OnPointerEnter(PointerEventData eventData)
    {    
        
        if (uiManager != null && text != null )
        {
            text.fontSize = uiManager.clickedSize; // 버튼에 마우스가 올라가면 클릭 크기로 변경
           
        }
        if (uiManager.secondChildImage != null)
        {
            uiManager.secondChildImage.gameObject.SetActive(true);
        }
      
    }

    public void OnPointerExit(PointerEventData eventData)
    {
           
        if (uiManager != null && text != null)
        {
            text.fontSize = uiManager.defaultSize; // 버튼에서 마우스가 나가면 기본 크기로 변경
           
        }

        if (uiManager.secondChildImage != null)
        {
            uiManager.secondChildImage.gameObject.SetActive(false);
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        float h, s, v;
        Color.RGBToHSV(highlightedColor, out h, out s, out v); //
        v *= uiManager.btnBrightness; // 명도를 50%로 줄임 => h,s,v는 Color속성 중 R,G,B 값을 기존 255에서 128로 설정한다
        highlightedColor = Color.HSVToRGB(h, s, v); //255에서 128바꾼 것을 하이라이트 컬러로 초기화 한다.

        //버튼 불투명도 구현부
        originalAlpha = highlightedColor.a;
        highlightedColor.a = originalAlpha * uiManager.btnOpacity; // 불투명도 값을 50%로 줄임
        text.alpha = uiManager.textOpacity;

        normalColorBlock = button.colors; //기존 버튼의 색깔을 변수에 저장

        highlightedColor = normalColorBlock.highlightedColor; //기존 버튼의 색깔 중 하이라이트 컬러를
                                                              //하이라트 컬러 변수에 저장한다.

        //명도 구현부


        //명도,불투명도 초기화값
        normalColorBlock.highlightedColor = highlightedColor;
        button.colors = normalColorBlock;

        Debug.Log("adadad");
        normalColorBlock.highlightedColor = highlightedColor;
        button.colors = normalColorBlock;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        text.alpha = textoriginalAlpha;
        normalColorBlock.highlightedColor = highlightedColor;
        button.colors = normalColorBlock; //normalColorBlock는 현재 기본 버튼 색깔임
    }
}
