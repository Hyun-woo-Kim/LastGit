using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    public Slider sliderHP;
    public Slider sliderMP;

    public float currentHP;
    public float maxHP;


    public float currentMP;
    public float maxMP;

    public bool isMPzero;
     PlayerControllerRope player;
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerControllerRope>();
        currentHP = maxHP;
        currentMP = maxMP;

        isMPzero = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {

        currentHP -= damage; // ��������ŭ ü�� ����
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP); // �ּҰ��� 0, �ִ밪�� �ִ� ü������ ����

        if (Mathf.RoundToInt(currentHP) > 0)
        {
            player.animatorPlayer.SetTrigger("PlayerHit");
            Debug.Log("�� ����");
        }
        else
        {
            player.animatorPlayer.SetTrigger("PlayerDeath");
            Debug.Log("�� ����");
        }

        UpdateHealthUI(damage); // UI ������Ʈ

    }

    void UpdateHealthUI(float damage)
    {
        sliderHP.value = currentHP / maxHP;
    }

    public void TakeGrapling(float grapValue)
    {
        currentMP -= grapValue;
        if(currentMP <= float.Epsilon)
        {
            isMPzero = true;
        }
        Debug.Log("GraplingValue: " + currentMP);
        GraplingSliderUI(); // UI ������Ʈ
    }

    void GraplingSliderUI()
    {
        Debug.Log("GraplingValue");


        sliderMP.value = currentMP ; // Update sliderMP based on currentMP and maxMP
       
    }

    void Die()
    {
        Debug.Log("Player died!");
    }
}
