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
            
             childboxCollider.isTrigger = false;     // childboxCollider�� ����� �ڵ�
            
           
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Hook")
        {

            childboxCollider.isTrigger = true;     // childboxCollider�� ����� �ڵ�


        }
    }

    //ó���� ��� �����ϰ� �ݶ��̴� ���� -> isTrigger = true
    //��ũ�� �±� �浹 �� ��� �Ұ����ϰ� ����. -> isTrigger = false 
    //�׷��ø� �� �÷��̾ ���� ������ isTrigger = true



}
