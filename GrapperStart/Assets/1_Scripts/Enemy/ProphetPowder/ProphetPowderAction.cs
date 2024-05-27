using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProphetPowderAction : MonoBehaviour
{
    // 근거리 사용 무기: 척력
    //원거리 사용 무기: 인력, 용언
    SpriteRenderer PpSpriteRenderer;
    void Start()
    {
        isAttackStart = false;
        PpSpriteRenderer = GetComponent<SpriteRenderer>();
        //맵 한 가운데에서 순간이동을 하며 등장
        StartCoroutine(FadeInSprite());
    }

    public bool isAttackStart;
    IEnumerator FadeInSprite()
    {
        Color color = PpSpriteRenderer.color;
        color.a = 0;
        PpSpriteRenderer.color = color;

        while (color.a < 1)
        {
            color.a += Time.deltaTime;
            PpSpriteRenderer.color = color;
            yield return null;
        }

        color.a = 1;
        PpSpriteRenderer.color = color;

        isAttackStart = true;
    }
    void Update()
    {
        if(isAttackStart)
        {
            DragonShoot();
        }
       
    }

    public LayerMask playerLayer;
    public float ShootDistance;
    public float projectileSpeed;


    public GameObject DragonWeapon;
    private GameObject DragonWeaponPrefab;
    void DragonShoot() //기본공격1 : 용언 발사
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, Mathf.Infinity, playerLayer);
        Debug.DrawRay(transform.position, Vector2.right * ShootDistance, Color.red);

        if (hit.collider != null)
        {
            float playerdistance = Mathf.Abs(hit.point.x - transform.position.x);

            if (playerdistance < ShootDistance)
            {
                //용언 발사
                if(DragonWeaponPrefab == null)
                {
                    DragonWeaponPrefab = Instantiate(DragonWeapon, transform.GetChild(1).position, Quaternion.identity);
                    Rigidbody2D rb = DragonWeaponPrefab.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.velocity = Vector2.right * projectileSpeed;
                    }
                }
               
                if (isCreateZakoom == false)
                {
                    StartCoroutine(ZaKoomCreate());
                }
                
            }
        }
    }

    public bool isCreateZakoom;
    public GameObject ZaKoomObj;
    private GameObject ZaKoomPrefab;
    IEnumerator ZaKoomCreate() //기본공격2 : 자쿰 돌 소환 .. 폭파?
    {
        ZaKoomPrefab = Instantiate(ZaKoomObj, transform.GetChild(0).position, Quaternion.identity);
        isCreateZakoom = true;
        //자쿰 돌 소환
        yield return null;
    }

    void GraplingPlayerNockBack() //기본공격3: 적이 그래플링 스킬 사용하면 밀쳐내기 (쿨타임 존재) 
    {
        //밀쳐내기 코드 구현


        DragonShoot(); //밀쳐낸 적에게 용언 발사.
    }
}
