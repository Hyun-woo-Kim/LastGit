using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hooking : MonoBehaviour
{
    Grapling grappling;
    Aiming aiming;
    GraplingRange graplingRange;

    public SpriteRenderer hookSpr;

    public Sprite attachedSprite;
    public Sprite defaultSprite;

    public Transform hooklinePos;
    public DistanceJoint2D joint2D;
    void Start()
    {
        grappling = GameObject.Find("Player").GetComponent<Grapling>();
       
        graplingRange = FindAnyObjectByType<GraplingRange>();
        joint2D = GetComponent<DistanceJoint2D>();
        //hookSpr = GetComponent<SpriteRenderer>();

        Debug.Log(grappling.transform.rotation);

    }
    private void Awake()
    {
        aiming = FindAnyObjectByType<Aiming>();

    }

    private void Update()
    {
        if (grappling.isHookActive)
        {
            Debug.Log("HOOK이 돌아간다");
            transform.rotation = grappling.transform.rotation;
           
        }


        if (grappling.isenemyGrapling) //적에게 갈고리 날렸을 때 , 갈고리는 적을 추격한다.
        {
            if (target != null && grappling.grapCount == 1)
            {
                transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * hookToEnemyspeed);
          
            }
        }

    }

    public Transform target;
    public float hookToEnemyspeed;

    public void rotationHook(Vector3 hookdir)
    {
        Vector3 playerdir = hookdir - transform.position;

        float angle = Mathf.Atan2(playerdir.y, playerdir.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle + 90f, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15.0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ring"))
        {
            joint2D.enabled = true;
            grappling.isAttatch = true;
            GetComponent<SpriteRenderer>().sprite = attachedSprite;
            //GrapplingObjManager.Instance.brightnessUp(collision);
        }
       

    }

   
    public float dealy;

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ring"))
        {
            GetComponent<SpriteRenderer>().sprite = defaultSprite;
            joint2D.enabled = false;
            graplingRange.isobjSkill = false;
            grappling.isAttatch = false; //추가
            //GrapplingObjManager.Instance.brightnessDown(collision);
        }
     
    }


}
