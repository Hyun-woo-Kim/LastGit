using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCollider : MonoBehaviour
{
     EdgeCollider2D childboxCollider;
    public GameObject childObject;
    public int childIndex = 0;
    SpriteRenderer spriteRenderer;

    public Sprite LightOnSprite;
     Sprite LightOffSprite;

    void Start()
    {
      
        spriteRenderer = GetComponent<SpriteRenderer>();
        LightOffSprite = spriteRenderer.sprite;
        childObject = transform.GetChild(childIndex).gameObject;
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
            spriteRenderer.sprite = LightOnSprite;
            childObject.SetActive(true);
            childObject.GetComponentInChildren<EdgeCollider2D>().isTrigger = false;     // childboxCollider�� ����� �ڵ�
        

        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Hook")
        {
            spriteRenderer.sprite = LightOffSprite;
            childObject.SetActive(false);
            childObject.GetComponentInChildren<EdgeCollider2D>().isTrigger = true;     // childboxCollider�� ����� �ڵ�



        }
    }

    //ó���� ��� �����ϰ� �ݶ��̴� ���� -> isTrigger = true
    //��ũ�� �±� �浹 �� ��� �Ұ����ϰ� ����. -> isTrigger = false 
    //�׷��ø� �� �÷��̾ ���� ������ isTrigger = true



}
