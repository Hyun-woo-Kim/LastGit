using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    public Slider sliderHP;
    public float currentHP;
    public float maxHP;


     PlayerControllerRope player;
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerControllerRope>();
        currentHP = maxHP;
        UpdateHealthUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if(collision.CompareTag("Enemy"))
    //    {
    //        Debug.Log("������ ����");
    //        TakeDamage(10f);
    //    }
    //}

    public void TakeDamage(float damage)
    {
        if (Mathf.RoundToInt(currentHP) > 0)
        {
            player.animatorPlayer.SetTrigger("PlayerHit");
            currentHP -= damage; // ��������ŭ ü�� ����
            currentHP = Mathf.Clamp(currentHP, 0f, maxHP); // �ּҰ��� 0, �ִ밪�� �ִ� ü������ ����
            Debug.Log("�� ����");
        }
        else
        {
            player.animatorPlayer.SetTrigger("PlayerDeath");
            Debug.Log("�� ����");
            return;
        }


        UpdateHealthUI(); // UI ������Ʈ

      
    }

    void UpdateHealthUI()
    {
        sliderHP.value = currentHP; // Slider ���� ���� ü�¿� ���� ����
    }


    void Die()
    {
        Debug.Log("Player died!");
    }
}
