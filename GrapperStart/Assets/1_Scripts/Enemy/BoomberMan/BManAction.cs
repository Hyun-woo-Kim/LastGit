using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BManAction : MonoBehaviour,Enemies
{
    Animator BManim;
    CapsuleCollider2D collder;
    Rigidbody2D Bmrigid;
    public BMdata bmdata;

    Vector2 InitCollSize;
    void Start()
    {
        BManim = GetComponent<Animator>();
        collder = GetComponent<CapsuleCollider2D>();
        Bmrigid = GetComponent<Rigidbody2D>();
        //hand.gameObject.SetActive(false);
        animSpeed = BManim.speed;
        InitCollSize = collder.size;

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

    

    public IEnumerator GraplingAtkDamaged(float damage)
    {
        yield return null;
    }

    public bool isDamage;
    public IEnumerator baseDamaged()
    {
        BManim.SetBool("BmAtk", true);
        BManim.SetFloat("BmAtkCount", -1.0f);

        isMove = false;
        isDamage = true;
        bmdata.DamagedHp(1);

        
        isMove = true;

        if (bmdata.bmHp <= 0.0f)
        {
            StartCoroutine(Died());
        }
        BManim.SetBool("BmAtk", false);
        yield return null;
       
    }

    public void PlayerToDamaged()
    {
        PlayerControllerRope player = GameObject.Find("Player").GetComponent<PlayerControllerRope>();
        PlayerData playerData = player.playerData;
        playerData.DamagedHp(1);
    }

    public bool isDied;
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
    public float nockbackForce;
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

    public bool hasAttacked;
    public bool isMove;


    void FindedPlayer()
    {
        Transform fourChild = transform.GetChild(5);
        SpriteRenderer spriteRenderer = fourChild.GetComponent<SpriteRenderer>();

        isFindPlayer = false;
        isReact = false;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(Findboxpos.transform.position, FindboxSize, 0);
        foreach(Collider2D collider in colliders)
        {
            if(collider.CompareTag("Player"))
            {
                //Vector2 colliderBm = new Vector2(1.3f, 3.0f);
                //collder.size = colliderBm;
                
                isFindPlayer = true;
                isReact = true;
                FunchCollider();
                if (isFindPlayer  && isMove == false && isStandUp == false)
                {
                    StartCoroutine(Find()); //1
                }
 
            }
            if(collider.CompareTag("Enemy"))
            {
                isFindEnemy = true;
                if (isFindEnemy == true)
                {
                    TeamEnemy();
                    //StartCoroutine(Find());
                }
            }
        }

        if((isFindPlayer || isFindEnemy) && isStandUp && isMove)
        {
            BmMove(); //3
        }
        else if ((isFindPlayer == false || isFindEnemy == false) && isStandUp)
        {
            Debug.Log("�����");
            StartCoroutine(NotFind());
        }      
        PatrolReaction(spriteRenderer);
    }
    public bool isTeamDamage;
    void TeamEnemy()
    {
        GameObject[] allies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject ally in allies)
        {
            // �Ʊ� ��ũ��Ʈ�� �����Ǿ� �ִ��� Ȯ��
            BloodWorkerAction allyScript = ally.GetComponent<BloodWorkerAction>();
            if (allyScript != null)
            {
                // �Ʊ� ��ũ��Ʈ�� �������� �������� Ȯ��
                if (allyScript.isBasicDamaged || allyScript.isGraplingDamaged)
                {
                    isTeamDamage = true;
                    StartCoroutine(Find());
                    Debug.Log("�Ʊ��� ���� ����");
                    // �Ʊ��� ������ �޾��� ���� ó�� ����
                }
                else if(!allyScript.isBasicDamaged || !allyScript.isGraplingDamaged)
                {
                    //isFindEnemy = false;
                    Debug.Log("�Ʊ��� ���� ���� ����");
                }
            }
            else
            {

            }
        }
    }
    public bool isFindEnemy;
    void BmMove()
    {

        if (isDamage == false && isDied == false && isMove)
        {
            FlipEnemy(target);
            transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime * followSpeed);
            if (!BManim.GetBool("BmAtk"))
            {
                Debug.Log("�̵� ��");
                BManim.SetFloat("BmAnimCount", 1.0f);
            }
        }

    }
    IEnumerator  NotFind()
    {
        isMove = false;
        yield return new WaitForSeconds(1.0f);
        BManim.SetBool("BmAtk", false);
        //���ĵ� �ִϸ��̼� ����� �߰�.
        BManim.SetBool("BmFind", false);
        isStandUp = false;
        isFindEnemy = false;
        collder.size = InitCollSize;

    }
    public float delay;
    public bool isStandUp;
    IEnumerator Find()
    {
        //�����̱� ��
        isMove = false;
        isStandUp = true;

        BManim.SetBool("BmFind",true);
        BManim.SetFloat("BmAnimCount",0.0f);
        yield return new WaitForSeconds(1.0f);
        
        Vector2 colliderBm = new Vector2(1, 2.6f);
        collder.size = colliderBm;
        yield return new WaitForSeconds(2.0f);
        isMove = true; //2
        PlayerControllerRope playeScr = FindAnyObjectByType<PlayerControllerRope>();
        target = playeScr.transform;
        //�����̱� 
    }

    public Transform Punchboxpos;
    public Vector2 PunchboxSize;

    public bool isReact;
    public Sprite reactSprite;
    void FunchCollider()
    {
        hasAttacked = false;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(Punchboxpos.transform.position, PunchboxSize, 0);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                hasAttacked = true;
            }
        }

        if(isDamage == true)
        {
            StartCoroutine(HandAttack());
        }
       
    }
    IEnumerator HandAttack()
    {
        //�񼱰� ������ �� ���� �ڵ�. �þ߿� �÷��̾ �־�� �ϰ�, �������� �Ծ���ϰ�,��ġ �ݶ��̴� �ȿ� �÷��̾ �־����.
        //���� ���� �� �� ���� �ڵ�. �þ߿� �÷��̾ �־�� �ϰ�, ��ġ �ݶ��̴� �ȿ� �÷��̾ �־���ϰ�
        Debug.Log("a");
        while (isFindPlayer && hasAttacked && isDamage)
        {
            BManim.SetBool("BmAtk", true);
            BManim.SetFloat("BmAtkCount", 0); // �� ���� ����
            yield return new WaitUntil(() => IsAnimationFinished()); // ù ��° �ִϸ��̼� ���Ḧ ��ٸ�

            BManim.SetFloat("BmAtkCount", 1); // ��ȭ ��ġ ���� ����
            yield return new WaitUntil(() => IsAnimationFinished()); // �� ��° �ִϸ��̼� ���Ḧ ��ٸ�
            isDamage = false;
        }


        if (isFindPlayer && hasAttacked == false)
        {
            BManim.SetBool("BmAtk", false);
            BmMove();
        }
    }
    bool IsAnimationFinished()
    {
        // �ִϸ��̼��� �������� ���θ� ���⿡�� Ȯ��
        return BManim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f;
    }
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
        if (isReact || isTeamDamage)
        {
            spriteRenderer.sprite = reactSprite;
        }
        else if (isReact == false)
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


}
