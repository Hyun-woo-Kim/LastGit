using System;
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
    BManAction bm;
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerControllerRope>();
        bm = FindAnyObjectByType<BManAction>();
        currentHP = maxHP;
        currentMP = maxMP;

        isMPzero = false;
    }

    public float damageInterval = 1.0f;  // 피해를 줄 시간 간격 (초 단위)
    public float totalDuration = 5.0f;   // 총 피해를 줄 지속 시간 (초 단위)
    public float elapsedTime = 0.0f;
    public float damageTimer = 0.0f;
    public bool isDamaging = false;
    void Update()
    {
        UpdateHealthUI(); // UI 업데이트

        if (bm.RepeatCount >= 1.0f)
        {
            // 점화 시작
            isDamaging = true;
           
        }

        if (isDamaging)
        {
            elapsedTime += Time.deltaTime;
            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                StartCoroutine(Firedamage(0.1f));
                damageTimer = 0.0f;
            }

            if (elapsedTime >= totalDuration)
            {
                isDamaging = false;
            }
        }
    }
    IEnumerator Firedamage(float damage)
    {
        currentHP -= damage;
        yield return new WaitForSeconds(0.1f);
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

       

    }

    void UpdateHealthUI()
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
