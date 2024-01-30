using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hooking : MonoBehaviour
{
    Grapling  grappling;
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

       
    }



        
    private void Update()
    {
        if(grappling.isHookActive)
        {
            transform.rotation = grappling.transform.rotation;

            
        }
       

        if (grappling.isenemyGrapling )
        {
            targetToEnemy();
        }
        
    }

 



    public Transform target;
    public float hookToEnemyspeed;
     void targetToEnemy()
    {
        if (target != null && grappling.grapCount == 1)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * hookToEnemyspeed);
            //transform.Translate((target.position - transform.position).normalized * hookToEnemyspeed * Time.deltaTime);
        }
    }





    public bool isHookRing;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ring"))
        {

            joint2D.enabled = true;
            grappling.isAttatch = true;
            GetComponent<SpriteRenderer>().sprite = attachedSprite;
        }
    
    }




    public float dealy;




    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ring"))
        {
            //PlayerControllerRope player = GameObject.Find("Player").GetComponent<PlayerControllerRope>();
            //player.hookAnchorSet();

            GetComponent<SpriteRenderer>().sprite = defaultSprite;
            //joint2D.enabled = false;
            grappling.isAttatch = false;
            graplingRange.isobjSkill = false;

        }    
    }

 
}
