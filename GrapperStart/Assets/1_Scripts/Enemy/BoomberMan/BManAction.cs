using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR;
using static System.Net.Mime.MediaTypeNames;

public class BManAction : MonoBehaviour,Enemies
{
    Animator BManim;
    CapsuleCollider2D collder;
    protected CircleCollider2D childCollider;
    SpriteRenderer bmSpr;
    Rigidbody2D Bmrigid;

    public BMdata bmdata;

    Vector2 InitCollSize;
    void Start()
    {
        BManim = GetComponent<Animator>();
        bmSpr = GetComponent<SpriteRenderer>();
        collder = GetComponent<CapsuleCollider2D>();

        Bmrigid = GetComponent<Rigidbody2D>();
        //hand.gameObject.SetActive(false);
        animSpeed = BManim.speed;
        InitCollSize = collder.size;

        UpdateOutline(false);
    }

 

    IEnumerator test()
    {

        childCollider = GetComponentInChildren<CircleCollider2D>();
        // �ڽ� Ŭ������ �ݶ��̴��� �����ϸ� ��Ȱ��ȭ
        if (childCollider != null)
        {
            Debug.Log("ss");
            childCollider.enabled = true;
            yield return new WaitForSeconds(0.1f);
            childCollider.enabled = false;
        }
    }
    // Update is called once per frame

    [Header("##Basic")]
    public Transform Findboxpos;
    public Vector2 FindboxSize;
    public Transform target;
    public bool isFindPlayer;
    public float followSpeed;
    public float moveSpeed;
    public bool isStand = false;
    public float growthRate = 0.9f; //�Ͼ�� �ӵ�
    public float notFindDelayAnim; //�÷��̾ ã�� ������ �� ���ĵ� ���� �ִϸ��̼� ������
    public bool isMove;
    public bool isPlayerMissing = false; //�÷��̾ ������ �� bool
    public bool isDied; //�׾��� ��
    public float animAimSpeed; //�÷��̾ ������ �������� �� �ִϸ��̼� �ӵ��� ���������ϴ� ����
    public float animSpeed; //���� �ִϸ��̼� �ӵ��� �ٲ�� ����

    [Header("##Reaction")]
    public bool isReact;
    public Sprite reactSprite;//�÷��̾ ã���� �� ��������Ʈ
    public Sprite missSprite; //�÷��̾ ������ �� ����ǥ ��������Ʈ.



    [Header("##Attack")]
    public float nockbackForce; //�׾��� �� �÷��̾�� �˹� �� �󸶳� ����
    public bool hasAttacked; //�������� �� true
    public bool isPlayerFindCoroutineRunning = false;
    public Transform Punchboxpos;
    public Vector2 PunchboxSize;
    public bool isColliderPlayer; 
    public bool isPunch;
    public bool isAttack;
    public float DamagedValue; //�÷��̾�� �󸶳� �������� ������
    public float basicDamage; //�� ���� ������
    public float powerDamage; //��ȭ ���� ������

    [Header("##Patrol")]
    public Vector2 patrolDirection; //���� ����
    public Vector2 startPosition; //���� ���� �� ó�� ��ġ
    public float patrolSpeed = 2.0f; //���� �ӵ�
    public float patrolDistance = 5.0f; // ���� �Ÿ�
    private bool hasReachedStartPosition;

    public IEnumerator GraplingAtkDamaged()
    {
        StartCoroutine(baseDamaged());

        yield return null;
    }

    public bool isAtkStop;
    public bool chaneAttackMon;
    public float WallrayDistance = 5f; // ���� ������ ����.
    public LayerMask PlayerLayer;
    public IEnumerator baseDamaged()
    {
        bmdata.DamagedHp(DamagedValue);
        chaneAttackMon = true;
        if(isStand)
        {
            

            if (bmdata.bmHp > 0)
            {
                isMove = false;
                isDamaged = true;

                BManim.SetTrigger("BmNockBack");
                yield return new WaitForSeconds(0.5f);
                BManim.SetTrigger("BmNockBackToIdle");

                isDamaged = false;
                isMove = true;
                
            }

            else if(bmdata.bmHp <= 0)
            {
                Debug.Log("0 �̸�");
                StartCoroutine(Died());
            }

        }

        isAtkStop = false;

        yield return null;

    }



    public void PlayerToDamaged()
    {
        StartCoroutine(test());
        ApplyDamageToPlayer(basicDamage);
    }

    public void PlayerToPowerDamaged()
    {
        StartCoroutine(test());
        ApplyDamageToPlayer(powerDamage);
    }
    private void ApplyDamageToPlayer(float damage)
    {
        PlayerControllerRope player = GameObject.Find("Player").GetComponent<PlayerControllerRope>();
        if (player != null)
        {
            PlayerData playerData = player.playerData;
            playerData.DamagedHp(damage);
        }
    }
    public IEnumerator Died()
    {

        Debug.Log("����");
        isDied = true;
        FlipEnemy(target);
        BManim.SetTrigger("BmBoomMove");
        Debug.Log(target.position);

        float elapsedTime = 0f;
        Vector2 initialPosition = transform.position;

        while (!isColliderPlayer && elapsedTime < 1.0f)
        {
            transform.position = Vector2.Lerp(initialPosition, target.position, elapsedTime);
            elapsedTime += Time.deltaTime * moveSpeed;

            yield return null;
        }

        if (isColliderPlayer) //isDied���¿��� �÷��̾�� �浹 ��
        {
            PlayerControllerRope playerController = FindAnyObjectByType<PlayerControllerRope>();
            if (playerController != null)
            {
                playerController.BMSkillMove(transform, nockbackForce); // ���� ��ġ�� ����
            }
            DestroyBM();
        }

    }
    public void SpeedDown()
    {
        followSpeed = 0.1f;
        BManim.speed = animAimSpeed;
    }

    public void EnemySet()
    {
        followSpeed = 0.2f;
        BManim.speed = animSpeed;
    }

    void Update()
    {
        if (!isPlayerFindCoroutineRunning && isFindPlayer && chaneAttackMon) // �ڷ�ƾ�� ���� ���� �ƴ� ��쿡�� ����
        {
            StartCoroutine(playerFind());
        }

        FindedPlayer();

        if (hasReachedStartPosition == true)
        {
            Debug.Log("�̵�");
            PatrolMovement(patrolSpeed, patrolDistance, patrolDirection, startPosition);
        }
    }

    void FindedPlayer()
    {
        Transform fourChild = transform.GetChild(5);
        SpriteRenderer spriteRenderer = fourChild.GetComponent<SpriteRenderer>();

        isReact = false;
        isFindPlayer = false;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(Findboxpos.transform.position, FindboxSize, 0);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                //Vector2 colliderBm = new Vector2(1.3f, 3.0f);
                //collder.size = colliderBm;
                target = collider.transform;
                hasReachedStartPosition = false;
                isFindPlayer = true;
                isReact = true;
               
              
            }
        }
        if (isFindPlayer && isStand  && isAtk == false)
        {
            isMove = true;
            isPlayerMissing = false;
            if (!hasFlipped)
            {
                FlipEnemy(target);
                hasFlipped = true; // ���������� ǥ��
            }
            else if(isMove)
            {
                BmMove();
            }
            
            
            
        }
        else if(isFindPlayer == false && isStand)
        {
            isMove = false;
            isPlayerMissing = true;
            StartCoroutine(NotFindPlayer(spriteRenderer));
            //�ʵ� ��ȸ
        }

        if(isFindPlayer == true && chaneAttackMon == true && isStand )
        {
            HandAttack();
        }       
        PatrolReaction(spriteRenderer);
    }

    bool hasFlipped = false;


    void PatrolMovement(float patrolSpeed, float patrolDis, Vector2 patrolDir, Vector2 starPos)
    {

        bool hasChangedDirection = false;

        WallTurn(); //���� �ÿ��� ���� ����

        if (!hasChangedDirection && Mathf.Abs(transform.position.x - startPosition.x) >= patrolDis && isWall)
        {
            Debug.Log("������ȯ");
            patrolDirection *= -1;
            startPosition = transform.position;
            hasChangedDirection = true;
            isWall = false;
        }

        if (patrolDir == Vector2.right)
        {
            Debug.Log("�̵�1");
            transform.localScale = new Vector3(-1, 1, 1);
            transform.Translate(patrolDir * patrolSpeed * Time.deltaTime);
        }
        else if (patrolDir == Vector2.left)
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.Translate(patrolDir * patrolSpeed * Time.deltaTime);
        }

        BManim.SetBool("BmIdle", false);
        BManim.SetFloat("BmAnimCount", 1.0f);

    }
    public LayerMask wallLayer;
    public bool isWall;
    public void WallTurn()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, patrolDirection, Mathf.Infinity, wallLayer);
        Debug.DrawRay(transform.position, patrolDirection * WallrayDistance, Color.red);
        // Raycast�� ���� �浹�� ���
        if (hit.collider != null)
        {
            float distance = Mathf.Abs(hit.point.x - transform.position.x);

            if (distance < WallrayDistance)
            {
                isWall = true;
            }
        }

    }
    IEnumerator playerFind()
    {
        gameObject.layer = 9; // Enemy���̾� ����
        isPlayerFindCoroutineRunning = true;
        BManim.SetBool("BmFind", true);

        float sizeY = 1.7f;
        float targetSizeY = 2.6f; // ��ǥ ũ��

        while (sizeY <= targetSizeY)
        {
            float delta = growthRate * Time.deltaTime;
            sizeY += delta;

            // ũ�Ⱑ ��ǥ ũ�⸦ �ʰ��ϴ� ��� ����
            if (sizeY > 2.5f)
            {
                break;
            }

            Vector2 colliderBm = new Vector2(1.0f, sizeY);
            collder.size = colliderBm;

            BManim.SetFloat("BmAnimCount", -1.0f);

            yield return null;
        }
        BManim.SetBool("BmIdle",true);
        isStand = true;
  
    }//�Ͼ�� �޼���



  
    public bool isAttacking;
    void BmMove()
    {        
        if (!BManim.GetBool("BmAtk"))
        {
            hasFlipped = false;
            if (!hasFlipped)
            {
                FlipEnemy(target);
                hasFlipped = true; // ���������� ǥ��
            }
            BManim.SetBool("BmAtk", false);
            BManim.SetBool("BmIdle", false);
            BManim.SetFloat("BmAnimCount", 1.0f);

            transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime * followSpeed);
        }

     
    }

    public bool isAtk;
    void HandAttack()
    {
        isAtk = false;
        UpdateOutline(false);
        Collider2D[] colliders = Physics2D.OverlapBoxAll(Punchboxpos.position, PunchboxSize, 0);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                
                isAtk = true;
            }
        }

        float distanceToPlayer = DistanceToPlayer(transform, target);

        // ���� �÷��̾���� �Ÿ��� �ִ� ���� �Ÿ� �̳��� �ִٸ�
        if (distanceToPlayer <= maxAttackDistance && isAtk && isDamaged == false)
        {
            // ���� ���� �ִϸ��̼��� ���
            isAttacking = true;
            
            StartCoroutine(PlayAttackAnimation());
        }
        else
        {
            
            BManim.SetBool("BmAtk", false);
            
        }

    }
    public float maxAttackDistance = 3.0f;
    float DistanceToPlayer(Transform playerTransform, Transform enemyTransform)
    {
        return Vector3.Distance(playerTransform.position, enemyTransform.position);
    }

    public bool isDamaged;
    IEnumerator PlayAttackAnimation()
    {
        UpdateOutline(true);
        BManim.SetBool("BmIdle", true); // ���� ���ĵ� ���� ����.
        yield return new WaitForSeconds(1.0f);
        BManim.SetBool("BmIdle", false); // ���� ���ĵ� ���� ����.

        if (isDamaged == false && isAttacking == true)
        {
            Debug.Log("���� ����");
            isMove = false;  
            BManim.SetBool("BmAtk", true); // ���� �ִϸ��̼�1
            BManim.SetFloat("BmAtkCount", 1.0f); // ���� �ִϸ��̼�2
            //UpdateOutline(false);

            isAttacking = false; // ���� ����
        }
        

    }




    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(isDied)
            {
                isColliderPlayer = true;
            }
            
        }
    }
    void FlipEnemy(Transform _target)
    {
        if (transform.position.x > _target.position.x)
        {
            Debug.Log("���� ��2");
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (transform.position.x < _target.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    void PatrolReaction(SpriteRenderer spriteRenderer)
    {        
        if (isReact  && isStand)
        {
            spriteRenderer.sprite = reactSprite;
        }
        else if (isReact == false)
        {
            spriteRenderer.sprite = null;

        }
    }
    
   

   

    public GameObject dieEffPrefab;
    private GameObject dieEff;
    void DestroyBM()
    {
        Destroy(this.gameObject);
        dieEff = Instantiate(dieEffPrefab, transform.position, Quaternion.identity);
        dieEff.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(Findboxpos.position, FindboxSize);//DrawWireCube(pos.position,boxsize)          
        Gizmos.DrawWireCube(Punchboxpos.position, PunchboxSize);//DrawWireCube(pos.position,boxsize)          
    }


    public Color OutLineEnemycolor = Color.yellow;

    [Range(0, 16)]
    public int outlineSize;
    public void UpdateOutline(bool outline)
    {
       
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        bmSpr.GetPropertyBlock(mpb);
        mpb.SetFloat("_Outline", outline ? 1f : 0);
        mpb.SetColor("_OutlineColor", OutLineEnemycolor);
        mpb.SetFloat("_OutlineSize", outlineSize);
        bmSpr.SetPropertyBlock(mpb);

    }

    public IEnumerator NotFindPlayer(SpriteRenderer sprite)
    {
        BManim.SetBool("BmAtk", false);

        if (!hasReachedStartPosition)
        {
            startPosition = transform.position;
            Debug.Log("1");
            if (target.position.x < transform.position.x)
            {
                Debug.Log("2");
                patrolDirection = Vector2.right;
            }
            else if (target.position.x > transform.position.x)
            {
                Debug.Log("3");
                patrolDirection = Vector2.left;
            }

            BManim.SetBool("BmIdle", true);

           yield return new WaitForSeconds(notFindDelayAnim);
            
            hasReachedStartPosition = true;
        }
        yield return null;
    }

    public IEnumerator EnemyAtkStop()
    {
        isAtkStop = true;
        //StartCoroutine(baseDamaged());
        yield return null;
    }
}
