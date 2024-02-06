using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTonSetting : MonoBehaviour //메인카메라에 들어있음.
{
    // Start is called before the first frame update
    void Start()
    {
        ResourceManager.Instance.ResourceSingleTonSet();
        MouseManager.Instance.MouseSingleTonSet();
        DialogueManager.Instance.DialogueSingleTonSet();
        GrapplingObjManager.Instance.ObjManagerSingleTonSet();
        SelectManager.Instance.SelectSingleTonSet();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
