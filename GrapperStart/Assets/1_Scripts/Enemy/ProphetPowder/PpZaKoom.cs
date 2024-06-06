using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PpZaKoom : MonoBehaviour
{
    private IObjectPool<PpZaKoom> _pool;

    // 자쿰 돌에 필요한 변수들
    public float lifetime = 5f; // 자쿰 돌의 수명
    private float _lifeTimer;

    // 오브젝트 풀을 설정하는 메서드
    public void SetManagedPool(IObjectPool<PpZaKoom> pool)
    {
        _pool = pool;
    }

    private void OnEnable()
    {
        // 자쿰 돌이 활성화될 때 수명 타이머 초기화
        _lifeTimer = lifetime;
    }

   

    private void OnDisable()
    {
        // 자쿰 돌이 비활성화될 때 추가적인 정리 작업
    }
}
