using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aiming : MonoBehaviour
{
    public LineRenderer lineAim;
    public Transform hookAim;

    Animator anim;
    Grapling grapling;
    Hooking hooking;
    GraplingRange graplingRange;
    PlayerControllerRope player;

    Vector3 mouse;
    public Transform playerAimPos;
    private void Start()
    {
        grapling = FindAnyObjectByType<Grapling>();

        graplingRange = FindAnyObjectByType<GraplingRange>();
        player = FindAnyObjectByType<PlayerControllerRope>();


        lineAim.positionCount = 2;
        lineAim.startWidth = lineAim.endWidth = 0.05f;
        lineAim.useWorldSpace = true;

    }

    //public Material dashedLineMaterial; // 점선 이미지를 사용한 머티리얼
    //public float dashLength = 0.2f; // 점선의 길이
    //public float gapLength = 0.1f; // 공백의 길이

    private void Update()
    {
        AimGrap();

    }


    public bool isAimEnemy = false;
    public bool isCollEnemy = false;
    public bool isAimRing = false;
    public bool isGrapplingReady = false;

    public GameObject EnemyObj;

    private void AimGrap()
    {


        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouse, Vector2.zero);
        //Vector3 aimStarPos = initialAimPos.transform.position;
        Vector3 aimStarPos = playerAimPos.transform.position;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(graplingRange.skillPos.position, graplingRange.radius);
        // 가져온 Collider2D들에 대한 처리
        if (hit.collider != null) //마우스 포인터로 움직이다가 오브젝트의 콜라이더가 있으면
        {
            foreach (Collider2D collider in colliders)
            {

                if ((collider.CompareTag("Ring") || collider.CompareTag("Enemy") || collider.CompareTag("Boss"))
                    && graplingRange.isRange && graplingRange.isobjSkill
                    && grapling.isAttatch == false) //범위 안에 있어야 하며, 범위 안에 obj가 있어야 하며 , Hook가 Ring과 충돌하지 않을때.
                {


                    //lineAim.material.mainTextureScale = new Vector2((dashLength + gapLength) / dashLength, 1f);
                    //lineAim.material.mainTextureOffset -= new Vector2(Time.deltaTime / (dashLength + gapLength), 0f);
                    lineAim.SetPosition(0, aimStarPos); //조준 시작 위치는 aimStarPos이다.
                    Vector3 mousedir = hookAim.transform.position - aimStarPos; //조준선 방향 구하기
                    float aimDistance = Vector2.Distance(aimStarPos, hookAim.transform.position); //조준선 사이 길이 구하기 

                    aimMousedir = mousedir;
                    aimLength = aimDistance;

                    if (hit.collider == collider) //마우스 포인터와 Ring or Enemy 오브젝트의 충돌이 같을 때
                    {
                        player.playerData.curSpeed = 3.0f;


                        if (collider.CompareTag("Ring") && isCollEnemy == false && grapling.isFlyReady == false && grapling.isHookActive == false)
                        {
                            //Debug.Log("b");
                            targetRing = collider.transform;
                            isAimRing = true;
                            //Debug.Log("b");

                            grapling.iscollObj = true;

                            hookAim.gameObject.SetActive(true);

                            Vector3 aimEndPos = collider.transform.position;
                            hookAim.position = aimEndPos;
                            lineAim.SetPosition(1, aimEndPos); //조준 마지막 위치는 aimEndPos(충돌한 오브젝트)이다.

                            ScenesManager.Instance.FindEnemiesInScene();
                            MouseManager.Instance.SetCursorType(MouseManager.CursorType.objIdle);
                        }



                        if (collider.CompareTag("Enemy") || (collider.CompareTag("Boss") && grapling.isGraplingEnemy == false && isAimRing == false))
                        {
                            //Debug.Log("a");
                            isAimEnemy = true;
                            isCollEnemy = true;

                            Vector3 rotEndPos = collider.transform.position;

                            lineAim.SetPosition(1, rotEndPos);

                            EnemyObj = collider.gameObject;


                            MouseManager.Instance.SetCursorType(MouseManager.CursorType.enemyIdle);

                            hookAim.position = collider.transform.position;

                            if (grapling.grapCount == 0)
                            {
                                //grapling.isGrapplingActive = true;
                                hookAim.gameObject.SetActive(true);
                            }
                            else
                            {
                                hookAim.gameObject.SetActive(false);
                            }

                        }




                    }
                }

            }
        }
        if (hit.collider == null || grapling.isAttatch) //링에 걸려있거나 마우스 포인터와 충돌하지 않았다면 
        {
            isAimRing = false;
            isCollEnemy = false;
            isCollPlayer = false;
            player.playerData.curSpeed = player.InitSpeed; //플레이어가 링을 조준하지 않았을 때 원래 스피드로 돌아감.
            isGrapplingReady = false;
            anim = GetComponent<Animator>();
            anim.SetBool("PlayerAimEnemy", false);
            grapling.iscollObj = false;

            hookAim.gameObject.SetActive(false);
            ScenesManager.Instance.FindEnemiesInScene();
            MouseManager.Instance.SetCursorType(MouseManager.CursorType.Idle);


        }

        if ((graplingRange.isobjSkill == false && graplingRange.isRange == false) ||
            (graplingRange.isenemySkill == false && graplingRange.isRange == false))
        {
            isGrapplingReady = false;
            hookAim.gameObject.SetActive(false);
            Grapling grapling = FindAnyObjectByType<Grapling>();
            grapling.ResetGrap();
        }

        else if (grapling.iscollObj == true)
        {
            //anim = GetComponent<Animator>();
            //anim.SetBool("PlayerAimEnemy", true);
        }
        else if (grapling.isGraplingEnemy == true)
        {
            hookAim.gameObject.SetActive(false);
        }

    }

    public bool isCollPlayer;
    public Vector3 GetAimDirection()
    {
        // 마우스 포인터 위치를 월드 좌표로 변환
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // 플레이어 위치와 마우스 위치 사이의 방향 벡터를 반환
        return (mousePosition - transform.position).normalized;
    }

    public Vector2 aimMousedir;
    public float aimLength;
    public Transform targetRing;



}
