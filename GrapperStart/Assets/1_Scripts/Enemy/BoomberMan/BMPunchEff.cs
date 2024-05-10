using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BMPunchEff : MonoBehaviour
{
    private IObjectPool<BMPunchEff> _ManagedPool;

    private void Start()
    {
       
    }

    public void TransformEff()
    {
        Invoke("DestroyEff", 0.5f);
    }
    public void SetManagedPool(IObjectPool<BMPunchEff> pool)
    {
        _ManagedPool = pool;
    }

    public void DestroyEff()
    {
        _ManagedPool.Release(this);
    }
}
