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
            Debug.Log("데미지 입음");
            TakeDamage(10f);
        }
    }

    void TakeDamage(float damage)
    {
        correntHP -= damage; // 데미지만큼 체력 감소
        correntHP = Mathf.Clamp(correntHP, 0f, maxHP); // 최소값은 0, 최대값은 최대 체력으로 제한

        UpdateHealthUI(); // UI 업데이트

        if (correntHP <= 0f)
        {
            Die(); // 체력이 0 이하인 경우 사망 처리
        }
    }

    void UpdateHealthUI()
    {
        sliderHP.value = correntHP; // Slider 값을 현재 체력에 따라 조절
    }

    void Die()
    {
        Debug.Log("Player died!");
    }
}
