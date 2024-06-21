using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Rendering;
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
public class ProphetPowderAction : MonoBehaviour,Enemies
{
    // �ٰŸ� ��� ����: ô��
    //���Ÿ� ��� ����: �η�, ���
    public PpPaze ppState;
    SpriteRenderer PpSpriteRenderer;
    Transform playerObject;
    Animator PpAnim;
    PlayerControllerRope playerController;
    Grapling grapling;
    private Transform playerTransform;

    private IObjectPool<PpZakoom> _zacomPool;
    private IObjectPool<PpBoom> _explosionPool;
    private IObjectPool<ProPhetWeapon> _Pool;

    [Header("##�����̵�")]
    public GameObject TelePortEffect;
    private GameObject TpEffectPrefab;
    public float TelePortDel; // ����Ʈ ������ ������. �� �ð��� ������ �����̵�
    [Header("##�⺻ �̵�")]
    public bool isMoving;
    public float moveSpeed;
    public float moveDistance = 5.0f; // �̵��� �Ÿ�

    [Header("##���� ���� ����")]
    public bool isRush = false;
    public float chargeMaxDistance = 5f; // ���� ������ �ִ� �Ÿ�
    public float chargeDistace = 5f; // ���� ���� �Ÿ�.
    public float isChargeDel = 1f; // ���� ������
    private Vector2 lastPlayerPos;
    public bool isChaging;
    public float chargeSpeed = 5f;
    public bool isChargingCooldown;
    public float chargingCoolTimer = 5f; // ��Ÿ�� Ÿ�̸�
    public float chargingCoolTimerDur;// ��Ÿ�� ���� �ð�
    [Header("##���� �׷��ø��Ͽ� ����� �� �ʿ��� ����")]
    public float nockbackForce;
    public bool isGraplingCooldown;
    public float graplingCooldownTimer; // ��Ÿ�� Ÿ�̸�
    public float graplingCooldownDuration = 5f; // ��Ÿ�� ���� �ð�

    public LayerMask playerLayer;
    public float ShootDistance;

    [Header("##��� ������ �ϱ� ���� ������ �ʿ� ������Ʈ")]
    public Transform bounsing;
    public Vector3 bounsingSize;
    public bool isAttack = false;
    public float meleeAttackForce;
    public GameObject DragonWeapon;
    private GameObject DragonWeaponPrefab;
    public GameObject ZacomPrefab;
    public GameObject ExplosionPrefab;
    public int activeDragonWeapons = 0; // Ȱ��ȭ�� ��� ������Ʈ ���� �����ϱ� ���� ����
    public bool isSpawningDragonWeapon = false; // ��� ��ȯ ������ Ȯ���ϴ� �÷���
    public bool spawnShapeCircle = true;
    public bool isDragonAtkReady;

    [Header("##��� ���ϵ��� ������� ����")]
    public float CircleRadius;
    public int CircleCount;
    public float squareSize;
    public int squareCount;
    [Header("##���,���� �� , ���� �� ��ź ����Ʈ ������")]
    public float WeaponLifeTime;
    public float ZakoomLifeTime;
    public float BoomEffLifeTime;
    public bool isDragonAttackCooldown = false;
    public float dragonAttackCooldownDuration = 10f;
    public float dragonAttackCooldownTimer = 0f;


    private void Awake()
    {
        _zacomPool = new UnityEngine.Pool.ObjectPool<PpZakoom>(CreateZacom, OnGetZacom, OnReleaseZacom, OnDestroyZacom, maxSize: 10);
        _explosionPool = new UnityEngine.Pool.ObjectPool<PpBoom>(CreateExplosion, OnGetExplosion, OnReleaseExplosion, OnDestroyExplosion, maxSize: 10);
        _Pool = new UnityEngine.Pool.ObjectPool<ProPhetWeapon>(CreateWeapon, OnGetWeapon, OnReleaseWeapon, OnDestroyWeapon, maxSize: 6); // ��������� UnityEngine.Pool ���ӽ����̽��� ����
    }
    private PpZakoom CreateZacom()
    {
        PpZakoom zacom = Instantiate(ZacomPrefab).GetComponent<PpZakoom>();
        zacom.SetManagedPool(_zacomPool);
        return zacom;
    }

    private void OnGetZacom(PpZakoom zacom)
    {
        zacom.gameObject.SetActive(true);
    }

    private void OnReleaseZacom(PpZakoom zacom)
    {
        zacom.gameObject.SetActive(false);
    }

    private void OnDestroyZacom(PpZakoom zacom)
    {
        Destroy(zacom.gameObject);
    }

    private PpBoom CreateExplosion()
    {
        PpBoom explosion = Instantiate(ExplosionPrefab).GetComponent<PpBoom>();
        explosion.SetManagedPool(_explosionPool);
        return explosion;
    }

    private void OnGetExplosion(PpBoom explosion)
    {
        explosion.gameObject.SetActive(true);
    }

    private void OnReleaseExplosion(PpBoom explosion)
    {
        explosion.gameObject.SetActive(false);
    }

    private void OnDestroyExplosion(PpBoom explosion)
    {
        Destroy(explosion.gameObject);
    }
    void Start()
    {
        this.ppState = PpPaze.STATE_None;
        playerController = FindAnyObjectByType<PlayerControllerRope>();
        grapling = FindAnyObjectByType<Grapling>();
        playerObject = GameObject.Find("Player").transform;
        PpSpriteRenderer = GetComponent<SpriteRenderer>();
        PpAnim = GetComponent<Animator>();

        PpAnim.enabled = false;
        isMoving = false;

        if (ppState == PpPaze.STATE_None)
        {
            StartCoroutine(FadeInSprite());
        }
    }

    IEnumerator FadeInSprite()
    {
        Color color = PpSpriteRenderer.color;
        color.a = 0;
        PpSpriteRenderer.color = color;
        if (TpEffectPrefab == null)
        {
            TpEffectPrefab = Instantiate(TelePortEffect, transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(TelePortDel);
        Destroy(TpEffectPrefab);

        PpAnim.enabled = true;
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
        PpAnim.Play("PP_Move");
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
        PpAnim.Play("PP_Idle");
    }

    void Set() //������ �Ѿ �� �ʱ�ȭ �ϴ� �Լ�.
    {
        isRush = false;
        isMoving = false;
        isChaging = false;
        isDragonAtkReady = false;
        isMeleeAttack = false;
        isSpawningDragonWeapon = false;
        lastPlayerPos = Vector2.zero;
        Debug.Log("�ʱ�ȭ");
    }

    void Update()
    {

        GraplingPlayerNockBack(); // 1. �÷��̾ �׷��ø��� ������ �Ŀ������ ���� ���� �����
        
        UpdateGraplingCooldown(); // 2. �÷��̾ ��ġ�� ������ ����Ǹ�, ��Ÿ�� �޼�����.
        UpdateDragonAttackCooldown();
        UpdateMeleeAttackCooldown();

        switch (this.ppState)
        {
            case PpPaze.STATE_Basic:
                if(grapling.isAttatch == false)
                {
                    StartCoroutine(PpMove());
                }
               
                break;
            case PpPaze.STATE_PazeOne:
                if (isRush == true && grapling.isAttatch == false && isChaging == false)
                {
                    StartCoroutine(ChargeToPlayer());
                }
                else if (isRush ==false && isMeleeAttack == true)
                {
                    MeleeAttack();
                }
                break;
            case PpPaze.STATE_PazeTwo:          
                Debug.Log("2������");
                break;
                // �ʿ��ϴٸ� �ٸ� ���� �߰�
        }

        PpDirection();
    }



    void GraplingPlayerNockBack() //�⺻����3: ���� �׷��ø� ��ų ����ϸ� ���ĳ��� (��Ÿ�� ����) 
    {
        if (isGraplingCooldown)
        {
            return; // �ƹ��͵� ������ϰ� ��ȯ
        }
        //���ĳ��� �ڵ� ����
        
        if (grapling.isLerping)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, Mathf.Infinity, playerLayer);
            Debug.DrawRay(transform.position, Vector2.right * ShootDistance, Color.blue);
            if (hit.collider != null)
            {
                float playerdistance = Mathf.Abs(hit.point.x - transform.position.x);

                if (playerdistance < ShootDistance)
                {
                    StartCoroutine(GraplingKnockback(playerController, nockbackForce, "Grapling"));
                }
            }
        }
        else
        {
            Debug.Log("���� �׷��ø� ���� ����");
        }

    }
    public bool isKnockbackInProgress = false;
    IEnumerator GraplingKnockback(PlayerControllerRope playerController, float nockbackForce,string attackWay)
    {
        PpAnim.SetTrigger("PPNockBackSkill");
        lastPlayerPos = playerObject.transform.position;
        yield return playerController.BMSkillMove(transform, nockbackForce);
        PpAnim.SetBool("PPMeleeAtk", true);
        //DragonShoot();
        if (attackWay == "Grapling")
        {
            Debug.Log("�׷��ø� �˹�");
            StartGraplingCooldown();
        }
        else if (attackWay == "MeleeAttack")
        {
            Debug.Log("ô�� �˹�");
            StartMeleeAttackCooldown();
        }

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

    void UpdateDragonAttackCooldown()
    {
        if (isDragonAttackCooldown)
        {
            dragonAttackCooldownTimer -= Time.deltaTime; // ��Ÿ�� Ÿ�̸� ����
            if (dragonAttackCooldownTimer <= 0)
            {
                spawnShapeCircle = !spawnShapeCircle;
                isDragonAttackCooldown = false; // ��Ÿ�� ����
                dragonAttackCooldownTimer = 0;
            }
        }
    }

    // ��� ���� ��Ÿ�� ����
    void StartDragonAttackCooldown()
    {
        isDragonAttackCooldown = true; // ��Ÿ�� ����
        dragonAttackCooldownTimer = dragonAttackCooldownDuration; // ��Ÿ�� Ÿ�̸� ����
    }

    public bool isMeleCooldown;
    public float MeleeCoolTimer; // ��Ÿ�� Ÿ�̸�
    public float MeleeCoolTimerDur;// ��Ÿ�� ���� �ð�
    void StartMeleeAttackCooldown()
    {
        isMeleCooldown = true; // ��Ÿ�� ����
        MeleeCoolTimer = MeleeCoolTimerDur; // Ÿ�̸� �ʱ�ȭ
    }
    void UpdateMeleeAttackCooldown()
    {
        if (isMeleCooldown)
        {
            MeleeCoolTimer -= Time.deltaTime; // Ÿ�̸� ����
            if (MeleeCoolTimer <= 0)
            {
                isMeleCooldown = false; // ��Ÿ�� ����
                MeleeCoolTimer = 0;
                isKnockbackInProgress = false;
            }
        }
    }

    IEnumerator ChargeToPlayer()
    {
        if (isChargingCooldown)
        {
            Debug.Log("���� ��Ÿ�� ���Դϴ�.");
            yield break;
        }
        isChaging = true; // ���� ����
        //���� �غ� �ִϸ��̼� ���. 
        yield return new WaitForSeconds(2.0f);
        PpAnim.Play("PP_Idle");
        isDragonAtkReady = false; //���� �� ��� ��ȯ ���ϰ�.false ó��


        float elapsedTime = 0f;
        Vector2 initialPosition = (Vector2)transform.position; // ��������� ĳ�����Ͽ� Vector2�� ��ȯ

        Vector2 targetPosition = lastPlayerPos;
        PpAnim.Play("PP_Charging");
        while (elapsedTime < chargeDistace)
        {
            targetPosition = lastPlayerPos;

            float newX = Mathf.Lerp(initialPosition.x, targetPosition.x, elapsedTime * chargeSpeed);

            transform.position = new Vector3(newX, transform.position.y, transform.position.z);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        PpAnim.Play("PP_Idle");
        yield return new WaitForSeconds(2.0f);
        isRush = false; //ô���� ���� bool���� ���� 1     
        isMeleeAttack = true;//ô���� ���� bool���� ���� 2
    }

    void ChagingSet()//������ ���� bool���� ����
    {
        isRush = true; //
        isChaging = false; // ���� ����
    }

    public bool isMeleeAttack;
    void MeleeAttack()
    {
        if (isMeleCooldown)
        {
            Debug.Log("��� ��Ÿ�� ���Դϴ�.");
            return;
        }
        isAttack = false;
        isDragonAtkReady = true;

        Collider2D[] coliderMelee = Physics2D.OverlapBoxAll(bounsing.position, bounsingSize, 0);

        foreach (Collider2D collider in coliderMelee)
        {
            if (collider.CompareTag("Player"))
            {     
                isAttack = true;
            }
        }

        if(isAttack == false && isDragonAtk == false)
        {
          
            PpAnim.SetBool("PPMeleeAtk", true);
        }

        if (isAttack && isMeleeAttack == true && !isKnockbackInProgress)
        {
            Debug.Log("ô�� ����");
            StartCoroutine(GraplingKnockback(playerController, meleeAttackForce,"MeleeAttack"));
        }
        else if (isAttack == false && isDragonAtkReady == true)
        {
            lastPlayerPos = playerObject.transform.position;
            if (isSpawningDragonWeapon == false && isDragonAttackCooldown == false)
            {
                isSpawningDragonWeapon = true;
                DragonWeaponFunc(lastPlayerPos); //��� ���� �޼��� ȣ��
            }

        }


    }


    void DragonWeaponFunc(Vector3 playerPos)
    {

        Vector3 spawnPosition = (Vector3)playerPos;

        if (spawnShapeCircle)
        {
            SpawnInCircle(spawnPosition);
        }
        else
        {
            SpawnInSquare(spawnPosition);
        }
    }

    public bool isDragonAtk;
    void SpawnInCircle(Vector3 spawnPosition)
    {


        isDragonAtk = true;
        for (int i = 0; i < CircleCount; i++)
        {
            PpAnim.SetTrigger("PPLongAtk");

            float angle = i * Mathf.PI * 2f / CircleCount; // �� ������ ����
            float x = spawnPosition.x + Mathf.Cos(angle) * CircleRadius;
            float y = spawnPosition.y + Mathf.Sin(angle) * CircleRadius;

            Vector3 position = new Vector3(x, y, 0f);
            var weapon = _Pool.Get();
            weapon.transform.position = position;
            activeDragonWeapons++;

            StartCoroutine(SpawnZacomAndExplosion(position));
        }
       

    }


    IEnumerator SpawnZacomAndExplosion(Vector3 position)
    {
        isDragonAtk = false;
        yield return new WaitForSeconds(WeaponLifeTime); //�� �ð� �ڿ� ��� ��Ȱ��ȭ  
        ReleaseDragonWeapons(); //��� ��Ȱ��ȭ
        isDragonAtk = true;
        PpAnim.SetTrigger("PPLongAtk");
        var zacom = _zacomPool.Get(); //���� Ǯ������ ���� ��
        zacom.transform.position = position; //��� ��ġ�� ���� ��ġ�� ���� ��Ű��

        yield return new WaitForSeconds(ZakoomLifeTime); //0.5�� ��
        PpAnim.SetTrigger("PPLongAtk");
        var explosion = _explosionPool.Get(); //���� ���� ����Ʈ ���� ��
        explosion.transform.position = position; //��� ��ġ�� ���� ���� ��ġ�� ���� ��Ű��

        zacom.ReturnToPool(); //���� ��Ȱ��ȭ - �̶��� ���� ���� ����Ʈ �������� ����
        yield return new WaitForSeconds(BoomEffLifeTime); // 0.2�� ��
        explosion.ReturnToPool(); //���� ��ź ����Ʈ ��Ȱ��ȭ

        isDragonAtk = false;
        isSpawningDragonWeapon = false;


        StartDragonAttackCooldown();
    }

    void SpawnInSquare(Vector3 spawnPosition)
    {
        
        Vector3[] offsets = new Vector3[]
        {
            new Vector3(-squareSize, squareSize, 0),
            new Vector3(0, squareSize, 0),
            new Vector3(squareSize, squareSize, 0),
            new Vector3(-squareSize, 0, 0),
            new Vector3(squareSize, 0, 0),
            new Vector3(-squareSize, -squareSize, 0),
            new Vector3(0, -squareSize, 0),
            new Vector3(squareSize, -squareSize, 0)
        };

        for (int i = 0; i < squareCount; i++)
        {
            PpAnim.SetTrigger("PPLongAtk");

            Vector3 position = spawnPosition + offsets[i];
            var weapon = _Pool.Get();
            weapon.transform.position = position;
            activeDragonWeapons++;

            StartCoroutine(SpawnZacomAndExplosion(position));
        }


    
        


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

    public bool isChargePlayer;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(isChaging)
            {
                isChargePlayer = true;
            }
         
        }
    }
    void PpDirection()
    {
        playerTransform = playerObject.transform;
        if (playerTransform.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            PpDir = Vector2.right;
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
            PpDir = Vector2.left;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(bounsing.position, bounsingSize);
    }

    IEnumerator Enemies.GraplingAtkDamaged()
    {
        throw new System.NotImplementedException();
    }

    public PPData ppdata;
    IEnumerator Enemies.baseDamaged()
    {
        ppdata.DamagedHp(10.0f);

        yield return null;

        if (ppdata.ppHP <= float.Epsilon)
        {

        }
    }

    void Enemies.PlayerToDamaged()
    {
        throw new System.NotImplementedException();
    }

    IEnumerator Enemies.Died()
    {
        throw new System.NotImplementedException();
    }

    void Enemies.SpeedDown()
    {
        throw new System.NotImplementedException();
    }

    void Enemies.EnemySet()
    {
        throw new System.NotImplementedException();
    }

    IEnumerator Enemies.NotFindPlayer(SpriteRenderer sprite)
    {
        throw new System.NotImplementedException();
    }

    IEnumerator Enemies.EnemyAtkStop()
    {
        yield return null;
    }

    void Enemies.UpdateOutline(bool outline)
    {
        throw new System.NotImplementedException();
    }
}
