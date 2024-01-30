using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodWorkerPatrol : BloodWorkerState
{
  


    
  


    public override void Initialize(BloodState state)
    {
        startPosition = transform.position;
        Debug.Log("Init" + startPosition);

    }
    public float patrolSpeed = 2.0f;
    public float patrolDistance = 10.0f; // ���� �Ÿ�

    private Vector2 startPosition;
    private Vector2 patrolDirection = Vector2.right;
    protected override void PatrolMovement(BloodState state)
    {
        Debug.Log("PatrolMovement" + startPosition);
        Debug.Log("PatrolMovement" + patrolDirection);

        transform.Translate(patrolDirection * patrolSpeed * Time.deltaTime);

        // ���� �Ÿ��� �ʰ����� ��� ������ ��ȯ�մϴ�.
        if (Vector2.Distance(transform.position, startPosition) >= patrolDistance)
        {
            // ������ ������ŵ�ϴ�.
            patrolDirection = -patrolDirection;
            Debug.Log("Distance" + patrolDirection);
            // �̵� �Ÿ� �ʱ�ȭ
            startPosition = transform.position;
        }


    }
}
