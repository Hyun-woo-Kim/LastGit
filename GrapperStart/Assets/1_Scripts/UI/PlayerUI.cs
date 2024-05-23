using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : SingleTonGeneric<PlayerUI>
{
    [Header("##Player")]
    public Slider sliderHP;
    public Slider sliderMP;
    public float currentHP;
    public float maxHP;
    public float currentMP;
    public float maxMP;
    public bool isMPzero;

    [Header("##BoomBerMan")]
    public float damageInterval; // 지속적인 피해를 줄 때 몇초 간격으로 줄어들게 할건지.
    public float totalDuration;   // 총 피해를 줄 지속 시간 (초 단위)
    private float elapsedTime = 0.0f;
    private float damageTimer = 0.0f;
    public bool isDamaging = false;
    public bool isDamagedPlayer = false;
    public int continueCount; //연속 조건, 1로 설정 시 (1타 -> 2타) x 1  , 2로 설정 시 (1타 -> 2타) x 2

    [Header("##UI")]
    public GameObject SelectUI;
    public bool isSelectUI = false;

    PlayerControllerRope player;
    BManAction bm;

    public void UISingleTonSet()
    {

    }

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerControllerRope>();
        bm = FindAnyObjectByType<BManAction>();

        UIBeAcitve();
        
        currentHP = maxHP;
        currentMP = maxMP;

        isMPzero = false;
    }

    void Update()
    {
        UpdateHealthUI(); // UI 업데이트

        if(bm != null)
        {
            if (bm.RepeatCount >= continueCount)
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
        
    }


    IEnumerator Firedamage(float damage)
    {
        isDamagedPlayer = true;
        currentHP -= damage;
        player.animatorPlayer.SetTrigger("PlayerHit");
        yield return new WaitForSeconds(0.1f);
        isDamagedPlayer = false;



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
        if (currentMP <= float.Epsilon)
        {
            isMPzero = true;
        }
        Debug.Log("GraplingValue: " + currentMP);
        GraplingSliderUI(); // UI 업데이트
    }

    void GraplingSliderUI()
    {
        Debug.Log("GraplingValue");


        sliderMP.value = currentMP; // Update sliderMP based on currentMP and maxMP

    }

    public void UIActive()
    {
        isSelectUI = true;
        SelectUI.gameObject.SetActive(true);
    }
    public void UIBeAcitve()
    {
        isSelectUI = false;
        SelectUI.gameObject.SetActive(false);
    }
    void Die()
    {
        Debug.Log("Player died!");
    }
}
