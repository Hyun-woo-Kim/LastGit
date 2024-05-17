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

    public float damageInterval = 1.0f;  // ���ظ� �� �ð� ���� (�� ����)
    public float totalDuration = 5.0f;   // �� ���ظ� �� ���� �ð� (�� ����)
    public float elapsedTime = 0.0f;
    public float damageTimer = 0.0f;
    public bool isDamaging = false;
    void Update()
    {
        UpdateHealthUI(); // UI ������Ʈ

        if (bm.RepeatCount >= 1.0f)
        {
            // ��ȭ ����
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
