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

    public PlayerData playerData;

    [Header("##Player Obj Grapling Rotation")]
    public float distanceSpeed = 0.1f;       // ���ӵ�
    public float releaseDeceleration = 0.9f;
    public float initialGraplingSpeed = 0.0f;  // �ʱ� �ӵ�
    public float graplingMaxSpeed_X = 5.0f; // �ִ� �ӵ�
    public float graplingMaxSpeed_Y = 5.0f; // �ִ� �ӵ�
    public float accelerationRate = 2.5f;       // ���ӵ�

    [Header("##Player BasicAttack")]
    public Transform attackpos;
    public Vector2 baseAtkboxSize;
    public float baseAtkCount = 0.0f;
    public float baseAtkTime;
    public int atkcount = 0;

    [Header("##Player Basic")]
    public float InitSpeed = 8.0f; //�⺻ �ʱⰪ �ӵ�
    public Transform playerPos;
    public Vector2 playerColliderBox;
    public bool isGrounded = true; //���� ���� bool���� 
    public float jumpForce = 8f;

    [Header("##Player Elevator")]
    public bool isSelectUI = false;
    public bool isElevator = false;
    public bool isFlyAction = false;
    public bool isStopMove = false;


   

    void Start()
    {
        playerData.curSpeed = InitSpeed;

        grapling = GetComponent<Grapling>();
        
        rigid = GetComponent<Rigidbody2D>();
        animatorPlayer = GetComponent<Animator>();
        sprPlayer = GetComponent<SpriteRenderer>();


        playerData.playerHp = 10; //�������� ü��


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move();


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
            if (atkcount == 0) //�⺻���� ī��Ʈ�� 0�� �� ,ó���� 0���� ����
            {
                atkcount++;
                StartCoroutine(PlayerAttack());
            }
        }



        AttackCool();
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
            if(collider.CompareTag("Ring") && grapling.isFlyReady) //��������� �浹 ���� �ڵ�.
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
                SelectManager.Instance.UIActive();
            }
        }
      
    }

 
    void AttackCool()
    {
        if (atkcount >= 1) //�⺻ ���� ī��Ʈ�� 1���� Ŭ ��
        {
            baseAtkTime -= Time.deltaTime; //�⺻ ���� �ð��� 2���� ��� �پ���.
            if (baseAtkTime <= 0.0f)//�⺻ ���� �ð��� 0���� ���� ��(0�� ���� ��)
            {
                baseAtkTime = 0.5f; //�ٽ� ó������ �⺻ ���ݽð��� 2.0�ʷ� �����.
                atkcount = 0;      // �⺻ ���� ī��Ʈ�� 0���� �����.
              
            }

        }
    }




    void PlayerJump()
    {
        
        rigid.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        isGrounded = false;

        if (isGrounded ==  false)
        {
            animatorPlayer.SetTrigger("PlayerJump");
        }       

    }

    public bool isLava;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            
        }
        if (collision.gameObject.name == "Lava")
        {
            Debug.Log("��� �浹");
            isLava = true;
        }
       

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        //3.���� ���� �ڵ�� ���� ���� ����
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            
        }
        if (collision.gameObject.name == "Lava")
        {
            Debug.Log("��� �浹");
            isLava = false;
        }
      
    }

   



    //public DistanceJoint2D distanceJoint; // DistanceJoint2D ������Ʈ

    

    void Move()
    {

        float horizontalInput = Input.GetAxis("Horizontal");

        //if (grapling.grapCount != 1.0f && grapling.isAttatch == false && grapling.isFlyReady  == false) 
        if (grapling.grapCount != 1.0f && grapling.isFlyReady == false)
        {

            if(grapling.isLerping == false )
            {
                MoveToPlayer(horizontalInput);
            }


            animatorPlayer.SetBool("PlayerAimEnemy", false); //���ؾִϸ��̼� ����.
            Time.timeScale = 1.0f;

        }
        else if (grapling.grapCount == 1.0f) //������ ���� �ɷ���  ��
        {
            animatorPlayer.SetBool("PlayerAimEnemy", true); //���ؾִϸ��̼� �ߵ�.
            Time.timeScale = 0.5f;
        }

      
     
        
    }

    void MoveToPlayer(float horizontalInput)
    {
        //if (grapling.isAttatch == false && SelectManager.Instance.isSelectUI == false)
        if (SelectManager.Instance.isSelectUI == false && grapling.isAttatch == false)
        {

            if (horizontalInput > 0) //else if (horizontalInput < 0 && grapling.isLerping == false)
            {
                
                Vector2 moveDirection = new Vector2(horizontalInput, 0);
                rigid.velocity = new Vector2(moveDirection.x * playerData.curSpeed, rigid.velocity.y);
                transform.localScale = new Vector3(1, 1, 1);

                //transform.eulerAngles = new Vector3(0, 0, 0);
                //sprPlayer.flipX = false;
                if(grapling.isFlyReady == false)
                {
                    animatorPlayer.SetFloat("Position_X", moveDirection.x); 
                }
                

            }
            else if (horizontalInput < 0) // else if (horizontalInput < 0 && grapling.isLerping == false)
            {
                //Debug.Log("�Ϲ����� ������");
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

    #region ���� �׷��ø� 360�� ȸ�� �޼���
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
    #endregion  //���� �Լ�

    public void flyAction(float swingForce) //�������� �޼���
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

                Enemies enemyScript = collider.GetComponentInParent<Enemies>(); //������ ������ �ִ� �Լ� ȣ�� �ڵ�@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                if (enemyScript != null)
                {
                    StartCoroutine(enemyScript.EnemyAtkStop());
                    //StartCoroutine(enemyScript.baseDamaged());
                }
                else
                {
                    Debug.Log("Enemies�������̽� ã�� ����");
                }

                animatorPlayer.SetTrigger("PlayerAttack");


            }
        }

        yield return null;

    }
    public void BMSkillMove(Transform bmPos,float _nockbackForce)
    {
        Vector2 knockbackVector = bmPos.position.x > transform.position.x ? Vector2.left : Vector2.right;

        // �˹� ���͸� �̿��Ͽ� �˹� ����
        Debug.Log(knockbackVector);
        rigid.AddForce(knockbackVector * _nockbackForce, ForceMode2D.Impulse);

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawWireCube(attackpos.position, baseAtkboxSize);//DrawWireCube(pos.position,boxsize)      
        Gizmos.DrawWireCube(playerPos.position, playerColliderBox);//DrawWireCube(pos.position,boxsize)      
    }
}
