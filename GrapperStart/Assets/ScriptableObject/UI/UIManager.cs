using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "UISettings")]

public class UIManager : ScriptableObject
{
    public int defaultSize = 30; // 기본 크기
    public int clickedSize = 50; // 클릭 시 크기
    public float btnBrightness; //명도 값
    public float btnOpacity; //버튼 불투명도 값
    public float textOpacity; //텍스트 불투명도 값

    public Image  secondChildImage;

  
}
