using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class Trap : MonoBehaviour
{
    public float damage; // 함정 데미지
    PlayerUI playerUIScr;

    public bool isTrap = false;

    private void Start()
    {
        playerUIScr = FindAnyObjectByType<PlayerUI>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            isTrap = true;

            playerUIScr.TakeDamage(damage);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isTrap = false;


        }
    }

}