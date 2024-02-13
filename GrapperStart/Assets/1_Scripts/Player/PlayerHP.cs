using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHP : MonoBehaviour
{
    public Slider sliderHP;
    public float correntHP;
    public float maxHP;

    void Start()
    {
        correntHP = maxHP;
        UpdateHealthUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            Debug.Log("������ ����");
            TakeDamage(10f);
        }
    }

    void TakeDamage(float damage)
    {
        correntHP -= damage; // ��������ŭ ü�� ����
        correntHP = Mathf.Clamp(correntHP, 0f, maxHP); // �ּҰ��� 0, �ִ밪�� �ִ� ü������ ����

        UpdateHealthUI(); // UI ������Ʈ

        if (correntHP <= 0f)
        {
            Die(); // ü���� 0 ������ ��� ��� ó��
        }
    }

    void UpdateHealthUI()
    {
        sliderHP.value = correntHP; // Slider ���� ���� ü�¿� ���� ����
    }

    void Die()
    {
        Debug.Log("Player died!");
    }
}
