using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    void Update()
    {
        FindedPlayer();
    }

    public Transform Findboxpos;
    public Vector2 FindboxSize;
    public Transform target;
    public bool isFindPlayer;
    public float followSpeed;
    public float moveSpeed;
    public float animAimSpeed;
    public float animSpeed;


    public bool hasAttacked;
    public bool isMove;
    public bool isPlayerFindCoroutineRunning = false;
    public bool isStand = false;
    public bool isPlayerMissing = false;

    public float growthRate = 0.9f; //�Ͼ�� �ӵ�
    public float notFindDelayAnim; //�÷��̾ ã�� ������ �� ���ĵ� ���� ������

    public Sprite missSprite; //�÷��̾ ������ �� ����ǥ ��������Ʈ.

    public Vector2 patrolDirection;
    public Vector2 startPosition;
    public float patrolSpeed = 2.0f;
    public float patrolDistance = 5.0f; // ���� �Ÿ�
    public bool hasReachedStartPosition;
    public float nockbackForce;
    public bool isDied;
    public IEnumerator GraplingAtkDamaged(float damage)
    {
        yield return null;
    }

    public bool isDamage;
    public bool isAtk;
    public IEnumerator baseDamaged()
    {
        isDamage = true;
        isMove = false;
        if (isStand)
        {
            BManim.SetBool("BmAtk", true);
            BManim.SetFloat("BmAtkCount", -1.0f);
            yield return new WaitForSeconds(0.5f);
            BManim.SetBool("BmAtk", false);
            isDamage = false;
            isMove = true;
        }
        yield return null;
    }

    public void PlayerToDamaged()
    {
        PlayerControllerRope player = GameObject.Find("Player").GetComponent<PlayerControllerRope>();
        PlayerData playerData = player.playerData;
        playerData.DamagedHp(1);
    }


    public IEnumerator Died()
    {

        Debug.Log("����");
        isDied = true;
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
            DestroyBM(1.0f);
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
                if (!isPlayerFindCoroutineRunning && isFindPlayer && isDamage) // �ڷ�ƾ�� ���� ���� �ƴ� ��쿡�� ����
                {
                    isDamage = false;
                    StartCoroutine(playerFind());
                }
                PunchCollider();
            }
            if (collider.CompareTag("Enemy"))
            {
                //if (isFindEnemy == false)
                //{
                //    BloodWorkerAction bwScr = FindFirstObjectByType<BloodWorkerAction>();
                //    if (bwScr.isBasicDamaged || bwScr.isGraplingDamaged)
                //    {
                //        Debug.Log("�Ʊ� �߰�");
                //        isFindEnemy = true;
                //    }

                //}
            }
        }
        if(isFindPlayer && isStand && isDamage == false)
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
 

    IEnumerator NotFindPlayer(SpriteRenderer sprite)
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
            if(isFindPlayer == false)
            {
                yield return new WaitForSeconds(notFindDelayAnim);
            }
           
            hasReachedStartPosition = true;
        }


        yield return null;

        if (!BManim.GetBool("BmAtk"))
        {
            BManim.SetBool("BmIdle", false);
            BManim.SetFloat("BmAnimCount", 1.0f);
            PatrolMovement(patrolSpeed, patrolDistance, patrolDirection, startPosition);
        }
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

      



        //�̵� �ִϸ��̼� �߰�. 



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
        yield return null;
    }//�Ͼ�� �޼���


    public bool isTeamDamage;
    void TeamEnemy()
    {
        //GameObject[] allies = GameObject.FindGameObjectsWithTag("Enemy");
        //foreach (GameObject ally in allies)
        //{
        //    // �Ʊ� ��ũ��Ʈ�� �����Ǿ� �ִ��� Ȯ��
        //    BloodWorkerAction allyScript = ally.GetComponent<BloodWorkerAction>();
        //    if (allyScript != null)
        //    {
        //        // �Ʊ� ��ũ��Ʈ�� �������� �������� Ȯ��
        //        if (allyScript.isBasicDamaged || allyScript.isGraplingDamaged)
        //        {
        //            isTeamDamage = true;
        //            StartCoroutine(Find());
        //            Debug.Log("�Ʊ��� ���� ����");
        //            // �Ʊ��� ������ �޾��� ���� ó�� ����
        //        }
        //        else if(!allyScript.isBasicDamaged || !allyScript.isGraplingDamaged)
        //        {
        //            //isFindEnemy = false;
        //            Debug.Log("�Ʊ��� ���� ���� ����");
        //        }
        //    }
        //    else
        //    {

        //    }
        //}
    }
    public bool isFindEnemy;
    void BmMove()
    {
         
            FlipEnemy(target);
            transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime * followSpeed);
            if (!BManim.GetBool("BmAtk"))
            {
            BManim.SetBool("BmIdle", false);
            BManim.SetFloat("BmAnimCount", 1.0f);
            }
        

    }

    public Transform Punchboxpos;
    public Vector2 PunchboxSize;

    public bool isReact;
    public Sprite reactSprite;


    public bool isPunch;
    void PunchCollider()
    {
        Transform bmHand = transform.GetChild(3);
        isPunch = false;
        UpdateOutline(false);
        Collider2D[] colliders = Physics2D.OverlapBoxAll(Punchboxpos.transform.position, PunchboxSize, 0);
        foreach (Collider2D collider in colliders)
        {
            if(collider.CompareTag("Player"))
            {
                if(isStand)
                {
                    isPunch = true;
                }


            }
        }


         if (isPunch && isDamage == false)
        {
            Debug.Log("����");
            StartCoroutine(delayAttack());
            bmHand.gameObject.SetActive(true);
            BManim.SetBool("BmAtk", true);
            BManim.SetFloat("BmAtkCount", 1.0f);

        }

        else if (isPunch == false)
        {
            bmHand.gameObject.SetActive(false);
            BManim.SetBool("BmAtk", false);
            UpdateOutline(false);
        }
    }

    IEnumerator delayAttack()
    {
        yield return new WaitForSeconds(0.3f);
        UpdateOutline(true);
    }
    public float distance;
    public bool isColliderPlayer;


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
        if ((isReact || isTeamDamage) && isStand)
        {
            spriteRenderer.sprite = reactSprite;
        }
        else if (isReact == false && isPlayerMissing == false)
        {
            spriteRenderer.sprite = null;

        }
    }
    void DestroyBM(float delay)
    {
        Destroy(this.gameObject, delay);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(Findboxpos.position, FindboxSize);//DrawWireCube(pos.position,boxsize)          
        Gizmos.DrawWireCube(Punchboxpos.position, PunchboxSize);//DrawWireCube(pos.position,boxsize)          
    }


    public Color OutLineEnemycolor = Color.yellow;

    [Range(0, 16)]
    public int outlineSize = 2;
    public void UpdateOutline(bool outline)
    {
       
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        bmSpr.GetPropertyBlock(mpb);
        mpb.SetFloat("_Outline", outline ? 1f : 0);
        mpb.SetColor("_OutlineColor", OutLineEnemycolor);
        mpb.SetFloat("_OutlineSize", outlineSize);
        bmSpr.SetPropertyBlock(mpb);


    }
}
