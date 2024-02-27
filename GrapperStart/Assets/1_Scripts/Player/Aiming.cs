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
    private Transform initialAimPos;
    private void Start()
    {
        grapling = FindAnyObjectByType<Grapling>();

        graplingRange = FindAnyObjectByType<GraplingRange>();
        player = FindAnyObjectByType<PlayerControllerRope>();


        lineAim.positionCount = 2;
        lineAim.startWidth = lineAim.endWidth = 0.05f;
        lineAim.useWorldSpace = true;

        //initialAimPos = playerAimPos;



    }

    //public Material dashedLineMaterial; // ���� �̹����� ����� ��Ƽ����
    //public float dashLength = 0.2f; // ������ ����
    //public float gapLength = 0.1f; // ������ ����

    private void Update()
    {
        AimGrap();

        

    }

 
    public bool isAimEnemy = false;
    public bool isGrapplingReady = false;

    public GameObject EnemyObj;
    private void AimGrap()
    {


        mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouse, Vector2.zero);
        //Vector3 aimStarPos = initialAimPos.transform.position;
        Vector3 aimStarPos = playerAimPos.transform.position;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(graplingRange.skillPos.position, graplingRange.radius);
        // ������ Collider2D�鿡 ���� ó��
        if (hit.collider != null) //���콺 �����ͷ� �����̴ٰ� ������Ʈ�� �ݶ��̴��� ������
        {
            foreach (Collider2D collider in colliders)
            {

                if ((collider.CompareTag("Ring") || collider.CompareTag("Enemy"))
                    && graplingRange.isRange && graplingRange.isobjSkill
                    && grapling.isAttatch == false) //���� �ȿ� �־�� �ϸ�, ���� �ȿ� obj�� �־�� �ϸ� , Hook�� Ring�� �浹���� ������.
                {
                    if(grapling.isLerping == false)
                    {
                        player.playerData.curSpeed = 3.0f;
                    }
                   
                    //lineAim.material.mainTextureScale = new Vector2((dashLength + gapLength) / dashLength, 1f);
                    //lineAim.material.mainTextureOffset -= new Vector2(Time.deltaTime / (dashLength + gapLength), 0f);
                    lineAim.SetPosition(0, aimStarPos); //���� ���� ��ġ�� aimStarPos�̴�.
                    Vector3 mousedir = hookAim.transform.position - aimStarPos; //���ؼ� ���� ���ϱ�
                    float aimDistance = Vector2.Distance(aimStarPos, hookAim.transform.position); //���ؼ� ���� ���� ���ϱ� 

                    aimMousedir = mousedir;
                    aimLength = aimDistance;

                    if (hit.collider == collider) //���콺 �����Ϳ� Ring or Enemy ������Ʈ�� �浹�� ���� ��
                    {
                       

                        if (collider.CompareTag("Ring") && isAimEnemy == false && grapling.isFlyReady == false && grapling.isHookActive == false)
                        {
                            targetRing = collider.transform;
                            Debug.Log("b");

                            grapling.iscollObj = true;

                            hookAim.gameObject.SetActive(true);

                            Vector3 aimEndPos = collider.transform.position;
                            hookAim.position = aimEndPos;
                            lineAim.SetPosition(1, aimEndPos); //���� ������ ��ġ�� aimEndPos(�浹�� ������Ʈ)�̴�.
             
                            MouseManager.Instance.SetCursorType(MouseManager.CursorType.objIdle);
                        }



                        if (collider.CompareTag("Enemy"))
                        {
                            isAimEnemy = true;

                            Vector3 rotEndPos = collider.transform.position;

                            lineAim.SetPosition(1, rotEndPos);

                             EnemyObj = collider.gameObject;


                            MouseManager.Instance.SetCursorType(MouseManager.CursorType.enemyIdle);

                            hookAim.position = this.transform.position;

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
        if (hit.collider == null || grapling.isAttatch) //���� �ɷ��ְų� ���콺 �����Ϳ� �浹���� �ʾҴٸ� 
        {
            player.playerData.curSpeed = player.InitSpeed;
            isGrapplingReady = false;
            anim = GetComponent<Animator>();
            anim.SetBool("PlayerAimEnemy", false);
            grapling.iscollObj = false;

            //isAimEnemy = false; //�����̴� ���� �� �߰� ����. 24_2_25

            hookAim.gameObject.SetActive(false);
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
            anim = GetComponent<Animator>();
            anim.SetBool("PlayerAimEnemy", true);
        }

    }


    public Vector2 aimMousedir;
    public float aimLength;
    public Transform targetRing;



}
