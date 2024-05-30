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
    Transform playerObject;
    void Start()
    {
        playerObject = GameObject.Find("Player").transform;
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
        FindPlayer();


    }
    void Update()
    {
        if(isAttackStart)
        {
            //DragonShoot();
        }

        GraplingPlayerNockBack(); //1. �÷��̾ �׷��ø��� ������ �Ŀ������ ���� ���� �����
        UpdateGraplingCooldown(); //2. �÷��̾ ��ġ�� ������ ����Ǹ�,��Ÿ�� �޼�����.

        if (isRush)
        {
            StartCoroutine(ChargeToPlayer());
            //ChargeToPlayer();
        }
    }

    public LayerMask playerLayer;
    public float ShootDistance;
    public float projectileSpeed;


    public GameObject DragonWeapon;
    private GameObject DragonWeaponPrefab;
    void DragonShoot() //�⺻����1 : ��� �߻�
    {
        PpDirection();

        RaycastHit2D hit = Physics2D.Raycast(transform.position, PpDir, Mathf.Infinity, playerLayer);
        Debug.DrawRay(transform.position, PpDir * ShootDistance, Color.red);

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
                        StartCoroutine(GraplingKnockback(playerController,nockbackForce));
                        
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
    IEnumerator GraplingKnockback(PlayerControllerRope playerController,float nockbackForce)
    {
        yield return playerController.BMSkillMove(transform, nockbackForce);
        //DragonShoot();
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





    public float chargeSpeed = 5f;
    private Transform playerTransform;
    public bool isRush = false;

    public float chargeMaxDistance = 5f; // ���� ������ �ִ� �Ÿ�
    public float chargeDistace = 5f; // ���� ���� �Ÿ�.

    public float isChargeDel = 1f; // ���� ������

    void FindPlayer()
    {
        isRush = true;

        lastPlayerPos = playerObject.transform.position;
    }

    public float UnChargeTime;
    public float NotChargeTime;
    public Vector3 lastPlayerPos;
    public bool isMove;
    IEnumerator ChargeToPlayer()
    {
        
        PpDirection();
        //���� �غ� �ִϸ��̼� ��� �ڵ� �����. 
        yield return new WaitForSeconds(isChargeDel);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, PpDir, chargeDistace,playerLayer);
        Debug.DrawRay(transform.position,PpDir * chargeDistace, Color.red);

        float playerdistance = Mathf.Abs(lastPlayerPos.x - transform.position.x);

        if (hit.collider != null)
        {

            //while (hit.collider.CompareTag("Player") && elapsedTime < 3.0f)
            //{
            //    transform.position = Vector2.Lerp(transform.position, lastPlayerPos , elapsedTime);
            //    elapsedTime += Time.deltaTime * chargeSpeed;

            //    yield return null;
            //}


            if (playerdistance > chargeMaxDistance)
            {
                isMove = true;
                Debug.Log("�����մϴ�.");
                transform.position += (Vector3)PpDir * chargeSpeed * Time.deltaTime;
            }
            else
            {
                MeleeAttack();
                playerdistance = 0.0f;
                //ô�� ���� �޼��� ȣ�� �����.
                Debug.Log("�������� �ʽ��ϴ�.");
            }


        }
       
        
        
        else if(hit.collider == null)
        {
            
            UnChargeTime += Time.deltaTime;
            if (UnChargeTime > NotChargeTime)
            {
                //�⺻ ���� �ִϸ��̼� 
                isRush = false;
                isMove = false;
                UnChargeTime = 0;

                Invoke("FindPlayer", 3.0f);
            }
            Debug.Log("hit�浹x");
        }

    }

    public Transform bounsing;
    public Vector3 bounsingSize;
    public bool isAttack = false;
    public float meleeAttackForce;
    void MeleeAttack()
    {
        if (isGraplingCooldown)
        {
            Debug.Log("��Ÿ�� ���Դϴ�.");
            return; // �ƹ��͵� ������ϰ� ��ȯ
        }

        isAttack = false;
        Collider2D[] coliderMelee = Physics2D.OverlapBoxAll(bounsing.position, bounsingSize, 0);

        foreach(Collider2D collider in coliderMelee)
        {
            if(collider.CompareTag("Player"))
            {
                isAttack = true;
            }
        }

        if(isAttack)
        {
            //ô�°��� �ִϸ��̼� ���
            Debug.Log("���� ������ ���� -> �˹�");
            PlayerControllerRope playerController = FindAnyObjectByType<PlayerControllerRope>();
            if (playerController != null)
            {
                StartCoroutine(GraplingKnockback(playerController, meleeAttackForce));

               

            }

        }
        else
        {
            Debug.Log("���� ������ ����");
        }
    }




    public Vector2 PpDir;
    void PpDirection()
    {
        playerTransform = playerObject.transform;
        if(playerTransform.transform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
            PpDir = Vector2.right;
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
            PpDir = Vector2.left;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireCube(bounsing.position, bounsingSize);//DrawWireCube(pos.position,boxsize)      
    }
}
