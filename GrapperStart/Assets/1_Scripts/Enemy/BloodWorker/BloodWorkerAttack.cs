using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodWorkerAttack : BloodWorkerAttackReady
{
    public override IEnumerator InstanRock(BloodState state, GameObject rockPref, Transform rockPos)
    {
        Vector3 rockVec = rockPos.position;
        yield return new WaitForSeconds(0.1f);
        Instantiate(rockPref, rockVec, Quaternion.identity);

      

        // 이후에 원하는 작업을 추가할 수 있습니다.
    }

    public override void RenchAttack(BloodState state, Collider2D[] collider,Animator renchAnim)
    {
       foreach(Collider2D renchCollider in collider)
        {
            if(renchCollider.CompareTag("Player"))
            {
                Flip(renchCollider);
                Debug.Log("타격");
                renchAnim.SetTrigger("RenchAttack");
            }
        }
    }

    void Flip(Collider2D player)
    {

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
