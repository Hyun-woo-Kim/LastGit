using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    public GameObject dialogueUI;
    bool isPlayerInteracting = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            isPlayerInteracting = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            isPlayerInteracting = false;
        }
    }

    private void Update()
    {
        if (isPlayerInteracting && Input.GetKeyDown(KeyCode.K))
        {
            dialogueUI.SetActive(true);
        }
    }
}

