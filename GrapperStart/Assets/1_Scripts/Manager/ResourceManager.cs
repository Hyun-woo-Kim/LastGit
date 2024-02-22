using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : SingleTonGeneric<ResourceManager>
{
    public void ResourceSingleTonSet()
    {
       
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
