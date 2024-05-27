using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProphetPowderAction : MonoBehaviour
{
    // �ٰŸ� ��� ����: ô��
    //���Ÿ� ��� ����: �η�, ���
    SpriteRenderer PpSpriteRenderer;
    void Start()
    {
        isAttackStart = false;
        PpSpriteRenderer = GetComponent<SpriteRenderer>();
        //�� �� ������� �����̵��� �ϸ� ����
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
    void DragonShoot() //�⺻����1 : ��� �߻�
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, Mathf.Infinity, playerLayer);
        Debug.DrawRay(transform.position, Vector2.right * ShootDistance, Color.red);

        if (hit.collider != null)
        {
            float playerdistance = Mathf.Abs(hit.point.x - transform.position.x);

            if (playerdistance < ShootDistance)
            {
                //��� �߻�
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
    IEnumerator ZaKoomCreate() //�⺻����2 : ���� �� ��ȯ .. ����?
    {
        ZaKoomPrefab = Instantiate(ZaKoomObj, transform.GetChild(0).position, Quaternion.identity);
        isCreateZakoom = true;
        //���� �� ��ȯ
        yield return null;
    }

    void GraplingPlayerNockBack() //�⺻����3: ���� �׷��ø� ��ų ����ϸ� ���ĳ��� (��Ÿ�� ����) 
    {
        //���ĳ��� �ڵ� ����


        DragonShoot(); //���ĳ� ������ ��� �߻�.
    }
}
