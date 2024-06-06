using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using static UnityEditor.PlayerSettings;
public enum PpPaze
{
    STATE_None,
    STATE_Basic,
    STATE_PazeOne,
    STATE_PazeTwo,
    STATE_PazeThree,
    STATE_PazeFour,
}
public class ProphetPowderAction : MonoBehaviour
{
    // �ٰŸ� ��� ����: ô��
    //���Ÿ� ��� ����: �η�, ���
    public PpPaze ppState;
    SpriteRenderer PpSpriteRenderer;
    Transform playerObject;
    PlayerControllerRope playerController;
    Grapling grapling;

    private Transform playerTransform;

    [Header("##TelePort")]
    public GameObject TelePortEffect;
    private GameObject TpEffectPrefab;
    public float TelePortDel; // ����Ʈ ������ ������. �� �ð��� ������ �����̵�
    [Header("##First Move")]
    public bool isMoving;
    public float chargeSpeed = 5f;
    public float moveSpeed;
    public float moveDistance = 5.0f; // �̵��� �Ÿ�

    [Header("##Chaged")]
    public bool isRush = false;
    public float chargeMaxDistance = 5f; // ���� ������ �ִ� �Ÿ�
    public float chargeDistace = 5f; // ���� ���� �Ÿ�.
    public float isChargeDel = 1f; // ���� ������
    private Vector2 lastPlayerPos;
    public bool isChaging;

    


    [Header("##GraplingNockBackAtk")]
    public float nockbackForce;
    public bool isGraplingCooldown;
    public float graplingCooldownTimer; // ��Ÿ�� Ÿ�̸�
    public float graplingCooldownDuration = 5f; // ��Ÿ�� ���� �ð�
    public bool isAttackStart;
    public LayerMask playerLayer;
    public float ShootDistance;

    [Header("##DragonWeaponAtk")]
    private IObjectPool<ProPhetWeapon> _Pool;
    public Transform bounsing;
    public Vector3 bounsingSize;
    public bool isAttack = false;
    public float meleeAttackForce;
    public GameObject DragonWeapon;
    private GameObject DragonWeaponPrefab;
    public int activeDragonWeapons = 0; // Ȱ��ȭ�� ��� ������Ʈ ���� �����ϱ� ���� ����
    public int maxDragonWeapons = 8; // �ִ� ��� ������Ʈ ��
    public bool isSpawningDragonWeapon = false; // ��� ��ȯ ������ Ȯ���ϴ� �÷���
    private bool spawnInCircle = true;

    private void Awake()
    {
        _Pool = new ObjectPool<ProPhetWeapon>(CreateWeapon, OnGetWeapon, OnReleaseWeapon, OnDestroyWeapon, maxSize: 6);
    }
    void Start()
    {
        this.ppState = PpPaze.STATE_None;
        playerController = FindAnyObjectByType<PlayerControllerRope>();
        grapling = FindAnyObjectByType<Grapling>();
        playerObject = GameObject.Find("Player").transform;
        PpSpriteRenderer = GetComponent<SpriteRenderer>();

        isAttackStart = false;

        isMoving = false;

        if (ppState == PpPaze.STATE_None)
        {
            StartCoroutine(FadeInSprite());
        }
    }

    IEnumerator FadeInSprite()
    {
        PpDirection();

        isAttackStart = false;

        Color color = PpSpriteRenderer.color;
        color.a = 0;
        PpSpriteRenderer.color = color;
        if (TpEffectPrefab == null)
        {
            TpEffectPrefab = Instantiate(TelePortEffect, transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(TelePortDel);
        Destroy(TpEffectPrefab);

        lastPlayerPos = playerObject.transform.position;
        while (color.a < 1)
        {
            color.a += Time.deltaTime;
            PpSpriteRenderer.color = color;
            yield return null;
        }

        color.a = 1;
        PpSpriteRenderer.color = color;

        this.ppState = PpPaze.STATE_Basic;
        Debug.Log("State changed to STATE_Basic");
        isRush = true;
    }
   

    IEnumerator PpMove()
    {
        if (isMoving)
        {
            yield break;
        }

        isMoving = true;

        float elapsedTime = 0f;
        Vector2 initialPosition = (Vector2)transform.position; // ��������� ĳ�����Ͽ� Vector2�� ��ȯ

        // Calculate the target position only on the x-axis
        Vector2 direction = new Vector2((lastPlayerPos.x - initialPosition.x), 0).normalized;
        Vector2 targetPosition = initialPosition + direction * moveDistance;

        while (elapsedTime < 1.0f)
        {
            transform.position = Vector2.Lerp(initialPosition, targetPosition, elapsedTime * moveSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition; // Ensure the final position is exact

        this.ppState = PpPaze.STATE_PazeOne;
        isRush = true;
        isMoving = false;
    }



    void Update()
    {

        GraplingPlayerNockBack(); // 1. �÷��̾ �׷��ø��� ������ �Ŀ������ ���� ���� �����
        UpdateGraplingCooldown(); // 2. �÷��̾ ��ġ�� ������ ����Ǹ�, ��Ÿ�� �޼�����.

       
        UpdateChargingCooldown();

        switch (this.ppState)
        {
            case PpPaze.STATE_Basic:
                if(grapling.isAttatch == false)
                {
                    StartCoroutine(PpMove());
                }
               
                break;
            case PpPaze.STATE_PazeOne:
                if (isRush == true && grapling.isAttatch == false && !isChaging)
                {
                    StartCoroutine(ChargeToPlayer());
                }
                else if (!isRush && isDragonAtkReady)
                {
                    MeleeAttack();
                }

                break;
                // �ʿ��ϴٸ� �ٸ� ���� �߰�
        }


    }



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
                        StartCoroutine(GraplingKnockback(playerController, nockbackForce));

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
    IEnumerator GraplingKnockback(PlayerControllerRope playerController, float nockbackForce)
    {
        lastPlayerPos = playerObject.transform.position;
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

    public bool isChargingCooldown;
    public float chargingCoolTimer = 5f; // ��Ÿ�� Ÿ�̸�
    public float chargingCoolTimerDur;// ��Ÿ�� ���� �ð�
    void StartChargingCooldown()
    {
        isChargingCooldown = true; // ��Ÿ�� ����
        chargingCoolTimer = chargingCoolTimerDur; // Ÿ�̸� �ʱ�ȭ
    }

    void UpdateChargingCooldown()
    {
        if (isChargingCooldown)
        {
            chargingCoolTimer -= Time.deltaTime; // Ÿ�̸� ����
            if (chargingCoolTimer <= 0)
            {
                isChargingCooldown = false; // ��Ÿ�� ����
                chargingCoolTimer = 0;
                isRush = true;
            }
        }
    }

    public bool isDragonAtkReady;
    IEnumerator ChargeToPlayer()
    {
        if (isChargingCooldown)
        {
            Debug.Log("���� ��Ÿ�� ���Դϴ�.");
            yield break;
        }
        isChaging = true; // ���� ����
        chargeSpeed = 0.0f;
        //���� �غ� �ִϸ��̼� ���. 
        Debug.Log("���� �غ� �ִϸ��̼� ���");
        yield return new WaitForSeconds(2.0f);
        Debug.Log("���� �ִϸ��̼� ���");
    
        isDragonAtkReady = false;
        chargeSpeed = 0.5f;
        float elapsedTime = 0.0f;

        Vector3 currentPosition = transform.position;
        Vector3 targetPosition = currentPosition + (Vector3)PpDir;

        while (elapsedTime < chargeSpeed)
        {
            isChaging = true;
            transform.position = Vector3.Lerp(currentPosition, targetPosition, elapsedTime / chargeSpeed);
            elapsedTime += Time.deltaTime;



            yield return null;
        }

        isRush = false;
        isDragonAtkReady = true;
        transform.position = targetPosition;
        isChaging = false; // ���� ����
        StartChargingCooldown();
    }


    void MeleeAttack()
    {
        if (isGraplingCooldown)
        {
            Debug.Log("��Ÿ�� ���Դϴ�.");
            return;
        }

        Debug.Log("ô�°��� �ִϸ��̼� ���");

        isAttack = false;
        Collider2D[] coliderMelee = Physics2D.OverlapBoxAll(bounsing.position, bounsingSize, 0);

        foreach (Collider2D collider in coliderMelee)
        {
            if (collider.CompareTag("Player"))
            {
                isAttack = true;
            }
        }


        if (playerController != null && isAttack)
        {
            Debug.Log("���� ������ ���� -> �˹�");
            StartCoroutine(GraplingKnockback(playerController, meleeAttackForce));
        }
        else if (isAttack == false && isDragonAtkReady == true)
        {
            lastPlayerPos = playerObject.transform.position;

            if (isSpawningDragonWeapon == false)
            {
                isAttackStart = true;
                isSpawningDragonWeapon = true;
                StartCoroutine(DragonWeaponSpawner());
            }

            Debug.Log("���� ������ ����");
        }

    }


    void DragonWeaponFunc()
    {
        if (activeDragonWeapons + 8 <= maxDragonWeapons)
        {
            Debug.Log("��� ����");

            Vector3 spawnPosition = (Vector3)lastPlayerPos;
            if (spawnInCircle)
            {
                SpawnInCircle(spawnPosition);
            }
            else
            {
                SpawnInSquare(spawnPosition);
            }
        }
        else
        {
            Debug.Log("�ִ� ��� ������ ����");
            ReleaseDragonWeapons();
        }
    }

    void SpawnInCircle(Vector3 spawnPosition)
    {
        float radius = 1f; // ���� ������
        int weaponCount = 8; // �� ���� ������ ���� ����

        for (int i = 0; i < weaponCount; i++)
        {
            float angle = i * Mathf.PI * 2f / weaponCount; // �� ������ ����
            float x = spawnPosition.x + Mathf.Cos(angle) * radius;
            float y = spawnPosition.y + Mathf.Sin(angle) * radius;

            Vector3 position = new Vector3(x, y, 0f);
            var weapon = _Pool.Get();
            weapon.transform.position = position;
            activeDragonWeapons++;
        }
        spawnInCircle = false; // ���� ������ �簢������ ����
    }

    void SpawnInSquare(Vector3 spawnPosition)
    {
        float halfSize = 1.0f; // �簢���� �� ũ��
        int weaponCount = 8; // �� ���� ������ ���� ����
        Vector3[] offsets = new Vector3[]
        {
            new Vector3(-halfSize, halfSize, 0),
            new Vector3(0, halfSize, 0),
            new Vector3(halfSize, halfSize, 0),
            new Vector3(-halfSize, 0, 0),
            new Vector3(halfSize, 0, 0),
            new Vector3(-halfSize, -halfSize, 0),
            new Vector3(0, -halfSize, 0),
            new Vector3(halfSize, -halfSize, 0)
        };

        for (int i = 0; i < weaponCount; i++)
        {
            Vector3 position = spawnPosition + offsets[i];
            var weapon = _Pool.Get();
            weapon.transform.position = position;
            activeDragonWeapons++;
        }
        spawnInCircle = true; // ���� ������ �������� ����
    }

    void ReleaseDragonWeapons()
    {
        for (int i = 0; i < 8; i++)
        {
            var weapon = FindActiveDragonWeapon(); // Ȱ��ȭ�� ��� ������Ʈ�� ã�Ƽ� ������
            if (weapon != null)
            {
                _Pool.Release(weapon);
                activeDragonWeapons--;
            }
        }
    }

    ProPhetWeapon FindActiveDragonWeapon()
    {
        
        ProPhetWeapon[] weapons = FindObjectsOfType<ProPhetWeapon>();
        foreach (var weapon in weapons)
        {
            if (weapon.gameObject.activeInHierarchy)
            {
                return weapon;
            }
        }
        return null;
    }

    IEnumerator DragonWeaponSpawner()
    {
        yield return new WaitForSeconds(3.0f);
        while (true)
        {
            DragonWeaponFunc();
            yield return new WaitForSeconds(2.0f); // 2�� �������� 8���� ����
        }
    }

    private ProPhetWeapon CreateWeapon()
    {
        ProPhetWeapon weapon = Instantiate(DragonWeapon).GetComponent<ProPhetWeapon>();
        weapon.SetManagedPool(_Pool);
        return weapon;
    }

    private void OnGetWeapon(ProPhetWeapon weapon)
    {
        weapon.gameObject.SetActive(true);
    }

    private void OnReleaseWeapon(ProPhetWeapon weapon)
    {
        weapon.gameObject.SetActive(false);
    }

    private void OnDestroyWeapon(ProPhetWeapon weapon)
    {
        Destroy(weapon.gameObject);
    }
    public Vector2 PpDir;

    void PpDirection()
    {
        playerTransform = playerObject.transform;
        if (playerTransform.position.x > transform.position.x)
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
        Gizmos.DrawWireCube(bounsing.position, bounsingSize);
    }
}
