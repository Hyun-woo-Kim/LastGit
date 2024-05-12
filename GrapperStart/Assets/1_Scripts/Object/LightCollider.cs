using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCollider : MonoBehaviour
{
     EdgeCollider2D childboxCollider;
    void Start()
    {
        childboxCollider = GetComponentInChildren<EdgeCollider2D>();
         
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Hook")
        {
            
             childboxCollider.isTrigger = false;     // childboxCollider를 사용한 코드
            
           
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Hook")
        {

            childboxCollider.isTrigger = true;     // childboxCollider를 사용한 코드


        }
    }

    //처음엔 통과 가능하게 콜라이더 설정 -> isTrigger = true
    //후크와 태그 충돌 시 통과 불가능하게 설정. -> isTrigger = false 
    //그래플링 후 플레이어가 땅에 닿으면 isTrigger = true



}
