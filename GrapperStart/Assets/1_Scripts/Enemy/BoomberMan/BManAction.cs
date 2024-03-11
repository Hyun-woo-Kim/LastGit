using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BManAction : MonoBehaviour,Enemies
{
    Animator BManim;
    CapsuleCollider2D collder;
    Rigidbody2D Bmrigid;
    public BMdata bmdata;
    void Start()
    {
        BManim = GetComponent<Animator>();
        collder = GetComponent<CapsuleCollider2D>();
        Bmrigid = GetComponent<Rigidbody2D>();
        hand = transform.GetChild(2);
        //hand.gameObject.SetActive(false);
        Debug.Log(hand.gameObject.name);
        animSpeed = BManim.speed;
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

    public Transform hand;

    public IEnumerator GraplingAtkDamaged(float damage)
    {
        yield return null;
    }

    public bool isDamage;
    public IEnumerator baseDamaged()
    {
        isMove = false;
        if(bmdata.bmHp <= float.Epsilon)
        {
            StartCoroutine(Died());
        }
        isDamage = true;
        bmdata.DamagedHp(1);
        
        BManim.SetBool("BmAtk", true);
        BManim.SetFloat("BmAtkCount", -1.0f);

        for (int i =0; i< 2; i++)
        {
           BManim.SetFloat("BmAtkCount", 0.0f);                 
           yield return new WaitForSeconds(0.5f);
            Debug.Log("잽 공격");
        }
        
        //BManim.SetBool("BmAtk", false);
        isDamage = false;
        isMove = true;
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

        Debug.Log("죽음");
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

        if (isColliderPlayer)
        {
           
            DestroyBM(1.0f);
        }

    }

    public void SpeedDown()
    {
        followSpeed = 0.1f;
        animAimSpeed = BManim.speed ;
    }

    public void EnemySet()
    {
        Debug.Log("플레이어가 전등을 조준 하지 않음");
        followSpeed = 0.2f;
        BManim.speed = animSpeed;
    }

    public bool hasAttacked;
    public bool hasFoundPlayer;
    public bool isMove;


    void FindedPlayer()
    {
        Transform fourChild = transform.GetChild(4);
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
                target = collider.transform;
                isFindPlayer = true;
                isReact = true;
                FunchCollider();
                if (!hasFoundPlayer)
                {
                    StartCoroutine(Find());
                }
                   
           
            }
        }

        if(isFindPlayer && isMove)
        {
            BManim.SetBool("BmAtk", false);
            BmMove();

        }
        else if (!isFindPlayer && hasAttacked)
        {
            hasAttacked = false;
            isMove = false;
        }

        PatrolReaction(spriteRenderer);
    }

    void DestroyBM(float delay)
    {
        Destroy(this.gameObject, delay);
    }
    public float delay;
    IEnumerator Find()
    {
        //움직이기 전
        hasFoundPlayer = true;
        BManim.SetTrigger("BmFinded");
        BManim.SetFloat("BmAnimCount",0.0f);
        yield return new WaitForSeconds(1.0f);
        
        Vector2 colliderBm = new Vector2(1, 2.6f);
        collder.size = colliderBm;
        yield return new WaitForSeconds(2.0f);
        isMove = true;

        //움직이기 
    }

    public Transform Punchboxpos;
    public Vector2 PunchboxSize;

    public bool isReact;
    public Sprite reactSprite;
    void FunchCollider()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(Punchboxpos.transform.position, PunchboxSize, 0);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                if (!hasAttacked && !isDamage)
                {
                    StartCoroutine(StopAttack());
                }
            }
        }
    }

    public float UpgradePunchDelay;
    IEnumerator StopAttack()
    {
        isMove = false;

        if (isDamage == false)
        {
            Debug.Log("강화 잽 공격");
            BManim.SetBool("BmAtk", true);
            BManim.SetFloat("BmAtkCount", 1.0f);
            yield return new WaitForSeconds(UpgradePunchDelay);
        }   
        hasAttacked = true;
        yield return new WaitForSeconds(1.0f);
        //BManim.SetBool("BmAtk", false);
        hasAttacked = false;
        isMove = true;
    }
    void BmMove()
    {

        if (isDamage == false && isDied == false)
        {
            FlipEnemy(target);
            transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime * followSpeed);
            if (!BManim.GetBool("BmAtk"))
            {
                Debug.Log("이동 중");
                BManim.SetFloat("BmAnimCount", 1.0f);
            }
        }
      
    }

    public bool isColliderPlayer;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isColliderPlayer = true;


        }
    }
    void FlipEnemy(Transform _target)
    {
        Debug.Log("방향뒤집기");
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
        //if (isReact || criture.isEnemyAttack)
        //{
        //    spriteRenderer.sprite = reactSprite;

        //}
        //else if (isReact == false || criture.isEnemyAttack == false)
        //{
        //    spriteRenderer.sprite = null;

        //}

        if (isReact)
        {
            spriteRenderer.sprite = reactSprite;

        }
        else if (isReact == false)
        {
            spriteRenderer.sprite = null;

        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(Findboxpos.position, FindboxSize);//DrawWireCube(pos.position,boxsize)          
        Gizmos.DrawWireCube(Punchboxpos.position, PunchboxSize);//DrawWireCube(pos.position,boxsize)          
    }


}
