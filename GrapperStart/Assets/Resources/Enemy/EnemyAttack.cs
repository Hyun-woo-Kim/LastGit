using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame



    void Update()
    {
        
           // EnemyToAttack();
        

    }
    //public Transform Enemyattackpos;
    //public Vector2 EnemyattackBoxSize;


    // Transform targetPos;
    //public bool isEnemyAttack;
    //void EnemyToAttack()
    //{
    //    isEnemyAttack = false;

    //    Collider2D[] collider = Physics2D.OverlapBoxAll(Enemyattackpos.transform.position, EnemyattackBoxSize, 0);

    //    foreach(Collider2D coll in collider)
    //    {
    //        if(coll.CompareTag("Player"))
    //        {
    //            Debug.Log("추격 준비");
    //            isEnemyAttack = true;
    //            targetPos = coll.transform;
    //        }
    //    }

    //    if(isEnemyAttack == true)
    //    {
    //        Debug.Log("타겟 넘김");
    //        Attack(targetPos);
    //    }

    //}

    //public float attackSpeed;
    //void Attack(Transform target)
    //{
    //    Debug.Log("추격 시작");
    //    if(transform.position.x > target.position.x)
    //    {
    //        //CancelInvoke();
    //    }
    //    CancelInvoke();
    //    transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime * attackSpeed);
    //}

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;

    //    Gizmos.DrawWireCube(Enemyattackpos.position, EnemyattackBoxSize);//DrawWireCube(pos.position,boxsize) 
    //}

}
