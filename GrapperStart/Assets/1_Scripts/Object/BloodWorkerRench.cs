using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodWorkerRench : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        playerUI = FindFirstObjectByType<PlayerUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    PlayerUI playerUI;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerUI.TakeDamage(1);
        }

       
    }
}
