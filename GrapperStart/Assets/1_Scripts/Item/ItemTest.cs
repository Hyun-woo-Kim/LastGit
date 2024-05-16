using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTest : MonoBehaviour
{
    public enum Type
    {
        Item,

    }

    public Type type;
    public int value;


    private Rigidbody2D rb;
    private bool isGrounded = false;

    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.angularVelocity = Random.Range(30f, 60f); // �������� ������ �� �������� ȸ�� �ӵ� ����
    }

    void Update()
    {
        //if (!isGrounded)
        //{
        //    // �������� ����� �ε����� 1�ʴ� 90���� ȸ��
        //    Debug.Log("����� �ε����� �� ȸ��");
        //    transform.Rotate(Vector3.up * 90 * Time.deltaTime);
        //}
        //else if(isGrounded)
        //{
        //    Invoke("BasicItemRotate", 1.0f);
        //}
        
    }

    //void BasicItemRotate()
    //{
    //    Debug.Log("1�ʴ� 1���� ȸ��");
    //    transform.Rotate(Vector3.up * 90 * Time.deltaTime);
    //}
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") && !hasBounced)
        {
            //isGrounded = true;
            //rb.velocity = Vector2.zero; // �������� �������� ���߿� �ӵ��� ���η� ����
            //rb.angularVelocity = 0f; // �������� ������ �� ȸ���� ����

            BounceItem();
             hasBounced = true;
        }
    }
    private bool hasBounced = false;
    public float bounceForce;

    void BounceItem()
    {
        rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
    }
}
