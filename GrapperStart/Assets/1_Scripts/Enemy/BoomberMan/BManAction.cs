using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class BManAction : MonoBehaviour,Enemies
{
    Animator BManim;
    CapsuleCollider2D collder;
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

    public IEnumerator GraplingAtkDamaged(float damage)
    {
        yield return null;
    }

    public bool isAtkStop;
    public bool chaneAttackMon;
    public float rayDistance = 5f; // Ray�� �ִ� �Ÿ�
    public LayerMask PlayerLayer;
    public IEnumerator baseDamaged()
    {
        chaneAttackMon = true;
        if (bmdata.bmHp >= 0)
        {
            bmdata.DamagedHp(DamagedValue);
            isMove = false;
            if (isStand)
            {
                BManim.SetBool("BmAtk", false);
                BManim.SetFloat("BmAnimCount", 0.0f);

                yield return new WaitForSeconds(0.1f);
                //BManim.SetBool("BmAtk", false);
              
                isMove = true;
            }
            isAtkStop = false;
        }

        else
        {
            StartCoroutine(Died());
        }

      

    }



    public void PlayerToDamaged()
    {
        ApplyDamageToPlayer(basicDamage);
    }

    public void PlayerToPowerDamaged()
    {
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
        FlipEnemy(target.transform);
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

                HandAttack();
              
            }
        }
        if (isFindPlayer && isStand  && isPunch == false)
        {
            isMove = true;
            isPlayerMissing = false;
            
             BmMove();
            
            
        }
        else if(isFindPlayer == false && isStand)
        {
            isMove = false;
            isPlayerMissing = true;
            StartCoroutine(NotFindPlayer(spriteRenderer));
            //�ʵ� ��ȸ
        }

       
        PatrolReaction(spriteRenderer);
    }
 

    

     void PatrolMovement(float patrolSpeed, float patrolDis, Vector2 patrolDir, Vector2 starPos)
    {

        bool hasChangedDirection = false;

        if (!hasChangedDirection && Mathf.Abs(transform.position.x - startPosition.x) >= patrolDis)
        {
            Debug.Log("������ȯ");
            patrolDirection *= -1;
            startPosition = transform.position;
            hasChangedDirection = true;
        }

        if (patrolDir == Vector2.right)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            transform.Translate(patrolDir * patrolSpeed * Time.deltaTime);
        }
        else if (patrolDir == Vector2.left)
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.Translate(patrolDir * patrolSpeed * Time.deltaTime);
        }



    }
    IEnumerator playerFind()
    {
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



    public bool isAttackReady;
    void BmMove()
    {
       
        FlipEnemy(target);

        transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime * followSpeed);
            
        if (!BManim.GetBool("BmAtk"))
        {
            BManim.SetBool("BmAtk", false);
            BManim.SetBool("BmIdle", false);
            BManim.SetFloat("BmAnimCount", 1.0f);
        }

     
    }

    void HandAttack()
    {
        Transform bmHand = transform.GetChild(3);
        //isPunch = false;
        //Collider2D[] colliders = Physics2D.OverlapBoxAll(Punchboxpos.transform.position, PunchboxSize, 0);
        //foreach (Collider2D collider in colliders)
        //{
        //    if (collider.CompareTag("Player"))
        //    {
        //        if (isStand)
        //        {
        //            isPunch = true;

        //        }

        //    }
        //}
        //if(isStand && isPunch)
        //{
        //    Debug.Log("�ڽ��� �������Ƿ� ���� o");
        //    StartCoroutine(delayAttack(0.5f));
        //}
        //if (isPunch == false)
        //{
        //    bmHand.gameObject.SetActive(false);
        //    //BmMove();
        //    //BManim.SetBool("BmAtk", false); //���� ������ ������Ƿ� ���ݾִϸ��̼� ����.
        //                                    //UpdateOutline(false);
        //}


        float distanceToPlayer = DistanceToPlayer(transform, target);

        // ���� �÷��̾���� �Ÿ��� �ִ� ���� �Ÿ� �̳��� �ִٸ�
        if (distanceToPlayer <= maxAttackDistance)
        {
            // ���� ���� �ִϸ��̼��� ���
            PlayAttackAnimation();
        }
        else
        {
            BManim.SetBool("BmAtk", false);
            BmMove();
        }

    }
    public float maxAttackDistance = 3.0f;
    float DistanceToPlayer(Transform playerTransform, Transform enemyTransform)
    {
        return Vector3.Distance(playerTransform.position, enemyTransform.position);
    }

    void PlayAttackAnimation()
    {
        // ���� �ִϸ��̼� ��� �ڵ带 ���⿡ �߰�
        BManim.SetBool("BmIdle", false); // ���� ���ĵ� ���� ����.
        BManim.SetBool("BmAtk", true); // ���� �ִϸ��̼�1
        BManim.SetFloat("BmAtkCount", 1.0f); // ���� �ִϸ��̼�2
    }

    public bool isHandAttack;
    public float Outlinedelay;
    public float puhchAttackDelay;
    IEnumerator delayAttack(float _attackDelay)
    {

        BManim.SetBool("BmIdle", true); // �ƿ����� ������ ���¿��� ���ĵ� ���� ����
        yield return new WaitForSecondsRealtime(Outlinedelay); // ������ �ɰ�
        UpdateOutline(true);

        if (isAtkStop == false)
        {
            yield return new WaitForSecondsRealtime(0.5f); // ������ �ɰ�
            BManim.SetBool("BmIdle", false); // ���� ���ĵ� ���� ����.
            BManim.SetBool("BmAtk", true); // ���� �ִϸ��̼�1
            BManim.SetFloat("BmAtkCount", 1.0f); // ���� �ִϸ��̼�2
        }
        UpdateOutline(false);
      
        isAttackReady = false;
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
        else if (isReact == false && isPlayerMissing == false)
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
        sprite.sprite = missSprite;


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
            if (isFindPlayer == false)
            {
                yield return new WaitForSeconds(notFindDelayAnim);
            }

            hasReachedStartPosition = true;
        }

        BManim.SetBool("BmIdle", false);
        BManim.SetFloat("BmAnimCount", 1.0f);
        PatrolMovement(patrolSpeed, patrolDistance, patrolDirection, startPosition);
        yield return null;
    }

    public IEnumerator EnemyAtkStop()
    {
        isAtkStop = true;
        StartCoroutine(baseDamaged());
        yield return null;
    }
}
