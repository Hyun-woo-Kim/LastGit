using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager resource;

    public static ResourceManager Resource
    {
        get
        {
            return resource;
        }
    }

    private void Awake()
    {
        if (resource != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            resource = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public T Load<T>(string path) where T : Object
    {
        T resource = Resources.Load<T>(path);
        if (resource == null)
        {
            Debug.LogError($"Resource not found at path: {path}");
        }
        return resource;
    }
}
