using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritureController : MonoBehaviour, Enemies
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

    public void EnemySet()
    {
        speedCr = 5.0f;
    }
    public void SpeedDown()//인터페이스 Enemies를 통해 구현해야 할 전등 조준시 속도 낮추는 메서드.
    {
        speedCr = 2.0f;
    }

    public void PlayerToDamaged() //인터페이스 Enemies를 통해 구현해야 할 플레이어에게 데미지 주는 메서드.
    {
        Debug.Log("플레이어 타격");
        PlayerControllerRope player = GameObject.Find("Player").GetComponent<PlayerControllerRope>();
        PlayerData playerData = player.playerData;
        playerData.DamagedHp(1);
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
            //Debug.Log("기본 움직임 시작");

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
    public IEnumerator GraplingAtkDamaged(float damage)
    {
        Debug.Log("크리쳐가 피해를 입었다.");
        sprCr.color = Color.red;
        CancelInvoke();
        moveCr = 0;
        animEnemy.SetTrigger("EnemyHit");
        yield return new WaitForSeconds(1.0f);
        sprCr.color = Color.white;
        Think();
    }

    public IEnumerator baseDamaged()
    {
        // Debug.Log("크리쳐가 피해를 입었다.");
        sprCr.color = Color.red;
        CancelInvoke();
        animEnemy.SetTrigger("EnemyHit");
        yield return new WaitForSeconds(1.0f);
        sprCr.color = Color.white;
        Think();
    }

    public IEnumerator Died()
    {
        yield return new WaitForSeconds(1.0f);

    }

    public Transform Enemyattackpos;
    public Vector2 EnemyattackBoxSize;


    public Transform targetPos;
    public bool isEnemyAttack;
    public bool isTeamBW;
    void EnemyToAttack()
    {
        isEnemyAttack = false;
        isTeamBW = false;
        isbaseMove = true;

        Collider2D[] collider = Physics2D.OverlapBoxAll(Enemyattackpos.transform.position, EnemyattackBoxSize, 0);

        
        foreach (Collider2D coll in collider) //시야
        {
           if (coll.CompareTag("Player"))
           {
                  
                    isEnemyAttack = true;
                    isbaseMove = false;
                    targetPos = coll.transform;

                  
           }
        }
            
            if (isEnemyAttack == true)
            {
                //Debug.Log("타겟 넘김");
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
        //Debug.Log("추격 시작");
        
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
        Gizmos.color = Color.black ;

        Gizmos.DrawWireCube(Enemyattackpos.position, EnemyattackBoxSize);//DrawWireCube(pos.position,boxsize) 
    }

    public void UpdateOutline(bool outline)
    {
        throw new System.NotImplementedException();
    }
}
