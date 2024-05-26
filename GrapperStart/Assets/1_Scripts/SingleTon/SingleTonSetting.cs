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
        ScenesManager.Instance.SceneSingleTonSet();
        //PlayerUI.Instance.UISingleTonSet();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
