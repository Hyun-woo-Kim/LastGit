using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class PlayerControllerRope : MonoBehaviour
{
    
    Grapling  grapling;
    Hooking hooking;

    Rigidbody2D rigid;
    public Animator animatorPlayer;
    SpriteRenderer sprPlayer;

    public PlayerData playerData;

    [Header("##Player Obj Grapling Rotation")]
    public float distanceSpeed = 0.1f;       // 가속도
    public float releaseDeceleration = 0.9f;
    public float initialGraplingSpeed = 0.0f;  // 초기 속도
    public float graplingMaxSpeed_X = 5.0f; // 최대 속도
    public float graplingMaxSpeed_Y = 5.0f; // 최대 속도
    public float accelerationRate = 2.5f;       // 가속도

    [Header("##Player BasicAttack")]
    public Transform attackpos;
    public Vector2 baseAtkboxSize;
    public float baseAtkCount = 0.0f;
    public float baseAtkTime;
    public int atkcount = 0;

    [Header("##Player Basic")]
    public float InitSpeed = 8.0f; //기본 초기값 속도
    public Transform playerPos;
    public Vector2 playerColliderBox;
    public bool isGrounded = true; //점프 조건 bool변수 
    public float jumpForce = 8f;

    [Header("##Player Elevator")]
    public bool isSelectUI = false;
    public bool isElevator = false;
    public bool isFlyAction = false;
    public bool isStopMove = false;


    public float raycastDistance = 50f;  // Raycast 최대 거리
    public LayerMask npcLayerMask;        // NPC 레이어 마스크 설정

    int direction;
    float detect_range = 1.5f;
    GameObject scanObject;

    void Start()
    {
        playerData.curSpeed = InitSpeed;
        grapling = GetComponent<Grapling>();
        
        rigid = GetComponent<Rigidbody2D>();
        animatorPlayer = GetComponent<Animator>();
        sprPlayer = GetComponent<SpriteRenderer>();


        playerData.playerHp = 10; //리스폰시 체력


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isMoveStop == false && isRestraint == false)
        {
            Move();
        }

        //Landing Paltform
        Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0)); //빔을 쏨(디버그는 게임상에서보이지 않음 ) 시작위치, 어디로 쏠지, 빔의 색 

        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));
        //빔의 시작위치, 빔의 방향 , 1:distance , ( 빔에 맞은 오브젝트를 특정 레이어로 한정 지어야할 때 사용 ) // RaycastHit2D : Ray에 닿은 오브젝트 클래스 

        //rayHit는 여러개 맞더라도 처음 맞은 오브젝트의 정보만을 저장(?) 
        if (rigid.velocity.y < 0)
        { // 뛰어올랐다가 아래로 떨어질 때만 빔을 쏨 
            if (rayHit.collider != null)
            { //빔을 맞은 오브젝트가 있을때  -> 맞지않으면 collider도 생성되지않음 
              // if (rayHit.distance < 0.5f)
              //animator.SetBool("isJumping", false); //거리가 0.5보다 작아지면 변경

            }
        }

        Debug.DrawRay(rigid.position, new Vector3(direction * detect_range, 0, 0), new Color(0, 0, 1));

        //Layer가 Object인 물체만 rayHit_detect에 감지 
        RaycastHit2D rayHit_detect = Physics2D.Raycast(rigid.position, new Vector3(direction, 0, 0), detect_range, LayerMask.NameToLayer("Object"));

        //감지되면 scanObject에 오브젝트 저장 
        if (rayHit_detect.collider != null)
        {
            scanObject = rayHit_detect.collider.gameObject;
            Debug.Log(scanObject.name);

        }
        else
        {
            scanObject = null;
        }

    }
   
    Vector3 mouse;

    public float baseSwingForce = 10f;

    private void Update()
    {
       

        PlayerCollider();
        if (Input.GetButtonDown("Jump") && isGrounded && grapling.isAttatch == false)
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


        if (Input.GetKeyDown(KeyCode.K))
        {

            if (scanObject != null)
            {
                GameManager.instance.Action(scanObject);
                //Debug.Log(scanObject.name);
            }


        }
    }

    void PlayerCollider()
    {
        GameObject elevatorObject = GameObject.Find("Elevator");
        Transform firstChildTransform = elevatorObject.transform.GetChild(0);
        SpriteRenderer spriteRenderer = firstChildTransform.GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.white;

        isElevator = false;
        isStopMove = false;
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(playerPos.position, playerColliderBox, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            if(collider.CompareTag("Elevator"))
            {            
                spriteRenderer.color = Color.red;

                isElevator = true;
            }
            if(collider.CompareTag("Ring") && grapling.isFlyReady) //공중제비시 충돌 관련 코드.
            {
                isFlyAction = true;
                isStopMove = true;
                Debug.Log("ac");
            }
        }
        if (isElevator == true)
        {
            if(Input.GetKey(KeyCode.F))
            {                
                PlayerUI.Instance.UIActive();
            }
        }
      
    }

 
    void AttackCool()
    {
        if (atkcount >= 1) //기본 공격 카운트가 1보다 클 때
        {
            baseAtkTime -= Time.deltaTime; //기본 공격 시간이 2에서 계속 줄어든다.
            if (baseAtkTime <= 0.0f)//기본 공격 시간이 0보다 작을 때(0이 됐을 때)
            {
                baseAtkTime = 0.5f; //다시 처음으로 기본 공격시간을 2.0초로 맞춘다.
                atkcount = 0;      // 기본 공격 카운트가 0으로 맞춘다.
              
            }

        }
    }




    void PlayerJump()
    {
        
        rigid.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        isGrounded = false;

        if (isGrounded ==  false)
        {
            animatorPlayer.SetBool("PlayerJump0",true);
        }       

    }

    public bool isLava;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("땅 충돌");
            isGrounded = true;
            animatorPlayer.SetBool("PlayerJump0", false);

        }
        if (collision.gameObject.name == "Lava")
        {
            Debug.Log("용암 충돌");
            isLava = true;
        }
       

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        //3.점프 관련 코드와 점프 조건 변수
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            
        }
        if (collision.gameObject.name == "Lava")
        {
            Debug.Log("용암 충돌");
            isLava = false;
        }
      
    }





    //public DistanceJoint2D distanceJoint; // DistanceJoint2D 컴포넌트

    public bool isMovementRestricted = false;

    void Move()
    {

        float horizontalInput = Input.GetAxis("Horizontal");

        if (grapling.grapCount != 1.0f && grapling.isFlyReady == false )
        {

            if(grapling.isLerping == false )
            {
                MoveToPlayer(horizontalInput);
            }


            animatorPlayer.SetBool("PlayerAimEnemy", false); //조준애니메이션 해제.
            Time.timeScale = 1.0f;

        }
        else if (grapling.grapCount == 1.0f) //적에게 갈고리 걸렸을  때
        {
            animatorPlayer.SetBool("PlayerAimEnemy", true); //조준애니메이션 발동.
            Time.timeScale = 0.5f;
        }

      
     
        
    }

  
    void MoveToPlayer(float horizontalInput)
    {
        //if (grapling.isAttatch == false && SelectManager.Instance.isSelectUI == false)
        if (PlayerUI.Instance.isSelectUI == false && grapling.isAttatch == false && isMoveStop == false)
        {

            if (horizontalInput > 0) //else if (horizontalInput < 0 && grapling.isLerping == false)
            {
                
                Vector2 moveDirection = new Vector2(horizontalInput, 0);
                rigid.velocity = new Vector2(moveDirection.x * playerData.curSpeed, rigid.velocity.y);
                transform.localScale = new Vector3(1, 1, 1);
                direction = 2;
              
                //transform.eulerAngles = new Vector3(0, 0, 0);
                //sprPlayer.flipX = false;
                if (grapling.isFlyReady == false)
                {
                    animatorPlayer.SetFloat("Position_X", moveDirection.x); 
                }
                

            }
            else if (horizontalInput < 0) // else if (horizontalInput < 0 && grapling.isLerping == false)
            {
                //Debug.Log("일반적인 움직임");
                Vector2 moveDirection = new Vector2(-horizontalInput, 0);
                rigid.velocity = new Vector2(-moveDirection.x * playerData.curSpeed, rigid.velocity.y);
                transform.localScale = new Vector3(-1, 1, 1);
                direction = -2;

                //transform.eulerAngles = new Vector3(0, 180, 0);
                //sprPlayer.flipX = true;
                if (grapling.isFlyReady == false)
                {
                    animatorPlayer.SetFloat("Position_X", moveDirection.x);
                }

            }



        }
       
    }

    #region 전등 그래플링 360도 회전 메서드
    //public void SwingPlayer()
    //{
    //    Hooking hook = GameObject.Find("Hook").GetComponent<Hooking>();
    //    Vector2 currentConnectedAnchor = hook.joint2D.connectedAnchor;

    //    if (grapling.hookisLeft && grapling.isAttatch)
    //    {

    //        if (Input.GetKey(KeyCode.A))
    //        {

    //            currentConnectedAnchor.x -= accelerationRate * Time.deltaTime;


    //            currentConnectedAnchor.x = Mathf.Max(currentConnectedAnchor.x, -graplingMaxSpeed_X); //1
    //                                                                                                 //currentConnectedAnchor.y  = Mathf.Max(currentConnectedAnchor.x, -graplingMaxSpeed_Y);

    //            if (currentConnectedAnchor.y < 3.0f)
    //            {
    //                currentConnectedAnchor.y += distanceSpeed * Time.deltaTime;
    //            }


    //        }


    //        else if (Input.GetKey(KeyCode.D))
    //        {


    //            currentConnectedAnchor.x += accelerationRate * Time.deltaTime;



    //            if (currentConnectedAnchor.y < 3.0f)
    //            {
    //                currentConnectedAnchor.y += distanceSpeed * Time.deltaTime;
    //            }

    //            currentConnectedAnchor.x = Mathf.Min(currentConnectedAnchor.x, graplingMaxSpeed_X);


    //        }

    //        else
    //        {
    //            //currentConnectedAnchor.x = 0.0f;

    //            //currentConnectedAnchor.y = 0.0f;
    //            currentConnectedAnchor.x = Mathf.Lerp(currentConnectedAnchor.x, initialGraplingSpeed, releaseDeceleration * Time.deltaTime);
    //            currentConnectedAnchor.y = Mathf.Lerp(currentConnectedAnchor.y, initialGraplingSpeed, releaseDeceleration * Time.deltaTime);


    //        }

    //    }
    //    else if (grapling.hookisRight)
    //    {

    //        if (Input.GetKey(KeyCode.A))
    //        {

    //            currentConnectedAnchor.x += accelerationRate * Time.deltaTime;



    //            if (currentConnectedAnchor.y < 3.0f)
    //            {
    //                currentConnectedAnchor.y += distanceSpeed * Time.deltaTime;
    //            }

    //            currentConnectedAnchor.x = Mathf.Min(currentConnectedAnchor.x, graplingMaxSpeed_X);


    //        }
    //        else if (Input.GetKey(KeyCode.D))
    //        {
    //            currentConnectedAnchor.x -= accelerationRate * Time.deltaTime;
    //            //currentConnectedAnchor.y -= accelerationRate * Time.deltaTime;

    //            currentConnectedAnchor.x = Mathf.Max(currentConnectedAnchor.x, -graplingMaxSpeed_X); //1
    //                                                                                                 //currentConnectedAnchor.y  = Mathf.Max(currentConnectedAnchor.x, -graplingMaxSpeed_Y);

    //            if (currentConnectedAnchor.y < 3.0f)
    //            {
    //                currentConnectedAnchor.y += distanceSpeed * Time.deltaTime;
    //            }


    //        }

    //        else
    //        {
    //            //currentConnectedAnchor.x = 0.0f;

    //            //currentConnectedAnchor.y = 0.0f;
    //            currentConnectedAnchor.x = Mathf.Lerp(currentConnectedAnchor.x, initialGraplingSpeed, releaseDeceleration * Time.deltaTime);
    //            currentConnectedAnchor.y = Mathf.Lerp(currentConnectedAnchor.y, initialGraplingSpeed, releaseDeceleration * Time.deltaTime);


    //        }
    //    }

    //    hook.joint2D.connectedAnchor = currentConnectedAnchor;
    //} 
    #endregion  //제외 함수

    public void flyAction(float swingForce) //공중제비 메서드
    {

        Debug.Log(swingForce);
        Hooking hookingScr = GameObject.Find("Hook").GetComponent<Hooking>();

        Vector3 playerdir = grapling.hook.transform.position - transform.position;

        float flyangle = Mathf.Atan2(playerdir.y, playerdir.x) * Mathf.Rad2Deg;

        Vector2 flyDirection = Quaternion.Euler(0, 0, flyangle) * Vector2.right;

        rigid.velocity = flyDirection * swingForce;
        //rigid.AddForce(flyDirection * swingForce, ForceMode2D.Impulse);

        hookingScr.joint2D.connectedAnchor = new Vector2(0.0f, 0.0f);

        hookingScr.joint2D.enabled = false;

    }



    IEnumerator PlayerAttack()
    {
        // animatorPlayer.SetFloat("Atk_Blend", baseAtkCount);
        animatorPlayer.SetTrigger("PlayerAttack");

        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(attackpos.position, baseAtkboxSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.CompareTag("Enemy"))
            {

                Enemies enemyScript = collider.GetComponentInParent<Enemies>(); //적에게 데미지 주는 함수 호출 코드@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                if (enemyScript != null)
                {
                    StartCoroutine(enemyScript.EnemyAtkStop());
                    StartCoroutine(enemyScript.baseDamaged());
                }
                else
                {
                    Debug.Log("Enemies인터페이스 찾지 못함");
                }

                animatorPlayer.SetTrigger("PlayerAttack");


            }
        }

        yield return null;

    }
 

    public bool isMoveStop;
    public IEnumerator BMSkillMove(Transform bmPos, float _nockbackForce)
    {
        Debug.Log("넉백");
        isMoveStop = true;
        Vector2 knockbackVector = bmPos.position.x > transform.position.x ? Vector2.left : Vector2.right;
        Debug.Log(knockbackVector);
        rigid.AddForce(knockbackVector * _nockbackForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        isMoveStop = false;
    }

    public bool isRestraint;
    public IEnumerator PpRestraint(float time)
    {
        //속박 애니메이션 재생 
        animatorPlayer.SetTrigger("PlayerHit");
        Debug.Log("속박 이동 정지");
        isRestraint = true;
        animatorPlayer.SetFloat("Position_X", 0);
        yield return new WaitForSeconds(time);
        isRestraint = false;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireCube(attackpos.position, baseAtkboxSize);//DrawWireCube(pos.position,boxsize)      
        Gizmos.DrawWireCube(playerPos.position, playerColliderBox);//DrawWireCube(pos.position,boxsize)      
    }


}
