using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodWorkerAttack : BloodWorkerAttackReady
{
    public override void InstanRock(BloodState state, GameObject rockPref, Transform rockPos)
    {    
         Vector3 rockVec = rockPos.position;
         Instantiate(rockPref, rockVec, Quaternion.identity);

    }


  


    public override void RenchAttack(BloodState state, Collider2D[] collider,Animator renchAnim)
    {
       foreach(Collider2D renchCollider in collider)
        {
            if(renchCollider.CompareTag("Player"))
            {
                Flip(renchCollider);
                renchAnim.SetTrigger("RenchAttack");
                Debug.Log("�÷��̾� �߰�");
            }
        }
    }

    void Flip(Collider2D player)
    {
        Debug.Log("������ȯ");
        Transform playerPos = player.transform;
        if (transform.position.x > playerPos.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (transform.position.x < playerPos.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    
}
