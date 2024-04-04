using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BoomBerManHand : MonoBehaviour
{
    public GameObject explosionParticlesPrefab; // 파티클 시스템을 할당할 변수

    private GameObject explosion;
    public float particleDel;

    private bool isParticleSpawned = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isParticleSpawned)
        {
            // 충돌한 위치를 파악하여 파티클 생성 및 삭제 코루틴 호출
            Vector3 collisionPosition = other.transform.position;
            StartCoroutine(InstantiateAndDestroyParticles(collisionPosition));
        }
    }

    IEnumerator InstantiateAndDestroyParticles(Vector3 position)
    {
        // 파티클 생성
        explosion = Instantiate(explosionParticlesPrefab, position, Quaternion.identity);
        isParticleSpawned = true;

        // 대기 후 파티클 삭제
        yield return new WaitForSeconds(particleDel);
        DestroyExplosion();
    }

    void DestroyExplosion()
    {
        // 파티클 삭제
        Destroy(explosion);
        isParticleSpawned = false;
    }

}
