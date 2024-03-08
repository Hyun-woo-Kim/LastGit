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
        if(bmdata.bmHp <= float.Epsilon)
        {
            StartCoroutine(Died());
        }
        isDamage = true;
        bmdata.DamagedHp(1);
        
        BManim.SetBool("BmAtk", true);
        BManim.SetFloat("BmAtkCount", -1.0f);
        yield return new WaitForSeconds(0.5f);
        for (int i =0; i< 2; i++)
        {
           BManim.SetFloat("BmAtkCount", 0.0f);                 
           yield return new WaitForSeconds(0.8f);
            Debug.Log("잽 공격");
        }
        
        BManim.SetBool("BmAtk", false);
        isDamage = false;
    }

    public void PlayerToDamaged()
    {
        
    }

    public bool isDied;
    public IEnumerator Died()
    {
        Debug.Log("죽음");
        //Vector2 direction = (target.position - transform.position).normalized;

        // 플레이어 방향으로 이동
        BManim.SetTrigger("BmBoomMove");
        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
        isDied = true;
        yield return null;
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
            BmMove();

        }
        else if (!isFindPlayer && hasAttacked)
        {
            hasAttacked = false;
            isMove = false;
        }

     
    }
    public float delay;
    IEnumerator Find()
    {
        //움직이기 전
        hasFoundPlayer = true;
        Debug.Log("움직이기 전");
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
                if (!hasAttacked)
                {
                    StartCoroutine(StopAttack());
                }
            }
        }
    }
    IEnumerator StopAttack()
    {
       
        if(isDamage == false)
        {
            Debug.Log("강화 잽 공격");
            BManim.SetBool("BmAtk", true);
            BManim.SetFloat("BmAtkCount", 1.0f);
        }
        
        isMove = false;
        hasAttacked = true;
        followSpeed = 0.0f;
        yield return new WaitForSeconds(1.0f);
        BManim.SetBool("BmAtk", false);
        BManim.SetFloat("BmAnimCount", 1.0f);
        followSpeed = 0.5f;
        hasAttacked = false;
        isMove = true;
    }
    void BmMove()
    {

        if (isDamage == false)
        {
            transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime * followSpeed);
            if (isDamage == false)
            {
                BManim.SetFloat("BmAnimCount", 1.0f);
            }
        }
      
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Player"))
        {
            if(isDied)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(Findboxpos.position, FindboxSize);//DrawWireCube(pos.position,boxsize)          
        Gizmos.DrawWireCube(Punchboxpos.position, PunchboxSize);//DrawWireCube(pos.position,boxsize)          
    }


}
