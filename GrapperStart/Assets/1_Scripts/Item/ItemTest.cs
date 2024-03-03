using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTest : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.angularVelocity = Random.Range(30f, 60f); // 아이템이 생성될 때 무작위로 회전 속도 지정
    }

    void Update()
    {
        if (!isGrounded)
        {
            // 아이템이 지면과 부딪히면 1초당 90도씩 회전
            Debug.Log("지면과 부딪히기 전 회전");
            transform.Rotate(Vector3.up * 90 * Time.deltaTime);
        }
        else if(isGrounded)
        {
            Invoke("BasicItemRotate", 1.0f);
        }
        
    }

    void BasicItemRotate()
    {
        Debug.Log("1초당 1번씩 회전");
        transform.Rotate(Vector3.up * 90 * Time.deltaTime);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            rb.velocity = Vector2.zero; // 아이템이 떨어지는 도중에 속도를 제로로 설정
            rb.angularVelocity = 0f; // 아이템이 떨어진 후 회전을 멈춤
        }
    }
}
