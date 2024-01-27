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

    Vector3 mouse;
    public Transform playerAimPos;
    private Transform initialAimPos;
    private void Start()
    {
         grapling = FindAnyObjectByType<Grapling>();
       
        graplingRange = FindAnyObjectByType<GraplingRange>();


        lineAim.positionCount = 2;
        lineAim.startWidth = lineAim.endWidth = 0.05f;
        lineAim.useWorldSpace = true;

        //initialAimPos = playerAimPos;
        Debug.Log(playerAimPos.position);

        
    }

    //public Material dashedLineMaterial; // 점선 이미지를 사용한 머티리얼
    //public float dashLength = 0.2f; // 점선의 길이
    //public float gapLength = 0.1f; // 공백의 길이

    private void Update()
    {
        AimGrap();

       

    }

    public bool isAimEnemy = false;
    public bool isAimObject = false;

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

                if ((collider.CompareTag("Ring") || collider.CompareTag("Enemy"))
                    && graplingRange.isRange && graplingRange.isobjSkill
                    && grapling.isAttatch == false) //범위 안에 있어야 하며, 범위 안에 obj가 있어야 하며 , Hook가 Ring과 충돌하지 않을때.
                {
                    //lineAim.material.mainTextureScale = new Vector2((dashLength + gapLength) / dashLength, 1f);
                    //lineAim.material.mainTextureOffset -= new Vector2(Time.deltaTime / (dashLength + gapLength), 0f);
                    lineAim.SetPosition(0, aimStarPos); //조준 시작 위치는 aimStarPos이다.

                    if (hit.collider == collider) //마우스 포인터와 Ring or Enemy 오브젝트의 충돌이 같을 때
                    {

                        hookAim.position = this.transform.position;

                        //hookAim.gameObject.SetActive(true);
                   
                            anim = GetComponent<Animator>();
                            anim.SetBool("PlayerAimEnemy",true);
                        
                     


                        if (collider.CompareTag("Ring") && isAimEnemy == false)
                        {   
                                
                                hookAim.gameObject.SetActive(true);

                                grapling.iscollObj = true;

                                Vector3 aimEndPos = collider.transform.position;
                                lineAim.SetPosition(1, aimEndPos); //조준 마지막 위치는 aimEndPos(충돌한 오브젝트)이다.
                                hookAim.position = aimEndPos;
                                float aimDistance = Vector2.Distance(aimStarPos, aimEndPos); //조준선 사이 길이 구하기 
                                Vector3 mousedir = aimEndPos - aimStarPos; //조준선 방향 구하기

                                AimData(aimDistance, mousedir); //조준선 방향, 길이 넘기기  

                            MouseManager.Instance.SetCursorType(MouseManager.CursorType.objIdle);
                        }
                        
                       

                        if (collider.CompareTag("Enemy"))
                        {
                            

                            Vector3 rotEndPos = collider.transform.position;

                            lineAim.SetPosition(1, rotEndPos);

                            GameObject colliderGo = collider.gameObject;


                            MouseManager.Instance.SetCursorType(MouseManager.CursorType.enemyIdle);

                            hookAim.position = this.transform.position;

                                if (grapling.grapCount == 0)
                                {
                                    grapling.isGrapplingActive = true;
                                    hookAim.gameObject.SetActive(true);
                                }
                                else
                                {
                                    hookAim.gameObject.SetActive(false);
                                }

                                if (Input.GetKeyDown(KeyCode.E))
                                {

                                    isAimEnemy = true;
                                    grapling.isGrapplingActive = false;
                                    grapling.GrapHandling(colliderGo);
                                }
                            
                                                   
                        }
                        
                      
                    }
                }

            }
        }
        if (hit.collider == null || grapling.isAttatch) //링에 걸려있거나 마우스 포인터와 충돌하지 않았다면 
        {
            anim = GetComponent<Animator>();
            anim.SetBool("PlayerAimEnemy", false);       
            grapling.iscollObj = false;

            hookAim.gameObject.SetActive(false);
            MouseManager.Instance.SetCursorType(MouseManager.CursorType.Idle);


        }

        if ((graplingRange.isobjSkill == false && graplingRange.isRange == false )||
            (graplingRange.isenemySkill == false && graplingRange.isRange == false))
        {
            hookAim.gameObject.SetActive(false);
            Grapling grapling = FindAnyObjectByType<Grapling>();
            grapling.ResetGrap();
        }

    }


    public Vector2 aimMousedir;
    public float aimLength;
    public void AimData(float length, Vector3 dir) //조준선 길이 및 방향과 갈고리 길이 및 방향을 일치시키기.
    {
        aimLength = length;
        aimMousedir = dir;

    }


}
