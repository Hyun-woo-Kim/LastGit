using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTonSetting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ResourceManager.Instance.ResourceSingleTonSet();
        MouseManager.Instance.MouseSingleTonSet();
        DialogueManager.Instance.DialogueSingleTonSet();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
