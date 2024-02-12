using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritureController : MonoBehaviour,Enemies
{
    Rigidbody2D rigidCr;
    SpriteRenderer sprCr;
    PlayerControllerRope player;
    Animator animEnemy;
    CapsuleCollider2D capsuleColl;

    private Enemies bw;

    public IEnumerator GraplingAtkDamaged()
    {
    
        sprCr.color = Color.red;
        CancelInvoke();
        moveCr = 0;
        animEnemy.SetTrigger("EnemyHit");
        yield return new WaitForSeconds(1.0f);
        sprCr.color = Color.white;
        Think();
        Debug.Log("�⺻ ���Ͱ� �������� ����");
        yield return null;
    }

    private void Start()
    {
        animEnemy = GetComponent<Animator>();
        rigidCr = GetComponent<Rigidbody2D>();
        sprCr = GetComponent<SpriteRenderer>();
        capsuleColl = GetComponent<CapsuleCollider2D>();
        player = GameObject.Find("Player").GetComponent<PlayerControllerRope>();
        grapling = GameObject.Find("Player").GetComponent<Grapling>();

        bw = GetComponentInParent<Enemies>();
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

            if (grapling.isLerping == false) //�÷��̾� �׷��ø� ���� ���� ���� ������ȯ
            {
                localScale();
            }
       

            Invoke("Think", 3);//���
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
  
    public IEnumerator baseDamagedCriture()
    {
        // Debug.Log("ũ���İ� ���ظ� �Ծ���.");
        sprCr.color = Color.red;
        CancelInvoke();
        animEnemy.SetTrigger("EnemyHit");
        yield return new WaitForSeconds(1.0f);
        sprCr.color = Color.white;
        Think();
    }

    public Transform Enemyattackpos;
    public Vector2 EnemyattackBoxSize;


    public Transform targetPos;
    public Vector2 targetPlayer;
    public bool isEnemyAttack;
    public bool isPlayerCheack;

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

                    isPlayerCheack = true;
                    isEnemyAttack = true;
                    isbaseMove = false;
                    targetPos = coll.transform;
                    targetPlayer = coll.transform.position;
                }

            }
            
        }
        

        if (isEnemyAttack == true)
        {
            Attack(targetPos);
        }
        
        else if(isPlayerCheack)
        {
            Enemies enemy = GetComponentInChildren<Enemies>();

            if (enemy != null)
            {
                Debug.Log(enemy);
                // Enemies �������̽��� ������ Ŭ������ �ν��Ͻ��� ��쿡�� ���
                if (enemy is BloodWorkerAction bloodWorkerAction)
                {
                    // EnemyB Ŭ������ �ν��Ͻ��� ��쿡�� ���
                    bool specialEnemyValue = bloodWorkerAction.isTeamEnemy;
                    Debug.Log("Special Enemy (EnemyB): " + specialEnemyValue);
                }
                else
                {
                    Debug.Log("EnemyB Ŭ������ ã�� ����");
                }
            }
            else
            {
                Debug.Log("Enemies �������̽��� ������ Ŭ������ ã�� ����");
            }
        }
    }

    public float speed = 1.0f; // �̵� �ӵ�
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Wall")) //������ȯ 
        {
            moveCr *= -1;
            localScale();
        }
    }

    public float attackSpeed;
    void Attack(Transform target)
    {
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
