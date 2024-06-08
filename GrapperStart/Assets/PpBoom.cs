using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PpBoom : MonoBehaviour
{
    private IObjectPool<PpBoom> _managedPool;

    public void SetManagedPool(IObjectPool<PpBoom> pool)
    {
        _managedPool = pool;
    }

    public void ReturnToPool()
    {
        if (_managedPool != null)
        {
            _managedPool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
