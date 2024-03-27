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
    public bool isAtk;
    public IEnumerator baseDamaged()
    {
        yield return null;
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
    public bool isPlayerFindCoroutineRunning = false;


    void FindedPlayer()
    {
        Transform fourChild = transform.GetChild(5);
        SpriteRenderer spriteRenderer = fourChild.GetComponent<SpriteRenderer>();


        
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

                if (!isPlayerFindCoroutineRunning && isFindPlayer) // 코루틴이 실행 중이 아닌 경우에만 실행
                {
                    StartCoroutine(playerFind());
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

        PatrolReaction(spriteRenderer);
    }
    public float growthRate = 0.9f; //일어나는 속도
    IEnumerator playerFind()
    {
        isPlayerFindCoroutineRunning = true;
        BManim.SetBool("BmFind", true);

        float sizeY = 1.7f;
        float targetSizeY = 2.7f; // 목표 크기

        while (sizeY <= targetSizeY)
        {
            float delta = growthRate * Time.deltaTime;
            sizeY += delta;

            // 크기가 목표 크기를 초과하는 경우 종료
            if (sizeY > 2.6f)
            {
                break;
            }

            Vector2 colliderBm = new Vector2(1.0f, sizeY);
            collder.size = colliderBm;

            BManim.SetFloat("BmAnimCount", -1.0f);

            yield return null;
            Debug.Log("반복");
        }
        //Vector2 offsetBm = new Vector2(0.3f, 0.0f);
        //collder.offset  = offsetBm;
        BManim.SetBool("BmIdle",true);
        yield return null;
        Debug.Log("종료");
    }


    public bool isTeamDamage;
    void TeamEnemy()
    {
        //GameObject[] allies = GameObject.FindGameObjectsWithTag("Enemy");
        //foreach (GameObject ally in allies)
        //{
        //    // 아군 스크립트가 부착되어 있는지 확인
        //    BloodWorkerAction allyScript = ally.GetComponent<BloodWorkerAction>();
        //    if (allyScript != null)
        //    {
        //        // 아군 스크립트가 적대적인 상태인지 확인
        //        if (allyScript.isBasicDamaged || allyScript.isGraplingDamaged)
        //        {
        //            isTeamDamage = true;
        //            StartCoroutine(Find());
        //            Debug.Log("아군이 공격 받음");
        //            // 아군이 공격을 받았을 때의 처리 수행
        //        }
        //        else if(!allyScript.isBasicDamaged || !allyScript.isGraplingDamaged)
        //        {
        //            //isFindEnemy = false;
        //            Debug.Log("아군이 공격 받지 않음");
        //        }
        //    }
        //    else
        //    {

        //    }
        //}
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

    public Transform Punchboxpos;
    public Vector2 PunchboxSize;

    public bool isReact;
    public Sprite reactSprite;


    public float atkAnimSpeed;
    public float AnimSpeed;
    private float atkCount;


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
