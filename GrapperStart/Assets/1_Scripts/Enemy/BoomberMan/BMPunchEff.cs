using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BMPunchEff : MonoBehaviour
{
    private IObjectPool<BMPunchEff> _ManagedPool;

    public SpriteRenderer effSpr;
    public float effDestroySpeed;

    private void Start()
    {
       
    }

    private void OnEnable()
    {
        
    }

    public void TransformEff()
    {
       
        Invoke("DestroyEff", effDestroySpeed);
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
