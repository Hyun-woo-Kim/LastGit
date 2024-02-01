using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTonGeneric<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance; //제네릭이므로 타입인 T를 적는다. 

    public static T Instance
    {
        get
        {
            if (instance == null) //instance가 없다면
            {
                instance = (T)FindAnyObjectByType(typeof(T)); //찾는다.

                if (instance == null) //찾았는데 만약 또 없다면 
                {
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T)); //게임오브젝를 생성한다. 
                    instance = obj.GetComponent<T>();
                }

            }
            return instance;
        }
    }

    private void Awake()
    {
        if (transform.parent != null && transform.root != null)
        {
            DontDestroyOnLoad(this.transform.root.gameObject);
        }
        DontDestroyOnLoad(this.gameObject); //만약 이 스크립트가 자식 오브젝트에 있다면 제대로 동작하지 않는다.
    }
}
