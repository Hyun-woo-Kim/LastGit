using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerRope : MonoBehaviour
{
    Grapling  grapling;

    Hooking hooking;

    
    Rigidbody2D rigid;
    Animator animatorPlayer;
    SpriteRenderer sprPlayer;


    GameObject Objcr;
    void Start()
    {
        
        grapling = GetComponent<Grapling>();
        
        rigid = GetComponent<Rigidbody2D>();
        animatorPlayer = GetComponent<Animator>();
        sprPlayer = GetComponent<SpriteRenderer>();

        Objcr = GameObject.FindGameObjectWithTag("Enemy");



    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();


    }

    private void Update()
    {

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            PlayerJump();
        }
       

        if (Input.GetKeyDown(KeyCode.X))
        {
            if (atkcount == 0) //기본공격 카운트가 0일 때 ,처음은 0으로 시작
            {
                atkcount++;
                StartCoroutine(PlayerAttack());
            }
        }

    

        AttackCool();
    }


    public float baseAtkTime;
    public int atkcount = 0;
    void AttackCool()
    {
        if (atkcount >= 1) //기본 공격 카운트가 1보다 클 때
        {
            baseAtkTime -= Time.deltaTime; //기본 공격 시간이 2에서 계속 줄어든다.
            if (baseAtkTime <= 0.0f)//기본 공격 시간이 0보다 작을 때(0이 됐을 때)
            {
                baseAtkTime = 0.5f; //다시 처음으로 기본 공격시간을 2.0초로 맞춘다.
                atkcount = 0;      // 기본 공격 카운트가 0으로 맞춘다.
                Debug.Log("기본공격 쿨타임 초기화");
            }

        }
    }


    public bool isGrounded = true; //점프 조건 bool변수 
    public float jumpForce = 8f;

    public bool isPlayerfly = false;
    void PlayerJump()
    {
        
        rigid.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        //isGrounded = false;
        isPlayerfly = false;

        if (isPlayerfly ==  false)
        {
            animatorPlayer.SetTrigger("PlayerJump");
        }
        

        





    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isPlayerfly = false;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        //3.점프 관련 코드와 점프 조건 변수
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
           
        }

    }

    public float curSpeed;



    //public DistanceJoint2D distanceJoint; // DistanceJoint2D 컴포넌트

    

    void Move()
    {

        float horizontalInput = Input.GetAxis("Horizontal");

        if (grapling.grapCount != 1.0f && grapling.isAttatch == false ) //&& grapling.isAttatch == false
        {

            if(grapling.isLerping == false )
            {
                MoveToPlayer(horizontalInput);
            }

            animatorPlayer.SetBool("PlayerAimEnemy", false); //조준애니메이션 해제.
            Time.timeScale = 1.0f;

        }
        if (grapling.grapCount == 1.0f) //적에게 갈고리 걸렸을  때
        {
            animatorPlayer.SetBool("PlayerAimEnemy", true); //조준애니메이션 발동.
            Time.timeScale = 0.5f;
        }

        
    }

    void MoveToPlayer(float horizontalInput)
    {
        if (grapling.isAttatch == false && isPlayerfly == false)
        {

            if (horizontalInput > 0) //else if (horizontalInput < 0 && grapling.isLerping == false)
            {
                Debug.Log("링에게 안걸렸을 때 D키 눌렀을 때 ");
                Vector2 moveDirection = new Vector2(horizontalInput, 0);
                rigid.velocity = new Vector2(moveDirection.x * curSpeed, rigid.velocity.y);
                transform.localScale = new Vector3(1, 1, 1);

                //transform.eulerAngles = new Vector3(0, 0, 0);
                //sprPlayer.flipX = false;
                animatorPlayer.SetFloat("Position_X", moveDirection.x);

            }
            else if (horizontalInput < 0) // else if (horizontalInput < 0 && grapling.isLerping == false)
            {
                Debug.Log("링에게 안걸렸을 때 A키 눌렀을 때 ");
                Vector2 moveDirection = new Vector2(-horizontalInput, 0);
                rigid.velocity = new Vector2(-moveDirection.x * curSpeed, rigid.velocity.y);
                transform.localScale = new Vector3(-1, 1, 1);
                //transform.eulerAngles = new Vector3(0, 180, 0);
                //sprPlayer.flipX = true;
                animatorPlayer.SetFloat("Position_X", moveDirection.x);

            }



        }
    }

    public float swingForce = 5f;  // 단진자 운동에 가해지는 힘



    public float swing_x;
    public float swing_y;
    public void SwingPlayer()
    {
       
         Hooking hook = GameObject.Find("Hook").GetComponent<Hooking>();
        Vector2 currentConnectedAnchor = hook.joint2D.connectedAnchor;




        if (grapling.hookisLeft)
        {

            Debug.Log("갈고리 날렸을 때 후크는 왼쪽에 있다.");


            if (Input.GetKey(KeyCode.A))
            {
              
                //currentConnectedAnchor.x -= accelerationRate * Time.deltaTime;
                //currentConnectedAnchor.y -= accelerationRate * Time.deltaTime;

                //currentConnectedAnchor.x = Mathf.Max(currentConnectedAnchor.x, -graplingMaxSpeed_X); //1
                                                                                                     //currentConnectedAnchor.y  = Mathf.Max(currentConnectedAnchor.x, -graplingMaxSpeed_Y);
                currentConnectedAnchor.x = swing_x;
                currentConnectedAnchor.y= swing_y;
                //if (currentConnectedAnchor.y < 2.0f)
                //{
                //    currentConnectedAnchor.y += distanceSpeed * Time.deltaTime;
                //}


            }


            else if (Input.GetKey(KeyCode.D))
            {


                //currentConnectedAnchor.x += accelerationRate * Time.deltaTime;



                //if (currentConnectedAnchor.y < 2.0f)
                //{
                //    currentConnectedAnchor.y += distanceSpeed * Time.deltaTime;
                //}

                //currentConnectedAnchor.x = Mathf.Min(currentConnectedAnchor.x, graplingMaxSpeed_X);
                
                currentConnectedAnchor.x = -swing_x;
                currentConnectedAnchor.y = swing_y;
            }

            else
            {
                //currentConnectedAnchor.x = 0.0f;

                //currentConnectedAnchor.y = 0.0f;
                currentConnectedAnchor.x = Mathf.Lerp(currentConnectedAnchor.x, initialGraplingSpeed, releaseDeceleration * Time.deltaTime);
                currentConnectedAnchor.y = Mathf.Lerp(currentConnectedAnchor.y, initialGraplingSpeed, releaseDeceleration * Time.deltaTime);


            }

        }
        else if (grapling.hookisRight)
        {
            Debug.Log("갈고리 날렸을 때 후크는 오른쪽에 있따");
            if (Input.GetKey(KeyCode.A))
            {

                currentConnectedAnchor.x += accelerationRate * Time.deltaTime;



                if (currentConnectedAnchor.y < 2.0f)
                {
                    currentConnectedAnchor.y += distanceSpeed * Time.deltaTime;
                }

                currentConnectedAnchor.x = Mathf.Min(currentConnectedAnchor.x, graplingMaxSpeed_X);


            }
            else if (Input.GetKey(KeyCode.D))
            {
                currentConnectedAnchor.x -= accelerationRate * Time.deltaTime;
                //currentConnectedAnchor.y -= accelerationRate * Time.deltaTime;

                currentConnectedAnchor.x = Mathf.Max(currentConnectedAnchor.x, -graplingMaxSpeed_X); //1
                                                                                                     //currentConnectedAnchor.y  = Mathf.Max(currentConnectedAnchor.x, -graplingMaxSpeed_Y);

                if (currentConnectedAnchor.y < 2.0f)
                {
                    currentConnectedAnchor.y += distanceSpeed * Time.deltaTime;
                }


            }

            else
            {
                //currentConnectedAnchor.x = 0.0f;

                //currentConnectedAnchor.y = 0.0f;
                currentConnectedAnchor.x = Mathf.Lerp(currentConnectedAnchor.x, initialGraplingSpeed, releaseDeceleration * Time.deltaTime);
                currentConnectedAnchor.y = Mathf.Lerp(currentConnectedAnchor.y, initialGraplingSpeed, releaseDeceleration * Time.deltaTime);


            }
        }

        hook.joint2D.connectedAnchor = currentConnectedAnchor;
    }
    
    public float playerdir;
    public void hookAnchorSet()
    {
       


     
        Hooking hookingScr = GameObject.Find("Hook").GetComponent<Hooking>();

        Vector3 playerdir = grapling.hook.transform.position - transform.position;

        float flyangle = Mathf.Atan2(playerdir.y, playerdir.x) * Mathf.Rad2Deg;

        Vector2 flyDirection = Quaternion.Euler(0, 0, flyangle) * Vector2.right;

        rigid.velocity = flyDirection * swingForce;
        //rigid.AddForce(flyDirection * swingForce, ForceMode2D.Impulse);

        hookingScr.joint2D.connectedAnchor = new Vector2(0.0f, 2.0f);

        hookingScr.joint2D.enabled = false;

    }


    public float initialDistance = 2.0f;
    public float distanceSpeed = 0.1f;       // 가속도

    public float releaseDeceleration = 0.9f;

    public float initialGraplingSpeed = 0.0f;  // 초기 속도

    public float graplingMaxSpeed_X = 5.0f; // 최대 속도
    public float graplingMaxSpeed_Y = 5.0f; // 최대 속도

    public float accelerationRate = 0.5f;       // 가속도
    public float graplingSpeed;

    public Transform attackpos;
    public Vector2 baseAtkboxSize;
    public float baseAtkCount = 0.0f;

    IEnumerator PlayerAttack()
    {
        // animatorPlayer.SetFloat("Atk_Blend", baseAtkCount);
        animatorPlayer.SetTrigger("PlayerAttack");

        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(attackpos.position, baseAtkboxSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.CompareTag("Enemy"))
            {
                //cameraShake.VibrateForTime(0.5f);//흔들리는 시간 0.5를 넘김(UI매니저 스크립트 참고)              
                //baseAtkCount += 1.0f;

                //collider.GetComponent<SkullController>().OnDamaged(1);
                ////collider.GetComponent<SkullController>().TakeDamaged(1);
                //StartCoroutine(enemySkull.EnemyDamaged(1, collider));
                //animatorPlayer.SetFloat("Atk_Blend", (float)baseAtkCount);

                CritureController enemyCr = Objcr.GetComponent<CritureController>();
                StartCoroutine(enemyCr.baseDamagedCriture());

                animatorPlayer.SetTrigger("PlayerAttack");


            }
        }

        yield return null;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireCube(attackpos.position, baseAtkboxSize);//DrawWireCube(pos.position,boxsize)      
    }
}
