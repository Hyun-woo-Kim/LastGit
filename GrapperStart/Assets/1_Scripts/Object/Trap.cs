using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public float damage = 10f; // ���� ������
    public Animator animatorPlayer;

    private void Start()
    {
        animatorPlayer = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Debug.Log("���� ������");
            //hpü�¿��� ������ ���� �Լ� �������� ��
           
        }
    }
}