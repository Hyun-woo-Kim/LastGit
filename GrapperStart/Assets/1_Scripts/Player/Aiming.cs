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

    //public Material dashedLineMaterial; // ���� �̹����� ����� ��Ƽ����
    //public float dashLength = 0.2f; // ������ ����
    //public float gapLength = 0.1f; // ������ ����

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
        // ������ Collider2D�鿡 ���� ó��
        if (hit.collider != null) //���콺 �����ͷ� �����̴ٰ� ������Ʈ�� �ݶ��̴��� ������
        {
            foreach (Collider2D collider in colliders)
            {

                if ((collider.CompareTag("Ring") || collider.CompareTag("Enemy"))
                    && graplingRange.isRange && graplingRange.isobjSkill
                    && grapling.isAttatch == false) //���� �ȿ� �־�� �ϸ�, ���� �ȿ� obj�� �־�� �ϸ� , Hook�� Ring�� �浹���� ������.
                {


                    //lineAim.material.mainTextureScale = new Vector2((dashLength + gapLength) / dashLength, 1f);
                    //lineAim.material.mainTextureOffset -= new Vector2(Time.deltaTime / (dashLength + gapLength), 0f);
                    lineAim.SetPosition(0, aimStarPos); //���� ���� ��ġ�� aimStarPos�̴�.
                    Vector3 mousedir = hookAim.transform.position - aimStarPos; //���ؼ� ���� ���ϱ�
                    float aimDistance = Vector2.Distance(aimStarPos, hookAim.transform.position); //���ؼ� ���� ���� ���ϱ� 

                    aimMousedir = mousedir;
                    aimLength = aimDistance;

                    if (hit.collider == collider) //���콺 �����Ϳ� Ring or Enemy ������Ʈ�� �浹�� ���� ��
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
                            lineAim.SetPosition(1, aimEndPos); //���� ������ ��ġ�� aimEndPos(�浹�� ������Ʈ)�̴�.

                            ScenesManager.Instance.FindEnemiesInScene();
                            MouseManager.Instance.SetCursorType(MouseManager.CursorType.objIdle);
                        }



                        if (collider.CompareTag("Enemy") && grapling.isGraplingEnemy == false && isAimRing == false)
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
        if (hit.collider == null || grapling.isAttatch) //���� �ɷ��ְų� ���콺 �����Ϳ� �浹���� �ʾҴٸ� 
        {
            isAimRing = false;
            isCollEnemy = false;
            isCollPlayer = false;
            player.playerData.curSpeed = player.InitSpeed; //�÷��̾ ���� �������� �ʾ��� �� ���� ���ǵ�� ���ư�.
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
        // ���콺 ������ ��ġ�� ���� ��ǥ�� ��ȯ
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // �÷��̾� ��ġ�� ���콺 ��ġ ������ ���� ���͸� ��ȯ
        return (mousePosition - transform.position).normalized;
    }

    public Vector2 aimMousedir;
    public float aimLength;
    public Transform targetRing;



}
