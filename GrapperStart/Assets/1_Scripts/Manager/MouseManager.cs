using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : SingleTonGeneric<MouseManager>
{
    
    public void MouseSingleTonSet()
    {
      
    }

    Texture2D idleIcon;
    Texture2D enemyIdleIcon;
    Texture2D objIdleIcon;

    public enum CursorType
    {
        None,
        Idle,
        enemyIdle,
        objIdle
    }

    CursorType _cursorType = CursorType.None;
    void Start()
    {


        idleIcon = ResourceManager.Instance.Load<Texture2D>("Texture/Mouse_Idle");
        enemyIdleIcon = ResourceManager.Instance.Load<Texture2D>("Texture/Mouse_enemy_Idle");
        objIdleIcon = ResourceManager.Instance.Load<Texture2D>("Texture/Mouse_Obj_Idle");
    }

    public void SetCursorType(CursorType newType)
    {
        
        if (_cursorType != newType)
        {
            

            switch (newType)
            {
                case CursorType.Idle:
                    Cursor.SetCursor(idleIcon, Vector2.zero, CursorMode.Auto);
                    break;

                case CursorType.enemyIdle:
                    Cursor.SetCursor(enemyIdleIcon, Vector2.zero, CursorMode.Auto);
                    break;
                case CursorType.objIdle:
                    Cursor.SetCursor(objIdleIcon, Vector2.zero, CursorMode.Auto);
                    break;
            }


            //Cursor.lockState = CursorLockMode.Confined;
            _cursorType = newType;
        }
    }


}

// Update is called once per frame


