using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum PpPaze
{
    STATE_PATROL,
    STATE_ROCKATTACK,
    STATE_STOP,
    STATE_FOLLOW,
    STATE_DIE,
}
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

    public GameObject TelePortEffect;
    private GameObject TpEffectPrefab;
    public float TelePortDel; //����Ʈ ������ ������. �� �ð��� ������ �����̵�
    IEnumerator FadeInSprite()
    {
        isAttackStart = false;

        Color color = PpSpriteRenderer.color;
        color.a = 0;
        PpSpriteRenderer.color = color;
        if(TpEffectPrefab == null)
        {
             TpEffectPrefab = Instantiate(TelePortEffect, transform.position, Quaternion.identity);
        }
      
        yield return new WaitForSeconds(TelePortDel);
        Destroy(TpEffectPrefab);
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

        GraplingPlayerNockBack();
        UpdateGraplingCooldown();

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

    public GameObject CreateEffect;
    private GameObject CtEffectPrefab;

    public float ZaKoomCreateDel; //�� �� �ڿ� ���� ���� �����Ұ��� ,�� �ʰ� ������ ���� �� ����
    public float ZaKoomEffectDel; //�� �� �ڿ� ���� �� ��ȯ ����Ʈ�� �����ٰ���, �� �ʰ� ������ ���� �� ��ȯ ����Ʈ ����
    IEnumerator ZaKoomCreate() //�⺻����2 : ���� �� ��ȯ .. ����?
    {
        isAttackStart = false; //false�� �����Ͽ� �� �ڷ�ƾ ���� �� ������ 1���� ���� Update������ ȣ�� x -> true�� ���� �� ��� �߻� ����.
        yield return new WaitForSeconds(ZaKoomEffectDel);
        if(CtEffectPrefab == null)
        {
            Debug.Log("���� �� ��ȯ ����Ʈ");
            CtEffectPrefab = Instantiate(CreateEffect, transform.GetChild(0).position, Quaternion.identity);
          
        }

        yield return new WaitForSeconds(ZaKoomCreateDel);
        Destroy(CtEffectPrefab);
        Debug.Log("���� �� ��ȯ ����Ʈ ����");
        if (ZaKoomPrefab == null)
        {
            Debug.Log("���� �� ��ȯ");
            ZaKoomPrefab = Instantiate(ZaKoomObj, transform.GetChild(0).position, Quaternion.identity);
        }
        
        isCreateZakoom = true;
        //���� �� ��ȯ
        yield return null;
    }

   
    public float nockbackForce;
    public bool isGraplingCooldown;
    public float graplingCooldownTimer; // ��Ÿ�� Ÿ�̸�
    public float graplingCooldownDuration = 5f; // ��Ÿ�� ���� �ð�
    void GraplingPlayerNockBack() //�⺻����3: ���� �׷��ø� ��ų ����ϸ� ���ĳ��� (��Ÿ�� ����) 
    {
        if (isGraplingCooldown)
        {
            Debug.Log("��Ÿ�� ���Դϴ�.");
            return; // �ƹ��͵� ������ϰ� ��ȯ
        }
        //���ĳ��� �ڵ� ����
        Grapling grapling = FindAnyObjectByType<Grapling>();
        if (grapling.isLerping)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, Mathf.Infinity, playerLayer);
            Debug.DrawRay(transform.position, Vector2.right * ShootDistance, Color.blue);
            if (hit.collider != null)
            {
                float playerdistance = Mathf.Abs(hit.point.x - transform.position.x);

                if (playerdistance < ShootDistance)
                {
                    PlayerControllerRope playerController = FindAnyObjectByType<PlayerControllerRope>();
                    if (playerController != null)
                    {
                        StartCoroutine(GraplingKnockback(playerController));
                        
                    }
                }
            }
        }
        else
        {
            Debug.Log("���� �׷��ø� ���� ����");
        }
        

        //DragonShoot(); //���ĳ� ������ ��� �߻�.
    }
    IEnumerator GraplingKnockback(PlayerControllerRope playerController)
    {
        yield return playerController.BMSkillMove(transform, nockbackForce);
        yield return new WaitForSeconds(0.5f);
        DragonShoot();
        StartGraplingCooldown();
    }
    void StartGraplingCooldown()
    {
        isGraplingCooldown = true; // ��Ÿ�� ����
        graplingCooldownTimer = graplingCooldownDuration; // Ÿ�̸� �ʱ�ȭ
    }

    void UpdateGraplingCooldown()
    {
        if (isGraplingCooldown)
        {
            graplingCooldownTimer -= Time.deltaTime; // Ÿ�̸� ����
            if (graplingCooldownTimer <= 0)
            {
                isGraplingCooldown = false; // ��Ÿ�� ����
                graplingCooldownTimer = 0;
            }
        }
    }
}
