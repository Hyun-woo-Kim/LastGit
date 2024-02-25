using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; //���ù� �߰�

public class IconRotation : MonoBehaviour
{
    public Image image; // �̹��� UI
    public Color defaultColor; // �⺻ ����
    public Color highlightColor; // ���� ����
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

