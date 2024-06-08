using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PpZakoom : MonoBehaviour
{
    private IObjectPool<PpZakoom> _managedPool;

    public void SetManagedPool(IObjectPool<PpZakoom> pool)
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
