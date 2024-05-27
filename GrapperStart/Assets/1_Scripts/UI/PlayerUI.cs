using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

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
    public float mpUpTime;



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

        InvokeRepeating("PlayerMpUp", 0.1f, mpUpTime);
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
            else if (bm.RepeatCount == float.Epsilon)
            {
                isDamaging = false;
            }

            
        }

        if (isDamaging)
        {
            elapsedTime += Time.deltaTime;
            damageTimer += Time.deltaTime;

            if (damageTimer >= damageInterval)
            {
                Debug.Log("�޷��");
                StartCoroutine(Firedamage(0.1f));
                damageTimer = 0.0f;
            }

            if (elapsedTime >= totalDuration)
            {
                isDamaging = false;
            }
        }
      
    }

    public bool isDead;

    IEnumerator Firedamage(float damage)
    {
        yield return ApplyDamage(damage, 0.1f);
    }

    public void TakeDamage(float damage)
    {
        
        StartCoroutine(ApplyDamage(damage));
    }

    private IEnumerator ApplyDamage(float damage, float delay = 0f)
    {
        isDamagedPlayer = true;
        currentHP -= damage;
         currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

        if (Mathf.RoundToInt(currentHP) > 0)
        {
            player.animatorPlayer.SetTrigger("PlayerHit");
        }
        else
        {
            isDamaging = false;
            PlayerDead();
        }

        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }

        isDamagedPlayer = false;
    }
    public void PlayerDead()
    {
        Debug.Log("a");
        player.isMoveStop = true;
        player.tag = "Untagged";
        player.gameObject.layer = 8;
        player.animatorPlayer.SetTrigger("PlayerDeath");
        PlayerDelete();
    }
    public GameObject DeathUI;
    private GameObject DeathUIPrefab;
    public void PlayerDelete()
    {
        GameObject uiCanvas = GameObject.Find("UI_Canvas");

        if (uiCanvas != null)
        {
            DeathUIPrefab = Instantiate(DeathUI, new Vector3(0,0,0), Quaternion.identity);
            // comboBarUI�� UI_Canvas�� ������ ����
            DeathUIPrefab.transform.SetParent(uiCanvas.transform, false);
        }
        else
        {
            Debug.Log("����");
        }
        //mn  Destroy(this.gameObject);
    }
    public GameObject Lava;
    private GameObject LavaPrefab;
    public Vector3 Lavaoffset;
    public void CreateLava()
    {

        Vector3 spawnPosition = transform.position + Vector3.down + Lavaoffset;
        LavaPrefab = Instantiate(Lava, spawnPosition, Quaternion.identity);
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
        GraplingSliderUI(); // UI ������Ʈ
    }

    void GraplingSliderUI()
    {
        sliderMP.value = currentMP; // Update sliderMP based on currentMP and maxMP

    }

    void PlayerMpUp()
    {
        if (currentMP < maxMP)
        {
            Debug.Log("������ 1����");
            currentMP += 1.0f;
            currentMP = Mathf.Clamp(currentMP, 0f, maxMP);
            GraplingSliderUI(); // Update MP slider UI
        }
        else
        {
            Debug.Log("������ ����");
        }
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
