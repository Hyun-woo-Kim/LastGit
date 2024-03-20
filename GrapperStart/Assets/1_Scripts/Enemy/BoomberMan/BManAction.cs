using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BManAction : MonoBehaviour,Enemies
{
    Animator BManim;
    CapsuleCollider2D collder;
    Rigidbody2D Bmrigid;
    public BMdata bmdata;

    Vector2 InitCollSize;
    void Start()
    {
        BManim = GetComponent<Animator>();
        collder = GetComponent<CapsuleCollider2D>();
        Bmrigid = GetComponent<Rigidbody2D>();
        //hand.gameObject.SetActive(false);
        animSpeed = BManim.speed;
        InitCollSize = collder.size;
       
 
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
    public float animAimSpeed;
    public float animSpeed;

    

    public IEnumerator GraplingAtkDamaged(float damage)
    {
        yield return null;
    }

    public bool isDamage;
    public IEnumerator baseDamaged()
    {
        isMove = false;
        bmdata.DamagedHp(1);

        if (bmdata.bmHp <= 1.0f)
        {
            StartCoroutine(Died());
        }
        isDamage = true;
        
        
        BManim.SetBool("BmAtk", true);
        BManim.SetFloat("BmAtkCount", -1.0f);

        for (int i =0; i< 2; i++)
        {
           BManim.SetFloat("BmAtkCount", 0.0f);                 
           yield return new WaitForSeconds(0.5f);
        }
        
        //BManim.SetBool("BmAtk", false);
        
        isMove = true;
    }

    public void PlayerToDamaged()
    {
        PlayerControllerRope player = GameObject.Find("Player").GetComponent<PlayerControllerRope>();
        PlayerData playerData = player.playerData;
        playerData.DamagedHp(1);
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

        if (isColliderPlayer) //isDied상태에서 플레이어와 충돌 시
        {
            PlayerControllerRope playerController = FindAnyObjectByType<PlayerControllerRope>();
            if (playerController != null)
            {
                playerController.BMSkillMove(transform, nockbackForce); // 적의 위치를 전달
            }
            DestroyBM(1.0f);
        }

    }
    public float nockbackForce;
    public void SpeedDown()
    {
        followSpeed = 0.1f;
        BManim.speed = animAimSpeed;
    }

    public void EnemySet()
    {
        followSpeed = 0.2f;
        BManim.speed = animSpeed;
    }

    public bool hasAttacked;
    public bool isMove;


    void FindedPlayer()
    {
        Transform fourChild = transform.GetChild(5);
        SpriteRenderer spriteRenderer = fourChild.GetComponent<SpriteRenderer>();


        isFindPlayer = false;
        isReact = false;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(Findboxpos.transform.position, FindboxSize, 0);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                //Vector2 colliderBm = new Vector2(1.3f, 3.0f);
                //collder.size = colliderBm;
                target = collider.transform;
                isFindPlayer = true;
                isReact = true;
                FunchCollider();
                if ((isFindPlayer || isFindEnemy) && isMove == false)
                {
                    Debug.Log("일어서기");
                    StartCoroutine(Find()); //1
                }


            }
            if (collider.CompareTag("Enemy"))
            {
                //if (isFindEnemy == false)
                //{
                //    BloodWorkerAction bwScr = FindFirstObjectByType<BloodWorkerAction>();
                //    if (bwScr.isBasicDamaged || bwScr.isGraplingDamaged)
                //    {
                //        Debug.Log("아군 발견");
                //        isFindEnemy = true;
                //    }

                //}
            }
        }

        if ((isFindPlayer ||isFindEnemy)&& isMove)
        {

            BmMove(); //3
        }
        else if (!isFindPlayer)
        {

            StartCoroutine(NotFind());
        }



        PatrolReaction(spriteRenderer);
    }
   


    public bool isTeamDamage;
    void TeamEnemy()
    {
        GameObject[] allies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject ally in allies)
        {
            // 아군 스크립트가 부착되어 있는지 확인
            BloodWorkerAction allyScript = ally.GetComponent<BloodWorkerAction>();
            if (allyScript != null)
            {
                // 아군 스크립트가 적대적인 상태인지 확인
                if (allyScript.isBasicDamaged || allyScript.isGraplingDamaged)
                {
                    isTeamDamage = true;
                    StartCoroutine(Find());
                    Debug.Log("아군이 공격 받음");
                    // 아군이 공격을 받았을 때의 처리 수행
                }
                else if(!allyScript.isBasicDamaged || !allyScript.isGraplingDamaged)
                {
                    //isFindEnemy = false;
                    Debug.Log("아군이 공격 받지 않음");
                }
            }
            else
            {

            }
        }
    }
    public bool isFindEnemy;
    void BmMove()
    {

        if (isDamage == false && isDied == false && isMove)
        {
            FlipEnemy(target);
            transform.position = Vector2.Lerp(transform.position, target.position, Time.deltaTime * followSpeed);
            if (!BManim.GetBool("BmAtk"))
            {
                Debug.Log("이동 중");
                BManim.SetFloat("BmAnimCount", 1.0f);
            }
        }

    }
    IEnumerator  NotFind()
    {
        isMove = false;
        yield return new WaitForSeconds(1.0f);
        BManim.SetBool("BmAtk", false);
        //스탠딩 애니메이션 여기다 추가.
        BManim.SetBool("BmFind", false);
        isStandUp = false;
        isFindEnemy = false;
        collder.size = InitCollSize;

    }
    public float delay;
    public bool isStandUp;
    IEnumerator Find()
    {
        //움직이기 전
        isMove = false;
        isStandUp = true;

        BManim.SetBool("BmFind",true);
        BManim.SetFloat("BmAnimCount",0.0f);
        yield return new WaitForSeconds(1.0f);
        
        Vector2 colliderBm = new Vector2(1, 2.6f);
        collder.size = colliderBm;
        yield return new WaitForSeconds(2.0f);
        isMove = true; //2
        PlayerControllerRope playeScr = FindAnyObjectByType<PlayerControllerRope>();
        target = playeScr.transform;
        //움직이기 
    }

    public Transform Punchboxpos;
    public Vector2 PunchboxSize;

    public bool isReact;
    public Sprite reactSprite;
    void FunchCollider()
    {
        hasAttacked = false;
        Collider2D[] colliders = Physics2D.OverlapBoxAll(Punchboxpos.transform.position, PunchboxSize, 0);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                hasAttacked = true;
            }
        }

        if(isDamage)
        {
            StartCoroutine(HandAttack());
        }
      
    }

    public bool firstAnimationFinished = true;
    public bool secondAnimationFinished = false;

    IEnumerator HandAttack()
    {
        while (isFindPlayer && hasAttacked)
        {
            if (firstAnimationFinished)
            {
                BManim.SetBool("BmAtk", true);
                BManim.SetFloat("BmAtkCount", 0); // 첫 번째 애니메이션 실행
                firstAnimationFinished = false;

                // 애니메이션 종료 대기
                yield return new WaitUntil(() => BManim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
                secondAnimationFinished = true;
                Debug.Log("첫 번째 애니메이션 실행");
            }
            else if (secondAnimationFinished)
            {
                BManim.SetBool("BmAtk", true);
                BManim.SetFloat("BmAtkCount", 1); // 두 번째 애니메이션 실행
                secondAnimationFinished = false;

                // 애니메이션 종료 대기
                yield return new WaitUntil(() => BManim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);
                firstAnimationFinished = true;
                Debug.Log("두 번째 애니메이션 실행");
            }

            Debug.Log("한 프레임 대기");
            yield return null;
        }

        isDamage = false;
        BManim.SetBool("BmAtk", false);
        BmMove();
    }

    public bool isColliderPlayer;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(isDied)
            {
                isColliderPlayer = true;
            }
            
        }
    }
    void FlipEnemy(Transform _target)
    {
        if (transform.position.x > _target.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (transform.position.x < _target.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
    void PatrolReaction(SpriteRenderer spriteRenderer)
    {        
        if (isReact || isTeamDamage)
        {
            spriteRenderer.sprite = reactSprite;
        }
        else if (isReact == false)
        {
            spriteRenderer.sprite = null;

        }
    }
    void DestroyBM(float delay)
    {
        Destroy(this.gameObject, delay);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireCube(Findboxpos.position, FindboxSize);//DrawWireCube(pos.position,boxsize)          
        Gizmos.DrawWireCube(Punchboxpos.position, PunchboxSize);//DrawWireCube(pos.position,boxsize)          
    }


}
