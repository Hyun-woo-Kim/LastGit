using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grapling : MonoBehaviour
{
    [Header("#Hook Object")]
    public LineRenderer line;
    public Transform hook; //������ ��ġ�� ��Ÿ���� Transform �����Դϴ�.


    GraplingRange graplingRange;
    Animator animPlayer;
    Aiming aim;
    Rigidbody2D rigid;
    CapsuleCollider2D playerColl;

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
    public bool hookisLeft;
    public bool hookisRight;
    [Header("##Aim & Grapling")]
    public bool iscollObj = false;
    public bool iscollenemy = false;

    [Header("##Enemy Grapling")]
    public float grapCount = 0.0f;
    public bool isGrap = false;
    public bool isLerping = false; // Lerp ������ ���θ� ��Ÿ���� ����
    public float lerpTime; // �� ���� Ŭ ���� �׷��ø� �ϴ� �ð��� ���ӵ�. �׷��ø� �ð�.
    public Transform enemyPosition;
    public float grapanimdelay;
    public Transform enemyHookPos;
    public bool isenemyGrapling;
    public float graplingDamage;


    private Camera cam;


    //������ �׸��� �������� �ΰ��� ����.
    //�� ���� Player�� ������: positionCount
    //�� ���� Hook�� ������: SetPosition

    PlayerControllerRope player;
    PlayerUI playerUI;



    void Start()
    {
        cam = Camera.main;

        animPlayer = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        playerColl = GetComponent<CapsuleCollider2D>();
        playerUI = FindFirstObjectByType<PlayerUI>();

        graplingRange = FindAnyObjectByType<GraplingRange>();
        aim = FindAnyObjectByType<Aiming>();

        player = GameObject.Find("Player").GetComponent<PlayerControllerRope>();
        HookSet();

        ghostDelaySeconds = ghostDelay;
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

    public bool isGraplingEnemy;

    void Update()
    {
      
        GraplingSkill();

        if (Input.GetKeyDown(KeyCode.E) && aim.isAimEnemy && aim.isCollEnemy && playerUI.isMPzero == false)
        {
            isGraplingEnemy = true;
            line.SetPosition(0, transform.GetChild(5).position);
            if (grapCount >= 0)
            {
                grapCount += 1.0f;

                if (grapCount == 1.0f)
                {

                    InstanComboSlider();
                }
                grapCounting();
            }


            animPlayer.SetBool("EnemyGrapling", true);
            animPlayer.SetFloat("EnemyGraplingCount", grapCount);

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


     

        if (isenemyGrapling)
        {
            Debug.Log("ȣ��");
            hook.GetComponent<Hooking>().rotationHook(transform.position);
        }
        else if (isHookActive)
        {
            hook.GetComponent<Hooking>().rotationHook(transform.position);
        }

        if(isMakeEnemyGrapligGhost || isMakeLightGrapligGhost)
        {
            GhostGraplingEffect();
        }

    }
    public float ghostDelay;
    private float ghostDelaySeconds;
    public GameObject ghost;
    public bool isMakeEnemyGrapligGhost = false;

    public GameObject ghostLight;
    public bool isMakeLightGrapligGhost = false;
    void GhostGraplingEffect()
    {
        if(isMakeEnemyGrapligGhost)
        {
            if (ghostDelaySeconds > 0)
            {
                ghostDelaySeconds -= Time.deltaTime;
            }
            else
            {
                GameObject currentGhost = Instantiate(ghost, transform.position, transform.rotation);
                Sprite currentSprite = GetComponent<SpriteRenderer>().sprite;
                currentGhost.transform.localScale = this.transform.localScale;
                currentGhost.GetComponent<SpriteRenderer>().sprite = currentSprite;
                ghostDelaySeconds = ghostDelay;
                Destroy(currentGhost, 1f);
            }
        }
        else if(isMakeLightGrapligGhost)
        {

            if (ghostDelaySeconds > 0)
            {
                ghostDelaySeconds -= Time.deltaTime;
            }
            else
            {
                GameObject currentGhost = Instantiate(ghostLight, transform.position, transform.rotation);
                Sprite currentSprite = GetComponent<SpriteRenderer>().sprite;
                currentGhost.transform.localScale = this.transform.localScale;
                currentGhost.GetComponent<SpriteRenderer>().sprite = currentSprite;
                ghostDelaySeconds = ghostDelay;
                Destroy(currentGhost, 1f);
            }
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



    public void RotPlayerArm()
    {
        playerArm.gameObject.SetActive(true);
        playerArmScr = FindObjectOfType<PlayerArm>();

        if (playerArmScr != null)
        {
            Debug.Log("5");
            playerArmScr.rotationArm(hook.transform.position); //���� ȸ���ϴ� �޼��� ȣ��
        }

        else
        {
            Debug.Log("���� ã�� ����");
        }


    }

    void RotPlayer()
    {
        Vector3 playerdir = hook.transform.position - transform.position;
        float hookangle = Mathf.Atan2(playerdir.y, playerdir.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(hookangle - 90f, Vector3.forward);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);//�÷��̾� ȸ���ϴ� �޼��� ȣ��

    }


    public void GraplingSkill() //�� �κ���Ʈ ���� �׷��ø� ��ų 
    {

        // line.SetPosition(0, playerArm.position);

        line.SetPosition(1, hook.GetComponent<Hooking>().hooklinePos.position);

        if (Input.GetKeyDown(KeyCode.E) &&
            isHookActive == false && iscollObj == true && playerUI.isMPzero == false)
        {
            isHookActive = true;
            hook.gameObject.SetActive(true);
            iscollObj = false;
            line.SetPosition(0, transform.GetChild(5).position);
            hook.position = aim.playerAimPos.position; //hook�� ���� ��ġ�� playerAimPos.

            isLineMax = false;

            hookRightLeft();
        }
        if (isHookActive == true && isLineMax == false && isAttatch == false)
        {

            //Hook������Ʈ�� ���ư� ������.
            hook.Translate(aim.aimMousedir.normalized * Time.deltaTime * hookMoveSpeed * 1.5f);

        }
        else if (isHookActive == true && isLineMax == true)
        {
            //Hook������Ʈ�� ���� �� �� ����.       

            // hook.position = Vector2.MoveTowards(hook.position, playerArm.transform.position, Time.deltaTime * hookDelSpeed); //hook���� transform��ġ�� hookDelSpeed�ӵ��� �̵��Ѵ�.(hook�� ���ƿ´�)
            if (Vector2.Distance(playerArm.transform.position, hook.position) < 0.1f)
            {
                isHookActive = false;
                isLineMax = false;
                hook.gameObject.SetActive(false);

            }



        }
        else if (isAttatch == true)
        {
            RotPlayerArm();
            RotPlayer();
            hookRigid();

            PlayerGraplingAnimCount = 0.0f;
            line.SetPosition(0, playerArm.position);

            animPlayer.SetFloat("PlayerGraplingCount", PlayerGraplingAnimCount);
            animPlayer.SetBool("PlayerGrapling", true);

            if (Input.GetKey(KeyCode.E) && isAttatch) 
            {
                rotationhook = transform.rotation.eulerAngles;

                Transform fifthChild = transform.GetChild(3);
                fifthChild.gameObject.SetActive(false);
              
                if (!isEKeyHeld)
                {

                    isEKeyHeld = true;
                }
;
            }
            else
            {

                if (isEKeyHeld)
                {
                    // E Ű�� ������ �� ������ �ڵ�
                    hookDetailDir();

                    playerUI.TakeGrapling(1.0f);

                    isFlyReady = true;
                    isStop = true;
                    isAttatch = false;
                    isHookActive = false;
                    isLineMax = false;

                    player.flyAction(baseSwingForce);

                    aim.aimMousedir.x = 0;
                    aim.aimMousedir.y = 0;
                    aim.aimLength = 0.0f;

                    isEKeyHeld = false;

                }
            }

        }

        if (isFlyReady == true)
        {
            LightGraplingStart();
        }
    }

    //���� �׷��ø� �� �޼���
    void LightGraplingStart()
    {
        transform.rotation = originalRotation;
        isMakeLightGrapligGhost = true;
        if (hookisLeft)
        {
            PlayerGraplingAnimCount = 1.0f;
            animPlayer.SetFloat("PlayerGraplingCount", PlayerGraplingAnimCount);
        }
        else if (hookisRight)//��ũ�� ������
        {
            PlayerGraplingAnimCount = -1.0f;
            animPlayer.SetFloat("PlayerGraplingCount", PlayerGraplingAnimCount);
        }


        GameObject playerObj = GameObject.Find("Player");
        Transform playerArm = playerObj.transform.GetChild(7);

        playerArm.gameObject.SetActive(false);
        hook.gameObject.SetActive(false);

        GameObject ground = GameObject.FindGameObjectWithTag("Ground");
        Transform groundPos = ground.transform;
        float verticalDistanceToGround = Mathf.Abs(transform.position.y - groundPos.position.y);
        if (verticalDistanceToGround < 3f && player.isFlyAction == true)
        {
            //aim.targetRing.GetComponentInChildren<BoxCollider2D>().isTrigger = true;
            Debug.Log("���� �浹 ����");
            player.isFlyAction = false;
            isFlyReady = false;
            playerColl.isTrigger = false;
            isWall = false;
            isMakeLightGrapligGhost = false;
            animPlayer.SetBool("PlayerGrapling", false);

        }
    }



    public float maxSwingAngle = 180f; // �ִ� ���� ����
    public float swingForce = 5f; // ���� ��
    public float hookDistance;
    void hookRigid()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        // ���� �Է��� �޾��� �� 180�� ������ ȸ���ϵ��� ����
        if (horizontalInput != 0)
        {
       
            if (horizontalInput > 0)
            {
                Vector2 swingDirection = Vector2.right;
                rigid.AddForce(swingDirection * swingForce, ForceMode2D.Force);

            }

            else if(horizontalInput < 0)
            {
                Vector2 swingDirection = Vector2.left;
                rigid.AddForce(swingDirection * swingForce, ForceMode2D.Force);
            }
            
            
          
        }
        
    }

    Vector3 rotationhook;
    void hookDetailDir()
    {
        if (transform.localScale.x == 1)
        {
            if ((rotationhook.z >= 0.0f && rotationhook.z <= 90.0f) || (rotationhook.z > 180.0f && rotationhook.z < 270.0f))
            {
                hookisLeft = true;
                hookisRight = false;
            }
            else if ((rotationhook.z > 90.0f && rotationhook.z < 180.0f) || (rotationhook.z < 360.0f && rotationhook.z > 270.0f))//��ũ�� ������
            {
                hookisRight = true;
                hookisLeft = false;
            }
        }
        else if (transform.localScale.x == -1)
        {
            if ((rotationhook.z >= 0.0f && rotationhook.z <= 90.0f) || (rotationhook.z > 180.0f && rotationhook.z < 270.0f))
            {
                hookisRight = true;
                hookisLeft = false;
            }
            else if ((rotationhook.z > 90.0f && rotationhook.z < 180.0f) || (rotationhook.z < 360.0f && rotationhook.z > 270.0f))//��ũ�� ������
            {
                hookisLeft = true;
                hookisRight = false;
            }
        }

    }
    public void hookRightLeft()
    {

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
    public bool isStop;
    public float delay;

    public void grapCounting()
    {

        isenemyGrapling = true;

        GraplingPlayerFlip(aim.EnemyObj);

        hook.position = enemyHookPos.position;

        if (grapCount == 1.0f && graplingRange.isenemySkill == true)
        {

            //InstanComboSlider();
            hook.gameObject.SetActive(true);

            hook.GetComponent<Hooking>().target = aim.EnemyObj.transform;


            //hook ó�� ��ġ�� �� ���� �߻� �ִϸ��̼� �� ��ġ�� �ʱ�ȭ.

        }
        
        else if (grapCount == 2.0f)
        {


            hook.position = aim.EnemyObj.transform.position;

            //GrapHandling(aim.EnemyObj);     
        }
    }

    public Vector2 comboBarPos;


    private Slider comboSliderPrefab;
    public Slider comboSlider;
    public float sliderSpeed;
    public float minPos;
    public float maxPos;
    private RectTransform pass;


    public Transform LookAt;
    public Vector3 Offset;
    public void InstanComboSlider()
    {
        Vector3 pos = cam.WorldToScreenPoint(LookAt.position + Offset);
        GameObject uiCanvas = GameObject.Find("UI_Canvas");

        if (uiCanvas != null && comboSliderPrefab == null)
        {
            //comboBarPos = Camera.main.WorldToScreenPoint(transform.GetChild(9).position);
            comboSliderPrefab = Instantiate(comboSlider, pos,Quaternion.identity);
            // comboBarUI�� UI_Canvas�� ������ ����
            comboSliderPrefab.transform.SetParent(uiCanvas.transform, false);

            SetComboSlider();

        }

        else
        {
            Debug.LogError("UI_Canvas not found");
        }


    }


    public void SetComboSlider()
    {
        Transform passTransform = comboSlider.transform.GetChild(1);
        pass = passTransform as RectTransform;

        comboSliderPrefab.value = 0.0f;
        //�����̴��� ����� �ʷϻ� �� �ȿ� �ִ� �� �˷��� �̹����� ��Ŀ �����ǰ� ��Ÿ �������� x���� �˾ƾ� ��.
        minPos = pass.anchoredPosition.x; //pass.anchoredPosition.x�� �ּҰ��� ��Ÿ������.
        maxPos = pass.sizeDelta.x + minPos;

        StartCoroutine(comboEnemyGrapling());
    }

    public float direction = 1.0f;
    IEnumerator comboEnemyGrapling()
    {
        yield return null;

        // �����̴� �̵� ����

        while (!(Input.GetKeyDown(KeyCode.E) || comboSliderPrefab.value == comboSliderPrefab.maxValue))
        {

            comboSliderPrefab.value += Time.deltaTime * sliderSpeed * direction;

            // �����̴� ���� max���� �ʰ��ϸ� �̵� ���� ����
            if (comboSliderPrefab.value >= comboSliderPrefab.maxValue)
            {
                direction = -1f; // �̵� ���� ����
                comboSliderPrefab.value += Time.deltaTime * sliderSpeed * direction;
            }

            // �����̴� ���� min�� �̸��̸� �̵� ���� ����
            if (comboSliderPrefab.value <= comboSliderPrefab.minValue)
            {
                direction = 1f;
                comboSliderPrefab.value += Time.deltaTime * sliderSpeed * direction;
            }

            yield return null;
        }


        if (comboSliderPrefab.value >= minPos && comboSliderPrefab.value <= maxPos)  // �����̴��� �ʷϻ� �ٿ� ��ġ ���� �ʾ��� ��
        {
            lerpTime = 0.8f;
            graplingDamage = 1.0f;
            GrapHandling(aim.EnemyObj, lerpTime, graplingDamage);
        }
        else if (comboSliderPrefab.value <= minPos || comboSliderPrefab.value >= maxPos) //�����̴��� �ʷϻ� �ٿ� ��ġ���� ��
        {
            lerpTime = 0.5f;
            graplingDamage = 2.0f;
            GrapHandling(aim.EnemyObj, lerpTime, graplingDamage);
        }
    }
    public void GrapHandling(GameObject enemy, float graplingTime, float damage)
    {
        // hook.position = enemy.transform.position;

        
         DestroyImmediate(comboSliderPrefab.gameObject);
         comboSliderPrefab = null; // ���� null�� ����


        StartCoroutine(LerpToTarget(enemy.transform, enemy, graplingTime, damage));
    }

    public float playerToEnemyDistance;
    IEnumerator LerpToTarget(Transform targetPosition, GameObject enemyObj, float graplingTime, float damage)
    {
        playerUI.TakeGrapling(1.0f);
        animPlayer.SetBool("EnemyGrapling", true);
        animPlayer.SetFloat("EnemyGraplingCount", 2.0f);
        enemyPosition = targetPosition;

        gameObject.layer = 8;

        lerpTime = graplingTime;
        if (enemyObj != null)
        {
            isLerping = true;

            float elapsedTime = 0f;
            Vector3 startingPos = transform.position;
            Vector3 targetPos = targetPosition.GetChild(1).position;

            while (elapsedTime < lerpTime)
            {
                isMakeEnemyGrapligGhost = true; //��Ʈ ����Ʈ true

                //2. �÷��̾� ȸ��
                //  transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, angle - 99.0f), rotationSpeed * Time.deltaTime); 
                //targetPosition = enemyPosition.position;
                line.SetPosition(0, new Vector3(enemyHookPos.position.x, enemyHookPos.position.y + 0.2f, enemyHookPos.position.z));
                hook.position = enemyObj.transform.position;
                transform.position = Vector3.Lerp(startingPos, targetPos, Mathf.SmoothStep(0f, 1f, elapsedTime / lerpTime));
                //Mathf.SmoothStep(float edge0, float edge1, float t); -> Mathf.SmoothStep(���� ��, �� �� , ���� ���� �� ���� ���� �μ�)
                // t�� 0���� �϶� ���� ���̸�, t�� 1 �̻� �϶� ��� ��.
                //�� �Լ��� Ư�� ���۰� �� �κ��� �ε巴�� ����� �߰� �κ��� �� ������ ���ӵǰų� ���ӵǴ� ���� ȿ���� �����ϴ� �� ���
                //Vector3.Lerp�Լ��� ���� ���� �ε巯����. 
                elapsedTime += Time.deltaTime;

                if (Vector3.Distance(transform.position, targetPos) < 0.01f)
                {
                    gameObject.layer = 7;
                    aim.isAimEnemy = false;
                    hook.gameObject.SetActive(false);

                    animPlayer.SetBool("EnemyGrapling", false);

                    GraplingPlayerFlip(enemyObj);
                    animPlayer.SetTrigger("PlayerAttack");
                    
                    yield return new WaitForSeconds(0.1f);
                    Enemies enemyScript = enemyObj.GetComponentInParent<Enemies>(); //������ ������ �ִ� �Լ� ȣ�� �ڵ�@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                    if (enemyScript != null)
                    {
                        StartCoroutine(enemyScript.baseDamaged());
                    }
                    else
                    {
                        Debug.Log("Enemies�������̽� ã�� ����");
                    }

                    break;

                }


                yield return null;
            }

            transform.rotation = originalRotation;

            grapCount = 0.0f;
            animPlayer.SetFloat("EnemyGraplingCount", grapCount);

            isMakeEnemyGrapligGhost = false; //Ghost ����Ʈ ����.

            isLerping = false; // Lerp ����
            isenemyGrapling = false; //�� �׷��ø� ����
            isGraplingEnemy = false;
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

    public bool isWall;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wall" && isFlyReady)
        {
            Debug.Log("�� �浹");
            isWall = true;
        }
    }




}
