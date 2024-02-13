using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodWorkerRock : MonoBehaviour
{
    public float throwForce; // 던질 힘
    public float gravityScaleDecreaseRate; // 중력 감소 비율

    private Rigidbody2D rockRigid;

    GameObject BwWorker;
    void Start()
    {
        rockRigid = GetComponent<Rigidbody2D>();
        rockRigid.gravityScale = 1.0f; // 초기 중력 값 설정
        BwWorker = GameObject.Find("BloodWorker");

        Invoke("DestroyRock", 3.0f);
    }
    private Vector2 throwDirection;
    private void Update()
    {
        // 중력 값을 시간에 따라 감소시킴
        rockRigid.gravityScale = Mathf.Max(0, rockRigid.gravityScale - gravityScaleDecreaseRate * Time.deltaTime);

        if(BwWorker.transform.localScale == new Vector3(1, 1, 1))
        {
            throwDirection = new Vector2(-1, 0); 
            transform.Translate(throwDirection * throwForce * Time.deltaTime);
        }
        else if(BwWorker.transform.localScale == new Vector3(-1, 1, 1))
        {
            throwDirection = new Vector2(1, 0); 
            transform.Translate(throwDirection * throwForce * Time.deltaTime);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Ground") || collision.CompareTag("Wall") || collision.CompareTag("Player"))
        {
            DestroyRock();
        }
    }

    private void DestroyRock()
    {
        Destroy(this.gameObject);
    }
}