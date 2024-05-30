using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    public GameObject dialogueUI;
    bool isPlayerInteracting = false;
    DialogueManager Dm;
    InteractionEvent interactionEvent;
    public GameObject gameobject;

    private void Awake()
    {
        Dm = GetComponent<DialogueManager>();
        //interactionEvent = GetComponent<InteractionEvent>();
    }

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
        if (isPlayerInteracting && Input.GetKeyDown(KeyCode.F))
        {
            dialogueUI.SetActive(true);
            Dm.ShowDialogue(gameobject.transform.GetComponent<InteractionEvent>().GetDialogue());
        }
    }

}

