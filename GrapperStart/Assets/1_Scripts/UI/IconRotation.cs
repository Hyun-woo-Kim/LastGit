using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; //���ù� �߰�

public class IconRotation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image image; // �̹��� UI
    public Animator animator;
    public Slider slider;


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (slider.value >= 3)
        {
            //animator.SetBool("MouseBool", true);
            animator.speed = 2.0f;
        }
        else
        {
            //animator.SetBool("MouseBool", false);
            animator.speed = 1.0f;
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (slider.value >= 3)
        {
            //animator.SetBool("MouseBool", false);
            animator.speed = 1.0f;
        }
 
    
    }
}


