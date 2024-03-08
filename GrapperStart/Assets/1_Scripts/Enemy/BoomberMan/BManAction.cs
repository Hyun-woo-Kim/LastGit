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

    }

    public void EnemySet()
    {
       
    }

    public bool hasAttacked;
    public bool hasFoundPlayer;
    public bool isMove;
    void FindedPlayer()
    {
        isFindPlayer = false;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(Findboxpos.transform.position, FindboxSize, 0);
        foreach(Collider2D collider in colliders)
        {
            if(collider.CompareTag("Player"))
            {
                //Vector2 colliderBm = new Vector2(1.3f, 3.0f);
                //collder.size = colliderBm;
                target = collider.transform;
                isFindPlayer = true;
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
        //if(isDied)
        //{
        //    BManim.SetTrigger("BmBoomMove");
        //    Debug.Log(target.position);
        //    transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime * moveSpeed);
        //    if(isColliderPlayer)
        //    {
        //        DestroyBM(1.0f);
        //    }
        //}

     
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
    IEnumerator StopAttack()
    {
        isMove = false;

        if (isDamage == false)
        {
            Debug.Log("강화 잽 공격");
            BManim.SetBool("BmAtk", true);
            BManim.SetFloat("BmAtkCount", 1.0f);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(Findboxpos.position, FindboxSize);//DrawWireCube(pos.position,boxsize)          
        Gizmos.DrawWireCube(Punchboxpos.position, PunchboxSize);//DrawWireCube(pos.position,boxsize)          
    }


}
