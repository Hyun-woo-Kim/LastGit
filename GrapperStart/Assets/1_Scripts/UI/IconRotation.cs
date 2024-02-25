using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; //지시문 추가

public class IconRotation : MonoBehaviour
{
    public Image image; // 이미지 UI
    public Color defaultColor; // 기본 색상
    public Color highlightColor; // 강조 색상
    public Animator animator;
    public Slider slider;

   

    private void OnMouseEnter()
    {
        image.color = highlightColor;

        animator.SetTrigger("OnMouseEnter");
    }

    private void OnMouseExit()
    {
        image.color = defaultColor;
        animator.ResetTrigger("OnMouseEnter");
    }
}

