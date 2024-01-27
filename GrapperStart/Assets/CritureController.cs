using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritureController : MonoBehaviour
{
    Rigidbody2D rigidCr;
    SpriteRenderer sprCr;
    PlayerControllerRope player;
    Animator animEnemy;
    CapsuleCollider2D capsuleColl;
    private void Start()
    {
        animEnemy = GetComponent<Animator>();
        rigidCr = GetComponent<Rigidbody2D>();
        sprCr = GetComponent<SpriteRenderer>();
        capsuleColl = GetComponent<CapsuleCollider2D>();
        player = GameObject.Find("Player").GetComponent<PlayerControllerRope>();
        grapling = GameObject.Find("Player").GetComponent<Grapling>();
        Think();
    }
    public int moveCr;
    public float speedCr;
    private void FixedUpdate()
    {
        aiMoveCriture();
        EnemyToAttack();
    }

    void aiMoveCriture()
    {
        if(grapling.isLerping == true)
        {
            speedCr = 0.5f;
            moveCr = (int)Random.Range(-speedCr, speedCr);
        }
        else
        {
            speedCr = 5.0f;
        }
        rigidCr.velocity = new Vector2(moveCr * speedCr, rigidCr.velocity.y);
        animEnemy.SetFloat("PosionX", moveCr);
    }

    Grapling grapling;


    public bool isbaseMove;
    void Think()
    {
        isbaseMove = true;
        if(isbaseMove == true)
        {
            moveCr = Random.Range(-1, 2);

            if (grapling.isLerping == false) //플레이어 그래플링 하지 않을 때만 방향전환
            {
                localScale();
            }
            Debug.Log("기본 움직임 시작");

            Invoke("Think", 3);//재귀
        }

           
        
       
    }

    void localScale()
    {
        if (moveCr == -1)
        {
            transform.localScale = new Vector3(moveCr, 1, 1);
        }
        else if(moveCr == 1)
        {
            transform.localScale = new Vector3(moveCr, 1, 1);
        }
      
    }
    public IEnumerator AtkDamagedCriture()
    {
        // Debug.Log("크리쳐가 피해를 입었다.");
        sprCr.color = Color.red;
        CancelInvoke();
        moveCr = 0;
        animEnemy.SetTrigger("EnemyHit");
        yield return new WaitForSeconds(1.0f);
        sprCr.color = Color.white;
        Think();
    }

    public IEnumerator baseDamagedCriture()
    {
        // Debug.Log("크리쳐가 피해를 입었다.");
        sprCr.color = Color.red;
        CancelInvoke();
        animEnemy.SetTrigger("EnemyHit");
        yield return new WaitForSeconds(1.0f);
        sprCr.color = Color.white;
        Think();
    }

    public Transform Enemyattackpos;
    public Vector2 EnemyattackBoxSize;


    Transform targetPos;
    public bool isEnemyAttack;
    void EnemyToAttack()
    {
        isEnemyAttack = false;
        isbaseMove = true;

        Collider2D[] collider = Physics2D.OverlapBoxAll(Enemyattackpos.transform.position, EnemyattackBoxSize, 0);

        if(isEnemyAttack == false)
        {
            foreach (Collider2D coll in collider)
            {
                if (coll.CompareTag("Player"))
                {
                    Debug.Log("추격 준비");
                    isEnemyAttack = true;
                    isbaseMove = false;
                    targetPos = coll.transform;
                }

            }
            
        }
      

        if (isEnemyAttack == true)
        {
            Debug.Log("타겟 넘김");
            Attack(targetPos);
        }
        
        

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Wall")) //방향전환 
        {
            moveCr *= -1;
            localScale();
        }
    }

    public float attackSpeed;
    void Attack(Transform target)
    {
        Debug.Log("추격 시작");
        
        if (transform.position.x > target.position.x)
        {
            //CancelInvoke();
            moveCr = -1;
            localScale();
        }
        else if(transform.position.x < target.position.x)
        {
            //CancelInvoke();
            moveCr = 1;
            localScale();
        }
      
        transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime * attackSpeed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(Enemyattackpos.position, EnemyattackBoxSize);//DrawWireCube(pos.position,boxsize) 
    }
}
