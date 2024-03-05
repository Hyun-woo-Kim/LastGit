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
        hand.gameObject.SetActive(false);
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
    Transform hand;

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

                if (!hasAttacked)
                {
                    Debug.Log("A");
                    StartCoroutine(StopAttack());
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

    IEnumerator StopAttack()
    {
        BManim.SetTrigger("BmBoomHandAttack");
        hasAttacked = true;
        isMove = false;
        followSpeed = 0.0f;
        yield return new WaitForSeconds(1.0f);
        Debug.Log("1");
        followSpeed = 0.5f;
        isMove = true;
        Debug.Log("2");
    }
    void BmMove()
    {

        if (isDamage == false)
        {
            transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime * followSpeed);
            BManim.SetBool("BmMove", true);
        }
      
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(Findboxpos.position, FindboxSize);//DrawWireCube(pos.position,boxsize)          
    }


}
