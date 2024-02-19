using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapling : MonoBehaviour
{
    [Header("#Hook Object")]
    public LineRenderer line;
    public Transform hook; //������ ��ġ�� ��Ÿ���� Transform �����Դϴ�.


    GraplingRange graplingRange;
    Animator animPlayer;
    Aiming aim;

    [Header("##Obj Grapling")]
    public Transform playerArm;
    public bool isHookActive = false;
    public bool isAttatch = false;
    public float hookDelSpeed;
    public float hookMoveSpeed;
    public float rotationSpeed = 5.0f; // ���� ������ ȸ�� �ӵ�
    private PlayerArm playerArmScr;
    public bool isFlyReady = false; //�������� �غ� ����
    public bool isEKeyHeld = false; //EŰ ��������
    private float eKeyHoldTime = 0f; //EŰ ������ �ð�
    public float PlayerGraplingAnimCount; //�׷��ø� ���� ���� Ʈ�� �ִϸ��̼� ���� 
    public float baseSwingForce; //�������� ���� �� ���� 
    private Quaternion originalRotation; // ������ ȸ������ ������ ����
    public bool isLineMax;
    [Header("##Aim & Grapling")]
    public bool iscollObj = false;
    public bool iscollenemy = false;

    [Header("##Enemy Grapling")]
    public float grapCount = 0.0f;
    public bool isGrap = false;
    public bool isLerping = false; // Lerp ������ ���θ� ��Ÿ���� ����
    public float lerpTime;
    public Transform enemyPosition;
    private float grapanimdelay = 0.3f;
    public Transform enemyHookPos;
    public bool isenemyGrapling;



  

    //������ �׸��� �������� �ΰ��� ����.
    //�� ���� Player�� ������: positionCount
    //�� ���� Hook�� ������: SetPosition

    PlayerControllerRope player;




    void Start()
    {


        animPlayer = GetComponent<Animator>();


        graplingRange = FindAnyObjectByType<GraplingRange>();
        aim = FindAnyObjectByType<Aiming>();

        player = GameObject.Find("Player").GetComponent<PlayerControllerRope>();
        HookSet();

        originalRotation = transform.rotation;

        #region Line������Ʈ ����
        //����
        //�����ϸ� ���� ������ 2���� �����Ǿ� ó�� ���� ��ġ�� ���� ������ ������ ��ġ�� �� . �� 2���� �����ȴ�.
        //�ε��� 0�� 1�� �ʺ�� 0.05�� ��� �����.
        //Line������Ʈ�� ���Ե� LineRenderer�� �ε��� 0�� ��ġ(���� ������ ù ��ġ)�� �÷��̾��� ��ġ�� �����ǰ�, 
        // �ε��� 1�� ��ġ(���� ������ ������ ��ġ��) Hook������Ʈ�� ��ġ�� �����ȴ�. �׸��� �� ������ ������ ���� �������� �����ȴ�.
        //line.SetPosition(0, this.transform.position); 
        //�ε��� 0�� ��ġ�� �� ��ũ��Ʈ�� ����ִ� ������Ʈ�� �� ��ġ�� �����Ѵ�.
        //line.SetPosition(1, this.hook.position); 
        //�ε��� 1�� ��ġ�� �� ��ũ��Ʈ�� ����ִ� hook�� ��ġ�� �����Ѵ�. 
        //SetPosition(index, position): Ư�� �ε����� ���� ��ġ�� �����մϴ�. ������ �׸��� �� �� ���� ��ġ�� ������ �� �ֽ��ϴ�.
        //positionCount: ������ ���� ������ �����մϴ�. �� �Ӽ��� ����Ͽ� ������ ��� ����Ǵ��� ������ �� �ֽ��ϴ�.
        //useWorldSpace: ������ ������ ���� ����(���� ��ǥ)���� ��ġ�ϵ��� ����, ���� ����(������Ʈ�� ���� ��ǥ)���� ��ġ�ϵ��� ������ �����մϴ�.
        #endregion 
    }


    void HookSet()
    {
        line.positionCount = 2;
        line.endWidth = line.startWidth = 0.025f;  //startWidth: ������ ���� �κ��� �ʺ� �����մϴ�.//endWidth: ������ �� �κ��� �ʺ� �����մϴ�.�̷ν� ������ �� �κп��� �󸶳� �о����ų� ���������� ������ �� �ֽ��ϴ�.

        line.useWorldSpace = true; //���� �������� ��ġ�ϵ��� ��. 

        SpriteRenderer hookspr = hook.GetComponent<SpriteRenderer>();
        hookspr.color = Color.white;

        line.startColor = Color.white;
        line.endColor = Color.white;
        isAttatch = false;



    }



    void Update()
    {
        GraplingSkill();


        if (graplingRange.isRange == true && isAttatch == false && graplingRange.isenemySkill == true) //�����ȿ� �־�� �ϸ�, ���� �ȿ� ���� �־�� �ϸ�, ���� �������� �ʾ��� ����
        {
            //isRange == true , isenemySkill == true
            GraplingEnmeyHookPos();
        }




        if (graplingRange.isRange == true) //���� ���� ���� and ���� �ȿ� �� ���� �� and ���� �ȿ� ���� ���� ��
        {
            if (grapCount == 1.0f && graplingRange.isenemySkill == false)
            {
                grapCount = 0.0f;
                animPlayer.SetBool("EnemyGrapling", false);
                // hokkobj.GetComponent<SpriteRenderer>().color;
                StartCoroutine(HookColor());
                aim.isAimEnemy = false;
            }


        }
        else if (graplingRange.isRange == false && graplingRange.isenemySkill == false)
        {

            hook.gameObject.SetActive(false);

            aim.isAimEnemy = false;


        }

        if (isHookActive)
        {
            RotData();
        }


    }

    public void GraplingEnmeyHookPos()
    {
        if (isLerping == false)
        {
            Transform aimPos = transform.GetChild(5);
            line.SetPosition(0, aimPos.position);
        }
        else if (isLerping == true)
        {
            Debug.Log("AD");
            line.SetPosition(0, new Vector3(enemyHookPos.position.x, enemyHookPos.position.y + 0.2f, enemyHookPos.position.z));
        }



    }


    public void ResetGrap()
    {
        grapCount = 0.0f;
        hook.gameObject.SetActive(false);
        isAttatch = false;
        isHookActive = false;
        isLineMax = false;

    }
    IEnumerator HookColor()
    {
        SpriteRenderer hookspr = hook.GetComponent<SpriteRenderer>();
        hookspr.color = Color.red;

        animPlayer.SetBool("EnemyGrapling", false);
        line.startColor = Color.red;
        line.endColor = Color.red;
        line.endWidth = line.startWidth = 0.1f;
        yield return new WaitForSeconds(0.3f);
        hook.gameObject.SetActive(false);
        HookSet();

    }



    public void RotData()
    {
     

        playerArmScr = FindObjectOfType<PlayerArm>();

        //PlayerArm arm = GameObject.FindObjectsOfType("Player_Arm").GetComponentInChildren<PlayerArm>();
        if (playerArmScr != null)
        {
            playerArmScr.rotationArm(hook.transform.position); //���� ȸ���ϴ� �޼��� ȣ��
            Vector3 playerdir = hook.transform.position - transform.position;
            float hookangle = Mathf.Atan2(playerdir.y, playerdir.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(hookangle - 90f, Vector3.forward);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);//�÷��̾� ȸ���ϴ� �޼��� ȣ��
        }

        else
        {
            Debug.Log("���� ã�� ����");
        }


    }



    public float hookDistance;

    public bool hookisLeft;
    public bool hookisRight;




    public void GraplingSkill() //�� �κ���Ʈ ���� �׷��ø� ��ų 
    {

        // line.SetPosition(0, playerArm.position);

        line.SetPosition(1, hook.GetComponent<Hooking>().hooklinePos.position);

        if (Input.GetKeyDown(KeyCode.E) &&
            isHookActive == false && iscollObj == true)
        {
            hook.gameObject.SetActive(true);

            //hook�� ���� ���� ��ġ�� playerAimPos
            line.SetPosition(0, aim.playerAimPos.position);
            iscollObj = false;

            hook.position = aim.playerAimPos.position; //hook�� ���� ��ġ�� playerAimPos.

            isHookActive = true;
            isLineMax = false;
         


            if (transform.position.x > hook.transform.position.x)
            {
                hookisLeft = true;
                hookisRight = false;
                hook.GetComponent<Hooking>().hookSpr.flipX = false;
            }
            else if (transform.position.x < hook.transform.position.x)
            {
                hookisRight = true;
                hookisLeft = false;
                hook.GetComponent<Hooking>().hookSpr.flipX = true;
            }


        }
        if (isHookActive == true && isLineMax == false && isAttatch == false)
        {

            float distanceFromHookPos = Vector2.Distance(aim.playerAimPos.position, transform.position); //�� ��ġ

            //Hook������Ʈ�� ���ư� ������.

            hook.Translate(aim.aimMousedir.normalized * Time.deltaTime * hookMoveSpeed * 1.5f);

        }
        else if (isHookActive == true && isLineMax == true)
        {
            //Hook������Ʈ�� ���� �� �� ����.       

            // hook.position = Vector2.MoveTowards(hook.position, playerArm.transform.position, Time.deltaTime * hookDelSpeed); //hook���� transform��ġ�� hookDelSpeed�ӵ��� �̵��Ѵ�.(hook�� ���ƿ´�)
            if (Vector2.Distance(playerArm.transform.position, hook.position) < 0.1f)
            {
                Debug.Log("ADADADA");
                isHookActive = false;
                isLineMax = false;
                hook.gameObject.SetActive(false);

            }



        }
        else if (isAttatch == true)
        {
            player.SwingPlayer();

            PlayerGraplingAnimCount = 0.0f;
            animPlayer.SetBool("PlayerGrapling", true);
            animPlayer.SetFloat("PlayerGraplingCount", PlayerGraplingAnimCount);

            line.SetPosition(0, playerArm.position);

            playerArm.gameObject.SetActive(true);

            if (Input.GetKey(KeyCode.E) && isAttatch) //�������� ���� ���� @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            {


                if (!isEKeyHeld)
                {
                    // E Ű�� ó������ ������ �� ������ �ڵ�

                    isEKeyHeld = true;
                }

                // E Ű�� ������ �ִ� ���� ��� ������ �ڵ�
                eKeyHoldTime += Time.deltaTime;

            }
            else
            {
                if (isEKeyHeld)
                {
                    // E Ű�� ������ �� ������ �ڵ�

                    PlayerGraplingAnimCount++;

                    isFlyReady = true;
                    isStop = true;
                    isAttatch = false;
                    isHookActive = false;
                    isLineMax = false;

                    if (eKeyHoldTime >= 0.5f) // 1�� �̻� ������ ��
                    {
                        baseSwingForce = 20.0f;
                        player.flyAction(baseSwingForce);
                    }
                    else
                    {
                        baseSwingForce = 15.0f;
                        player.flyAction(baseSwingForce);
                    }

                    
                    aim.aimMousedir.x = 0;
                    aim.aimMousedir.y = 0;
                    aim.aimLength = 0.0f;
                    isEKeyHeld = false;
                    eKeyHoldTime = 0f; // ������ �ִ� �ð� �ʱ�ȭ
                }
            }//�������� ���� ���� @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

        }

        if (isFlyReady == true)
        {
            transform.rotation = originalRotation;
            Debug.Log("�������� ��");
            animPlayer.SetFloat("PlayerGraplingCount", PlayerGraplingAnimCount);

            GameObject playerObj = GameObject.Find("Player");
            Transform playerArm = playerObj.transform.GetChild(7);

            playerArm.gameObject.SetActive(false);
            hook.gameObject.SetActive(false);

            GameObject ground = GameObject.Find("Ground");
            Transform groundPos = ground.transform;
            float verticalDistanceToGround = Mathf.Abs(transform.position.y - groundPos.position.y);
            if (verticalDistanceToGround < 3f && player.isFlyAction == true)
            {
                Debug.Log("�������� �� �ٴڿ� ��� ���� �������� ����");

                player.isFlyAction = false;
                isFlyReady = false;
                animPlayer.SetBool("PlayerGrapling", false);

            }
        }


    }


    public bool isStop;
    public float delay;
    //1. ������Ʈ �ɸ� ����. 2. eŰ�� ���� . 3.�������� ����. 4. playerArm�� hook�� ��������� �� ������Ʈ ����.

    public void GrapHandling(GameObject target)
    {
        isenemyGrapling = true;
        GraplingPlayerFlip(target);

        hook.position = enemyHookPos.position; //hook ó�� ��ġ�� �� ���� �߻� �ִϸ��̼� �� ��ġ�� �ʱ�ȭ.

        grapCount += 1.0f;

        animPlayer.SetBool("EnemyGrapling", true);
        animPlayer.SetFloat("EnemyGraplingCount", grapCount);

        if (grapCount == 1.0f && graplingRange.isenemySkill == true)
        {
            aim.isAimObject = false;

            hook.gameObject.SetActive(true);
            hook.GetComponent<Hooking>().target = target.transform;

        }
        if (grapCount == 2.0f)
        {

            aim.isAimEnemy = false;

            hook.position = target.transform.position;

            StartCoroutine(LerpToTarget(target.transform, target));
        }

    }


    IEnumerator LerpToTarget(Transform targetPosition, GameObject enemyObj)
    {
        enemyPosition = targetPosition;

        aim.isAimEnemy = false;
        gameObject.layer = 8;

        if (enemyObj != null)
        {
            isLerping = true;
            float elapsedTime = 0f;
            Vector3 startingPos = transform.position;

            while (elapsedTime < lerpTime)
            {
                //2. �÷��̾� ȸ��
                //  transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, angle - 99.0f), rotationSpeed * Time.deltaTime); 
                //targetPosition = enemyPosition.position;

                hook.position = enemyObj.transform.position;
                transform.position = Vector3.Lerp(startingPos, enemyPosition.transform.position, Mathf.SmoothStep(0f, 1f, elapsedTime / lerpTime));
                //Mathf.SmoothStep(float edge0, float edge1, float t); -> Mathf.SmoothStep(���� ��, �� �� , ���� ���� �� ���� ���� �μ�)
                // t�� 0���� �϶� ���� ���̸�, t�� 1 �̻� �϶� ��� ��.
                //�� �Լ��� Ư�� ���۰� �� �κ��� �ε巴�� ����� �߰� �κ��� �� ������ ���ӵǰų� ���ӵǴ� ���� ȿ���� �����ϴ� �� ���
                //Vector3.Lerp�Լ��� ���� ���� �ε巯����. 

                elapsedTime += Time.deltaTime;
                yield return null;
            }



            if (Vector2.Distance(transform.position, enemyObj.transform.position) < 3.0f)
            {
                gameObject.layer = 7;
                hook.gameObject.SetActive(false);

                animPlayer.SetBool("EnemyGrapling", false);


                GraplingPlayerFlip(enemyObj);
                animPlayer.SetTrigger("PlayerAttack");
                yield return new WaitForSeconds(grapanimdelay);

                Enemies enemyScript = enemyObj.GetComponentInParent<Enemies>(); //������ ������ �ִ� �Լ� ȣ�� �ڵ�@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                if (enemyScript != null)
                {
                    StartCoroutine(enemyScript.GraplingAtkDamaged());
                }
                else
                {
                    Debug.Log("Enemies�������̽� ã�� ����");
                }
            }

            transform.rotation = originalRotation;


            grapCount = 0.0f;
            animPlayer.SetFloat("EnemyGraplingCount", grapCount);
            isLerping = false; // Lerp ����
            isenemyGrapling = false; //�� �׷��ø� ����
        }

    }


    void GraplingPlayerFlip(GameObject enemy)
    {

        if (enemy.transform.localScale.x == 1)
        {
            if (transform.localScale.x == -1)
            {
                if (enemy.transform.position.x > transform.position.x)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }

            }
            if (transform.localScale.x == 1)
            {
                if (enemy.transform.position.x < transform.position.x)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
            }

        }
        if (enemy.transform.localScale.x == -1)
        {
            if (transform.localScale.x == -1)
            {
                if (enemy.transform.position.x > transform.position.x)
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }

            }
            if (transform.localScale.x == 1)
            {
                if (enemy.transform.position.x < transform.position.x)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                }
            }

        }




    }





}
