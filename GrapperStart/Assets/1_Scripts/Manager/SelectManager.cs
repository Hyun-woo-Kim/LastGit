using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectManager : SingleTonGeneric<SelectManager>
{
    public GameObject SelectUI;

    public void SelectSingleTonSet()
    {

    }


    public bool isSelectUI = false;
    public void UIActive()
    {
        isSelectUI = true;
        SelectUI.gameObject.SetActive(true);
    }
    public void UIBeAcitve()
    {
        isSelectUI = false;
        SelectUI.gameObject.SetActive(false);
    }
}
