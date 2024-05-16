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
    //        Debug.Log("데미지 입음");
    //        TakeDamage(10f);
    //    }
    //}

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


    void Die()
    {
        Debug.Log("Player died!");
    }
}
