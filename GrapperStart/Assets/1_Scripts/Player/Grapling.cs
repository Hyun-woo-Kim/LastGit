using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grapling : MonoBehaviour
{
    [Header("#Hook Object")]
    public LineRenderer line;
    public Transform hook; //갈고리의 위치를 나타내는 Transform 변수입니다.


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
    public float rotationSpeed = 5.0f; // 조절 가능한 회전 속도
    private PlayerArm playerArmScr;
    public bool isFlyReady = false; //공중제비 준비 여부
    public bool isEKeyHeld = false; //E키 눌림여부
    private float eKeyHoldTime = 0f; //E키 누르는 시간
    public float PlayerGraplingAnimCount; //그래플링 관련 블렌드 트리 애니메이션 변수 
    public float baseSwingForce; //공중제비 세기 값 변수 
    private Quaternion originalRotation; // 원래의 회전값을 저장할 변수
    public bool isLineMax;
    public bool hookisLeft;
    public bool hookisRight;
    [Header("##Aim & Grapling")]
    public bool iscollObj = false;
    public bool iscollenemy = false;

    [Header("##Enemy Grapling")]
    public float grapCount = 0.0f;
    public bool isGrap = false;
    public bool isLerping = false; // Lerp 중인지 여부를 나타내는 변수
    public float lerpTime; // 이 값이 클 수록 그래플링 하는 시간이 지속됨. 그래플링 시간.
    public Transform enemyPosition;
    public float grapanimdelay;
    public Transform enemyHookPos;
    public bool isenemyGrapling;
    public float graplingDamage;


    private Camera cam;


    //라인을 그리는 포지션을 두개로 설정.
    //한 점은 Player의 포지션: positionCount
    //한 점은 Hook의 포지션: SetPosition

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

        #region Line컴포넌트 정리
        //정리
        //시작하면 점의 갯수는 2개로 설정되어 처음 라인 위치의 점과 마지막 라인의 위치의 점 . 총 2개로 설정된다.
        //인덱스 0과 1의 너비는 0.05로 모두 맞춘다.
        //Line오브젝트에 포함된 LineRenderer의 인덱스 0의 위치(갈고리 라인의 첫 위치)는 플레이어의 위치로 설정되고, 
        // 인덱스 1의 위치(갈고리 라인의 마지막 위치는) Hook오브젝트의 위치로 설정된다. 그리고 이 라인의 공간은 월드 공간으로 설정된다.
        //line.SetPosition(0, this.transform.position); 
        //인덱스 0의 위치를 이 스크립트를 들고있는 오브젝트의 현 위치로 지정한다.
        //line.SetPosition(1, this.hook.position); 
        //인덱스 1의 위치를 이 스크립트를 들고있는 hook의 위치로 지정한다. 
        //SetPosition(index, position): 특정 인덱스의 점의 위치를 설정합니다. 라인을 그리는 데 각 점의 위치를 지정할 수 있습니다.
        //positionCount: 라인의 점의 개수를 설정합니다. 이 속성을 사용하여 라인이 어떻게 연결되는지 결정할 수 있습니다.
        //useWorldSpace: 라인의 점들이 월드 공간(세계 좌표)에서 위치하도록 할지, 로컬 공간(오브젝트의 로컬 좌표)에서 위치하도록 할지를 결정합니다.
        #endregion 
    }


    void HookSet()
    {
        line.positionCount = 2;
        line.endWidth = line.startWidth = 0.025f;  //startWidth: 라인의 시작 부분의 너비를 설정합니다.//endWidth: 라인의 끝 부분의 너비를 설정합니다.이로써 라인이 끝 부분에서 얼마나 넓어지거나 좁아지는지 조절할 수 있습니다.

        line.useWorldSpace = true; //월드 공간에서 위치하도록 함. 

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

        if (graplingRange.isRange == true) //범위 켜진 상태 and 범위 안에 링 있을 때 and 범위 안에 적이 없을 때
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
            Debug.Log("호출");
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
            playerArmScr.rotationArm(hook.transform.position); //팔이 회전하는 메서드 호출
        }

        else
        {
            Debug.Log("팔을 찾지 못함");
        }


    }

    void RotPlayer()
    {
        Vector3 playerdir = hook.transform.position - transform.position;
        float hookangle = Mathf.Atan2(playerdir.y, playerdir.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(hookangle - 90f, Vector3.forward);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);//플레이어 회전하는 메서드 호출

    }


    public void GraplingSkill() //링 로브젝트 기준 그래플링 스킬 
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
            hook.position = aim.playerAimPos.position; //hook의 시작 위치는 playerAimPos.

            isLineMax = false;

            hookRightLeft();
        }
        if (isHookActive == true && isLineMax == false && isAttatch == false)
        {

            //Hook오브젝트가 날아갈 때구문.
            hook.Translate(aim.aimMousedir.normalized * Time.deltaTime * hookMoveSpeed * 1.5f);

        }
        else if (isHookActive == true && isLineMax == true)
        {
            //Hook오브젝트가 돌아 올 때 구문.       

            // hook.position = Vector2.MoveTowards(hook.position, playerArm.transform.position, Time.deltaTime * hookDelSpeed); //hook에서 transform위치로 hookDelSpeed속도로 이동한다.(hook가 돌아온다)
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
                    // E 키가 놓였을 때 실행할 코드
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

    //전등 그래플링 후 메서드
    void LightGraplingStart()
    {
        transform.rotation = originalRotation;
        isMakeLightGrapligGhost = true;
        if (hookisLeft)
        {
            PlayerGraplingAnimCount = 1.0f;
            animPlayer.SetFloat("PlayerGraplingCount", PlayerGraplingAnimCount);
        }
        else if (hookisRight)//후크는 오른쪽
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
            Debug.Log("땅과 충돌 직전");
            player.isFlyAction = false;
            isFlyReady = false;
            playerColl.isTrigger = false;
            isWall = false;
            isMakeLightGrapligGhost = false;
            animPlayer.SetBool("PlayerGrapling", false);

        }
    }



    public float maxSwingAngle = 180f; // 최대 스윙 각도
    public float swingForce = 5f; // 스윙 힘
    public float hookDistance;
    void hookRigid()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        // 수평 입력을 받았을 때 180도 각도로 회전하도록 제한
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
            else if ((rotationhook.z > 90.0f && rotationhook.z < 180.0f) || (rotationhook.z < 360.0f && rotationhook.z > 270.0f))//후크는 오른쪽
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
            else if ((rotationhook.z > 90.0f && rotationhook.z < 180.0f) || (rotationhook.z < 360.0f && rotationhook.z > 270.0f))//후크는 오른쪽
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


            //hook 처음 위치는 적 갈고리 발사 애니메이션 팔 위치에 초기화.

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
            // comboBarUI를 UI_Canvas의 하위로 설정
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
        //슬라이더의 밸류가 초록색 바 안에 있는 지 알려면 이미지의 앵커 포지션과 델타 사이즈의 x값을 알아야 함.
        minPos = pass.anchoredPosition.x; //pass.anchoredPosition.x는 최소값을 나타내야함.
        maxPos = pass.sizeDelta.x + minPos;

        StartCoroutine(comboEnemyGrapling());
    }

    public float direction = 1.0f;
    IEnumerator comboEnemyGrapling()
    {
        yield return null;

        // 슬라이더 이동 방향

        while (!(Input.GetKeyDown(KeyCode.E) || comboSliderPrefab.value == comboSliderPrefab.maxValue))
        {

            comboSliderPrefab.value += Time.deltaTime * sliderSpeed * direction;

            // 슬라이더 값이 max값을 초과하면 이동 방향 반전
            if (comboSliderPrefab.value >= comboSliderPrefab.maxValue)
            {
                direction = -1f; // 이동 방향 반전
                comboSliderPrefab.value += Time.deltaTime * sliderSpeed * direction;
            }

            // 슬라이더 값이 min값 미만이면 이동 방향 반전
            if (comboSliderPrefab.value <= comboSliderPrefab.minValue)
            {
                direction = 1f;
                comboSliderPrefab.value += Time.deltaTime * sliderSpeed * direction;
            }

            yield return null;
        }


        if (comboSliderPrefab.value >= minPos && comboSliderPrefab.value <= maxPos)  // 슬라이더가 초록색 바에 위치 하지 않았을 떄
        {
            lerpTime = 0.8f;
            graplingDamage = 1.0f;
            GrapHandling(aim.EnemyObj, lerpTime, graplingDamage);
        }
        else if (comboSliderPrefab.value <= minPos || comboSliderPrefab.value >= maxPos) //슬라이더가 초록색 바에 위치했을 때
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
         comboSliderPrefab = null; // 변수 null로 설정


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
                isMakeEnemyGrapligGhost = true; //고스트 이펙트 true

                //2. 플레이어 회전
                //  transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, angle - 99.0f), rotationSpeed * Time.deltaTime); 
                //targetPosition = enemyPosition.position;
                line.SetPosition(0, new Vector3(enemyHookPos.position.x, enemyHookPos.position.y + 0.2f, enemyHookPos.position.z));
                hook.position = enemyObj.transform.position;
                transform.position = Vector3.Lerp(startingPos, targetPos, Mathf.SmoothStep(0f, 1f, elapsedTime / lerpTime));
                //Mathf.SmoothStep(float edge0, float edge1, float t); -> Mathf.SmoothStep(시작 값, 끝 값 , 시작 값과 끝 값의 보간 인수)
                // t가 0이하 일때 시작 값이며, t가 1 이상 일때 결과 값.
                //이 함수는 특히 시작과 끝 부분을 부드럽게 만들어 중간 부분이 더 빠르게 가속되거나 감속되는 등의 효과를 생성하는 데 사용
                //Vector3.Lerp함수와 같이 사용시 부드러워짐. 
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
                    Enemies enemyScript = enemyObj.GetComponentInParent<Enemies>(); //적에게 데미지 주는 함수 호출 코드@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                    if (enemyScript != null)
                    {
                        StartCoroutine(enemyScript.baseDamaged());
                    }
                    else
                    {
                        Debug.Log("Enemies인터페이스 찾지 못함");
                    }

                    break;

                }


                yield return null;
            }

            transform.rotation = originalRotation;

            grapCount = 0.0f;
            animPlayer.SetFloat("EnemyGraplingCount", grapCount);

            isMakeEnemyGrapligGhost = false; //Ghost 이펙트 종료.

            isLerping = false; // Lerp 종료
            isenemyGrapling = false; //적 그래플링 종료
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
            Debug.Log("벽 충돌");
            isWall = true;
        }
    }




}
