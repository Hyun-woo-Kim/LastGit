using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public float damage = 10f; // 함정 데미지
    public Animator animatorPlayer;

    private void Start()
    {
        animatorPlayer = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            Debug.Log("함정 데미지");
            //hp체력에서 데미지 들어가는 함수 가져오면 끝
           
        }
    }
}