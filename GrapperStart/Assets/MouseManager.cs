using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    private static MouseManager _instance = null;

    public static MouseManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
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


        idleIcon = ResourceManager.Resource.Load<Texture2D>("Texture/Mouse_Idle");
        enemyIdleIcon = ResourceManager.Resource.Load<Texture2D>("Texture/Mouse_enemy_Idle");
        objIdleIcon = ResourceManager.Resource.Load<Texture2D>("Texture/Mouse_Obj_Idle");
    }

    public void SetCursorType(CursorType newType)
    {
        
        if (_cursorType != newType)
        {
            

            switch (newType)
            {
                case CursorType.Idle:
                    Debug.Log("¸¶¿ì½º ");
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


