using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodWorkerRock : MonoBehaviour
{
    public float throwForce = 10f; // 던질 힘
    public float gravityScaleDecreaseRate = 0.1f; // 중력 감소 비율

    private Rigidbody2D rockRigid;

    void Start()
    {
        rockRigid = GetComponent<Rigidbody2D>();
        rockRigid.gravityScale = 1.0f; // 초기 중력 값 설정

        Invoke("DestroyRock", 3.0f);
    }

    private void Update()
    {
        // 중력 값을 시간에 따라 감소시킴
        rockRigid.gravityScale = Mathf.Max(0, rockRigid.gravityScale - gravityScaleDecreaseRate * Time.deltaTime);

        transform.Translate(transform.right * -1 * throwForce * Time.deltaTime);
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