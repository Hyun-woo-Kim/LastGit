using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueNPC : MonoBehaviour, IInteractable
{
    #region Variables

    [SerializeField]
    Dialogue dialogue;

    bool isStartDialogue = false;

    GameObject interactGO;

    #endregion Variables

    #region Unity Methods
    #endregion Unity Methods

    #region IInteractable Interface

    [SerializeField]
    private float distance = 2.0f;

    public float Distance => distance;

    public void Interact(GameObject other)
    {
        float calcDistance = Vector2.Distance(other.transform.position, transform.position);
        if (calcDistance > distance)
        {
            return;
        }

        if (isStartDialogue)
        {
            return;
        }

        if(calcDistance > distance && Input.GetKeyDown(KeyCode.K))
        {
            DialogueManager.Instance.StartDialogue(dialogue);
        }

        this.interactGO = other;

        DialogueManager.Instance.OnEndDialogue += OnEndDialogue;
        isStartDialogue = true;

        DialogueManager.Instance.StartDialogue(dialogue);
    }

    public void StopInteract(GameObject other)
    {
        isStartDialogue = false;

        
    }

    #endregion IInteractable Interface

    #region Methods
    private void OnEndDialogue()
    {
        StopInteract(interactGO);
    }
    #endregion Methods
}
