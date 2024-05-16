using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    public Slider sliderHP;
    public float currentHP;
    public float maxHP;

    public bool isHealth;
     PlayerControllerRope player;
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerControllerRope>();
        currentHP = maxHP;
        isHealth = true;
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


    void Die()
    {
        Debug.Log("Player died!");
    }
}
