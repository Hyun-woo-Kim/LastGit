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

        currentHP -= damage; // 데미지만큼 체력 감소
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP); // 최소값은 0, 최대값은 최대 체력으로 제한

        if (Mathf.RoundToInt(currentHP) > 0)
        {
            player.animatorPlayer.SetTrigger("PlayerHit");
            Debug.Log("피 있음");
        }
        else
        {
            player.animatorPlayer.SetTrigger("PlayerDeath");
            Debug.Log("피 없음");
        }

        UpdateHealthUI(damage); // UI 업데이트

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
        GraplingSliderUI(); // UI 업데이트
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
