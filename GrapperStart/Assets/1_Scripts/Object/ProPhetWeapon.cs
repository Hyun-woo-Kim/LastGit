using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ProPhetWeapon : MonoBehaviour
{
    private IObjectPool<ProPhetWeapon> _ManagedPool;

    public void SetManagedPool(IObjectPool<ProPhetWeapon> pool)
    {
        _ManagedPool = pool;
    }

    public void InvokeDestroyWeapon()
    {
        Invoke("DestroyWeapon", 1.0f);
    }
    public void DestroyWeapon()
    {
        _ManagedPool.Release(this);
    }
}
