using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomBerManHand : MonoBehaviour
{
    public ParticleSystem explosionParticlesPrefab; // 파티클 시스템을 할당할 변수
    private ParticleSystem explosionParticles;
    public float particleDuration = 1.0f;
    private void OnEnable()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 충돌한 위치를 파악
            Vector3 collisionPosition = other.transform.position;

            // 충돌 위치에 파티클 생성
            if (explosionParticles == null)
            {
                // 파티클 시스템을 충돌 위치에 생성하고 재생
                if (explosionParticlesPrefab != null)
                {
                    Debug.Log("이펙트");

                    // 파티클 시스템 프리팹을 인스턴스화하여 충돌 위치에 생성
                    explosionParticles = Instantiate(explosionParticlesPrefab, collisionPosition, Quaternion.identity);

                    // 파티클 재생
                    explosionParticles.Play();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 충돌이 끝나면 파티클 제거
            if (explosionParticles != null)
            {
                Destroy(explosionParticles.gameObject);
                explosionParticles = null;
            }
        }
    }


}
