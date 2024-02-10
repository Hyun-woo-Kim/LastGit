using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodWorkerRock : MonoBehaviour
{
    public float throwForce = 10f; // ���� ��
    public float gravityScaleDecreaseRate = 0.1f; // �߷� ���� ����

    private Rigidbody2D rockRigid;

    void Start()
    {
        rockRigid = GetComponent<Rigidbody2D>();
        rockRigid.gravityScale = 1.0f; // �ʱ� �߷� �� ����

        Invoke("DestroyRock", 3.0f);
    }

    private void Update()
    {
        // �߷� ���� �ð��� ���� ���ҽ�Ŵ
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