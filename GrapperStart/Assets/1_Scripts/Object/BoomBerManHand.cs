using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BoomBerManHand : MonoBehaviour
{
    public GameObject explosionParticlesPrefab; // 파티클 시스템을 할당할 프리팹 변수
    public ParticleSystem effPunch; // 충돌 이펙트를 재생할 파티클 시스템

    private bool isParticleSpawned = false; // 이펙트가 생성되었는지 여부를 나타내는 플래그

    // 충돌이 감지되었을 때 호출되는 메서드
    private void OnCollisionEnter(Collision collision)
    {
        // 충돌한 객체가 플레이어인지 확인
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("플레이어와의 충돌");
            // 이펙트가 생성되지 않았다면
            if (!isParticleSpawned)
            {
                // 파티클 시스템 프리팹을 인스턴스화하여 생성
                Debug.Log("이펙트 생성");
                    // 파티클 시스템 프리팹을 인스턴스화하여 생성
                    GameObject explosion = Instantiate(explosionParticlesPrefab, transform.position, Quaternion.identity);
                    // 이펙트 재생
                    explosion.GetComponent<ParticleSystem>().Play();
                    // 이펙트가 생성되었음을 표시
                    isParticleSpawned = true;
                
            }
        }
    }
}
