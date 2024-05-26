using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleTonGeneric<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance; //���׸��̹Ƿ� Ÿ���� T�� ���´�. 

    public static T Instance
    {
        get
        {
            if (instance == null) //instance�� ���ٸ�
            {
                instance = (T)FindAnyObjectByType(typeof(T)); //ã�´�.

                if (instance == null) //ã�Ҵµ� ���� �� ���ٸ� 
                {
                    GameObject obj = new GameObject(typeof(T).Name, typeof(T)); //���ӿ������� �����Ѵ�. 
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
       
    }
}
