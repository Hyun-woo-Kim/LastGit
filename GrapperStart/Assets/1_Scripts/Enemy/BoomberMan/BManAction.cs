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
        hand.gameObject.SetActive(true);
        for (int i =0; i< 2; i++)
        {
            BManim.SetTrigger("BmHandAttack");
            yield return new WaitForSeconds(0.5f);
        }
        hand.gameObject.SetActive(false);
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
                target = collider.transform;
                isFindPlayer = true;

                if (!hasAttacked)
                {
                    Debug.Log("A");
                    hand.gameObject.SetActive(true);
                    BManim.SetTrigger("BmBoomHandAttack");
                    StartCoroutine(stopMove());

                }
            }
        }

        if(isFindPlayer)
        {
            Debug.Log("B");
            hasAttacked = true;
           
            BmMove();

        }
        else if (!isFindPlayer && hasAttacked)
        {
            hasAttacked = false;
            isMove = false;
        }
     
    }
    IEnumerator stopMove()
    {

        Debug.Log("C");
        yield return new WaitForSeconds(delay);
        hand.gameObject.SetActive(false);
        isMove = true;

        Debug.Log("D");
    }
    public float delay;
    void BmMove()
    {
        Debug.Log("E");
        if (isDamage == false && isMove)
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
