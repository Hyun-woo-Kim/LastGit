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
    public float patrolDistance = 10.0f; // 순찰 거리

    private Vector2 startPosition;
    private Vector2 patrolDirection = Vector2.right;
    protected override void PatrolMovement(BloodState state)
    {
        Debug.Log("PatrolMovement" + startPosition);
        Debug.Log("PatrolMovement" + patrolDirection);

        transform.Translate(patrolDirection * patrolSpeed * Time.deltaTime);

        // 순찰 거리를 초과했을 경우 방향을 전환합니다.
        if (Vector2.Distance(transform.position, startPosition) >= patrolDistance)
        {
            // 방향을 반전시킵니다.
            patrolDirection = -patrolDirection;
            Debug.Log("Distance" + patrolDirection);
            // 이동 거리 초기화
            startPosition = transform.position;
        }


    }
}
