using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        if(isMoveStop == false)
        {
            Move();
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


        // 카메라의 Forward 방향으로 Raycast 발사
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        // 레이캐스트 실행
        if (Physics.Raycast(ray, out hit, raycastDistance, npcLayerMask))
        {
            // 충돌한 객체가 "NPC" 태그를 가지고 있는지 확인
            if (hit.collider.CompareTag("Npc"))
            {
                Debug.Log("NPC found: " + hit.collider.name);
                // NPC를 찾았을 때의 추가 처리 (예: NPC에게 알림 보내기, 효과 재생 등)
            }
        }

        // 기즈모를 사용하여 Raycast 시각화
        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red);
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireCube(attackpos.position, baseAtkboxSize);//DrawWireCube(pos.position,boxsize)      
        Gizmos.DrawWireCube(playerPos.position, playerColliderBox);//DrawWireCube(pos.position,boxsize)      
    }


    //Item 관련
    private bool iDown;
    GameObject nearItem;
    public GameObject[] Items;
    public bool[] hasItems;

    void Interation()
    {
        if (iDown && nearItem != null)
        {
            if (nearItem.tag == "Weapon")
            {
                ItemTest item = nearItem.GetComponent<ItemTest>();
                int ItemIndex = item.value;
                hasItems[ItemIndex] = true;

                Destroy(nearItem);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            nearItem = other.gameObject;
            Debug.Log(nearItem.name);
        }
       
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            nearItem = null;
        }
    }

       


}
