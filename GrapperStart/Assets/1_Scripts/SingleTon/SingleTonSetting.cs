using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTonSetting : MonoBehaviour //����ī�޶� �������.
{
    // Start is called before the first frame update
    void Start()
    {
        ResourceManager.Instance.ResourceSingleTonSet();
        MouseManager.Instance.MouseSingleTonSet();
        ScenesManager.Instance.SceneSingleTonSet();
        //PlayerUI.Instance.UISingleTonSet();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
