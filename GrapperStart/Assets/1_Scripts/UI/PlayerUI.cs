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
    public float damageInterval; // �������� ���ظ� �� �� ���� �������� �پ��� �Ұ���.
    public float totalDuration;   // �� ���ظ� �� ���� �ð� (�� ����)
    private float elapsedTime = 0.0f;
    private float damageTimer = 0.0f;
    public bool isDamaging = false;
    public bool isDamagedPlayer = false;
    public int continueCount; //���� ����, 1�� ���� �� (1Ÿ -> 2Ÿ) x 1  , 2�� ���� �� (1Ÿ -> 2Ÿ) x 2

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
        UpdateHealthUI(); // UI ������Ʈ

        if(bm != null)
        {
            if (bm.RepeatCount >= continueCount)
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
        if (currentMP <= float.Epsilon)
        {
            isMPzero = true;
        }
        Debug.Log("GraplingValue: " + currentMP);
        GraplingSliderUI(); // UI ������Ʈ
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
