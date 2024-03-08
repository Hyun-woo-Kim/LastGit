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

    public Transform hand;

    public IEnumerator GraplingAtkDamaged(float damage)
    {
        yield return null;
    }

    public bool isDamage;
    public IEnumerator baseDamaged()
    {
        bmdata.DamagedHp(1);

        BManim.SetTrigger("BmNockBack");
        yield return new WaitForSeconds(0.5f);
        for (int i =0; i< 2; i++)
        {
            BManim.SetTrigger("BmHandAttack");
            yield return new WaitForSeconds(0.5f);
        }
        isDamage = false;
    }

    public void PlayerToDamaged()
    {
        
    }

    public IEnumerator Died()
    {
        yield return null;
    }

    public void SpeedDown()
    {

    }

    public void EnemySet()
    {
       
    }

    public bool hasAttacked;
    public bool isMove;
    void FindedPlayer()
    {
        isFindPlayer = false;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(Findboxpos.transform.position, FindboxSize, 0);
        foreach(Collider2D collider in colliders)
        {
            if(collider.CompareTag("Player"))
            {
                Vector2 colliderBm = new Vector2(1.3f, 3.0f);
                collder.size = colliderBm;
                target = collider.transform;
                isFindPlayer = true;
                FunchCollider();
                StartCoroutine(Find());
           
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
        Debug.Log("움직이기 전");
        BManim.SetBool("BmFind",true);
        BManim.SetFloat("BmAnimCount",0.0f);
        yield return new WaitForSeconds(2.0f);
        isMove = true;
        Debug.Log("움직이기 시작");
    }

    public Transform Punchboxpos;
    public Vector2 PunchboxSize;

    void FunchCollider()
    {
        Debug.Log("0");
        Collider2D[] colliders = Physics2D.OverlapBoxAll(Punchboxpos.transform.position, PunchboxSize, 0);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                //hand.gameObject.SetActive(true);
                Debug.Log("AA");
                if (!hasAttacked)
                {

                    StartCoroutine(StopAttack());
                }
            }
        }
    }
    IEnumerator StopAttack()
    {
        Debug.Log("2");
        BManim.SetBool("BmBoomHandAtk",true);
        isMove = false;
        hasAttacked = true;
        followSpeed = 0.0f;
        yield return new WaitForSeconds(1.0f);
        BManim.SetBool("BmBoomHandAtk", false);
        followSpeed = 0.5f;
        hasAttacked = false;
        isMove = true; 
        
        Debug.Log("3");
    }
    void BmMove()
    {

        if (isDamage == false)
        {
            Debug.Log("추적 중");
            transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime * followSpeed);
            BManim.SetFloat("BmAnimCount", 1.0f);
        }
      
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(Findboxpos.position, FindboxSize);//DrawWireCube(pos.position,boxsize)          
        Gizmos.DrawWireCube(Punchboxpos.position, PunchboxSize);//DrawWireCube(pos.position,boxsize)          
    }


}
