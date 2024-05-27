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

        InvokeRepeating("PlayerMpUp", 0.1f, mpUpTime);
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
                Debug.Log("쭈루룩");
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
            // comboBarUI를 UI_Canvas의 하위로 설정
            DeathUIPrefab.transform.SetParent(uiCanvas.transform, false);
        }
        else
        {
            Debug.Log("없음");
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
        GraplingSliderUI(); // UI 업데이트
    }

    void GraplingSliderUI()
    {
        sliderMP.value = currentMP; // Update sliderMP based on currentMP and maxMP

    }

    void PlayerMpUp()
    {
        if (currentMP < maxMP)
        {
            Debug.Log("에너지 1충전");
            currentMP += 1.0f;
            currentMP = Mathf.Clamp(currentMP, 0f, maxMP);
            GraplingSliderUI(); // Update MP slider UI
        }
        else
        {
            Debug.Log("에너지 꽉참");
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
