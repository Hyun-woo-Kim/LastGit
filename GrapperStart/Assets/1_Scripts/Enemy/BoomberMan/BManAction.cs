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

    public IEnumerator GraplingAtkDamaged(float damage)
    {
        yield return null;
    }

    public bool isDamage;
    public IEnumerator baseDamaged()
    {
        bmdata.DamagedHp(1);
        // BManim.SetTrigger("BmNockBack");

        BManim.SetTrigger("BmNockBack");
        for (int i =0; i< 3; i++)
        {
        
            BManim.SetTrigger("BmHandAttack");
            yield return new WaitForSeconds(0.1f);
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
            }
        }
        if(isFindPlayer)
        {
            BmMove();
        }
    }

    void BmMove()
    {
        transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime * followSpeed);
        if(isDamage == false)
        {
            BManim.SetBool("BmMove", true);
        }
      
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(Findboxpos.position, FindboxSize);//DrawWireCube(pos.position,boxsize)          
    }


}
