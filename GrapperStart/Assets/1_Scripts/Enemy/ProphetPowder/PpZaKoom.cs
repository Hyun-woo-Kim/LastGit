using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PpZaKoom : MonoBehaviour
{
    private IObjectPool<PpZaKoom> _pool;

    // ���� ���� �ʿ��� ������
    public float lifetime = 5f; // ���� ���� ����
    private float _lifeTimer;

    // ������Ʈ Ǯ�� �����ϴ� �޼���
    public void SetManagedPool(IObjectPool<PpZaKoom> pool)
    {
        _pool = pool;
    }

    private void OnEnable()
    {
        // ���� ���� Ȱ��ȭ�� �� ���� Ÿ�̸� �ʱ�ȭ
        _lifeTimer = lifetime;
    }

   

    private void OnDisable()
    {
        // ���� ���� ��Ȱ��ȭ�� �� �߰����� ���� �۾�
    }
}
