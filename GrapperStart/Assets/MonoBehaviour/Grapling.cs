using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapling : MonoBehaviour
{
    public LineRenderer line;
    public Transform  hook; //갈고리의 위치를 나타내는 Transform 변수입니다.

  
    GraplingRange graplingRange;
    Animator animPlayer;
   


    Aiming aim;

    [Header("#PlayerSkill")]
    //Vector2 mousedir;
    public bool isHookActive = false;
    public bool isLineMax = false;
    public bool isAttatch = false;
    public float hookDelSpeed;
    public float hookMoveSpeed;
   // public float hookLength;
   
    public bool iscollObj = false;
    public bool iscollenemy = false;
    public float grapCount = 0.0f;
    public bool isGrap = false;
    public bool isLerping = false; // Lerp 중인지 여부를 나타내는 변수
    public float lerpTime;
    public Transform enemyPosition;


    public Transform playerArm;

    private Quaternion originalRotation; // 원래의 회전값을 저장할 변수

    //라인을 그리는 포지션을 두개로 설정.
    //한 점은 Player의 포지션: positionCount
    //한 점은 Hook의 포지션: SetPosition

    PlayerControllerRope player;

    void Start()
    {
        animPlayer = GetComponent<Animator>();
        

        graplingRange = FindAnyObjectByType<GraplingRange>();
        aim = FindAnyObjectByType<Aiming>();

        player = GameObject.Find("Player").GetComponent<PlayerControllerRope>();
        HookSet();      
        
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

    void Update()
    {
        GraplingSkill();

        if(graplingRange.isRange == true &&  isAttatch == false && graplingRange.isenemySkill == true) //범위안에 있어야 하며, 범위 안에 적이 있어야 하며, 링에 부착하지 않았을 때만
        {
           //isRange == true , isenemySkill == true
            GraplingEnmeyHookPos();
        }




        if (graplingRange.isRange == true ) //범위 켜진 상태 and 범위 안에 링 있을 때 and 범위 안에 적이 없을 때
        {
            if(grapCount == 1.0f && graplingRange.isenemySkill == false)
            {
                grapCount = 0.0f;
                animPlayer.SetBool("EnemyGrapling", false);
                // hokkobj.GetComponent<SpriteRenderer>().color;
                StartCoroutine(HookColor());             
                aim.isAimEnemy = false;
            }
           

        }
        else if(graplingRange.isRange == false && graplingRange.isenemySkill == false)
        {
           // grapCount = 0.0f;
            hook.gameObject.SetActive(false);
            aim.isAimEnemy = false;
            animPlayer.SetBool("PlayerGrapling", false);

        }



       

        if (player.isPlayerfly == true)
        {
            float distance = Vector2.Distance(playerArm.transform.position, hook.position);
            float Mindistance;

           
            Mindistance = Mathf.Min(0.5f, distance); //distance와 0.5를 비교하여 작은값을 출력한다. 

            


            if (Mindistance < 0.5f || distance < 0.9f) // distance가 0.5보다 작을 때 추가적인 작업을 수행하고 싶다면 이 부분을 수정하세요.
            {
                playerArm.gameObject.SetActive(false);
                hook.gameObject.SetActive(false);
                Debug.Log(distance);
            }


            //else if (distance < 1.0f)
            //{
            //    playerArm.gameObject.SetActive(false);
            //     hook.gameObject.SetActive(false);
            //}



        }


        if (isHookActive)
        {
            RotData();
        }

    }

    public void GraplingEnmeyHookPos()
    {
        if(isLerping == false)
        {
            line.SetPosition(0, enemyHookPos.position);
        }
        else if(isLerping == true )
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

    public float rotationSpeed = 5.0f; // 조절 가능한 회전 속도
    public float hookrotationSpeed = 5.0f; // 조절 가능한 회전 속도

    private PlayerArm playerArmScr;

    public void RotData()
    {

        playerArmScr = FindObjectOfType<PlayerArm>();

        //PlayerArm arm = GameObject.FindObjectsOfType("Player_Arm").GetComponentInChildren<PlayerArm>();
        if (playerArmScr != null)
        {
            playerArmScr.rotationArm(hook.transform.position);
            

            Vector3 playerdir = hook.transform.position - transform.position;

            float hookangle = Mathf.Atan2(playerdir.y, playerdir.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.AngleAxis(hookangle - 90f, Vector3.forward);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
       
        else
        {
            Debug.Log("팔을 찾지 못함");
        }    
        

    }
        


    public float hookDistance;

    public bool hookisLeft;
    public bool hookisRight;




    public void GraplingSkill() //링 로브젝트 기준 그래플링 스킬 
    {

        // line.SetPosition(0, playerArm.position);
        
        line.SetPosition(1, hook.GetComponent<Hooking>().hooklinePos.position);

        if (Input.GetKeyDown(KeyCode.E) && 
            isHookActive == false && iscollObj == true)
        { 
            animPlayer.SetTrigger("Player_Grapling_Ready");

            //hook의 라인 시작 위치는 playerAimPos
            line.SetPosition(0, aim.playerAimPos.position);
            iscollObj = false;

            hook.position = aim.playerAimPos.position; //hook의 시작 위치는 playerAimPos.

            isHookActive = true;
            isLineMax = false;
            hook.gameObject.SetActive(true);

  
            if (transform.position.x > hook.transform.position.x)
            {
                hookisLeft = true;
                hookisRight = false;
                hook.GetComponent<Hooking>().hookSpr.flipX = false;
            }
            else if(transform.position.x < hook.transform.position.x)
            {
                hookisRight = true;
                hookisLeft = false;
                hook.GetComponent<Hooking>().hookSpr.flipX = true;
            }
           

        }
        if (isHookActive == true && isLineMax == false && isAttatch == false)
        {
            
            float distanceFromHookPos = Vector2.Distance(aim.playerAimPos.position, transform.position); //팔 위치

            //Hook오브젝트가 날아갈 때구문.

            hook.Translate(aim.aimMousedir.normalized * Time.deltaTime * hookMoveSpeed * 1.5f);

        }
        else if (isHookActive == true && isLineMax == true )
        {
            //Hook오브젝트가 돌아 올 때 구문.       

           // hook.position = Vector2.MoveTowards(hook.position, playerArm.transform.position, Time.deltaTime * hookDelSpeed); //hook에서 transform위치로 hookDelSpeed속도로 이동한다.(hook가 돌아온다)
            if (Vector2.Distance(playerArm.transform.position, hook.position) < 0.1f)
            {
                isHookActive = false;
                isLineMax = false;
                hook.gameObject.SetActive(false);

            }

            animPlayer.SetBool("PlayerGrapling", false);

        }
        else if (isAttatch == true)
        {
            player.SwingPlayer();

            animPlayer.SetBool("PlayerGrapling", true);
            line.SetPosition(0, playerArm .position);


            playerArm.gameObject.SetActive(true);
           
            if (Input.GetKeyDown(KeyCode.E))
            {
                player.hookAnchorSet();
                player.isPlayerfly = true;
                StopObjGrapling();


            }
           
        }
       


    }

    //1. 오브젝트 걸린 상태. 2. e키를 누름 . 3.공중제비 시작. 4. playerArm과 hook이 가까워지면 두 오브젝트 삭제.


    public void StopObjGrapling()
    {
        //hook.position = Vector2.MoveTowards(hook.position, playerArm.transform.position, Time.deltaTime * hookDelSpeed);
        transform.rotation = originalRotation;

        isAttatch = false;
        isHookActive = false;
        isLineMax = false;
       
            
        animPlayer.SetBool("PlayerGrapling", false);
        animPlayer.SetTrigger("PlayerFly");
        //코드
       
        
    
    }



    public float playerspeed;
    private float grapanimdelay = 0.3f;
    public bool isGrapplingActive = false;
    public Transform enemyHookPos;


    public bool isenemyGrapling;
    public void GrapHandling(GameObject target)
    {
        isenemyGrapling = true;
        GraplingPlayerFlip(target);
         
        hook.position = enemyHookPos.position;

        grapCount += 1.0f;

        animPlayer.SetBool("EnemyGrapling", true);
        animPlayer.SetFloat("EnemyGraplingCount", grapCount);
        if (grapCount == 1.0f && graplingRange.isenemySkill == true)
        {
            aim.isAimObject  = false;

            hook.gameObject.SetActive(true);

        }
        if (grapCount == 2.0f)
        {

            aim.isAimEnemy = false;

            hook.position = target.transform.position;
            
            StartCoroutine(LerpToTarget(enemyPosition.position, target));
        }

    }
 

    IEnumerator LerpToTarget(Vector3 targetPosition,GameObject enemyObj)
    {

       
        //animPlayer.SetFloat("EnemyGraplingCount", grapCount);
        aim.isAimEnemy = false;
        gameObject.layer = 8;

        if (enemyObj != null)
        {
           
            
            isLerping = true;
            float elapsedTime = 0f;
            Vector3 startingPos = transform.position;
             targetPosition = new Vector3(enemyPosition.position.x, enemyPosition.position.y, enemyPosition.position.z);

            while (elapsedTime < lerpTime)
            {
                //2. 플레이어 회전
                //  transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0f, 0f, angle - 99.0f), rotationSpeed * Time.deltaTime); 
                //targetPosition = enemyPosition.position;

                hook.position = enemyObj.transform.position;
                transform.position = Vector3.Lerp(startingPos, targetPosition, Mathf.SmoothStep(0f, 1f, elapsedTime / lerpTime));
                //Mathf.SmoothStep(float edge0, float edge1, float t); -> Mathf.SmoothStep(시작 값, 끝 값 , 시작 값과 끝 값의 보간 인수)
                // t가 0이하 일때 시작 값이며, t가 1 이상 일때 결과 값.
                //이 함수는 특히 시작과 끝 부분을 부드럽게 만들어 중간 부분이 더 빠르게 가속되거나 감속되는 등의 효과를 생성하는 데 사용
                //Vector3.Lerp함수와 같이 사용시 부드러워짐. 

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
                StartCoroutine(enemyObj.GetComponent<CritureController>().AtkDamagedCriture());
            }

            transform.rotation = originalRotation;


            grapCount = 0.0f;
            animPlayer.SetFloat("EnemyGraplingCount", grapCount);
            isLerping = false; // Lerp 종료
            isenemyGrapling = false; //적 그래플링 종료
        }
        
    }


    void GraplingPlayerFlip(GameObject enemy)
    {
        
        if(enemy.transform.localScale.x == 1)
        {
            if(transform.localScale.x == -1)
            {
                if(enemy.transform.position.x > transform.position.x)
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
