using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BoomBerManHand : MonoBehaviour
{
    public GameObject explosionParticlesPrefab; // ��ƼŬ �ý����� �Ҵ��� ����

    private GameObject explosion;
    public float particleDel;

    private bool isParticleSpawned = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isParticleSpawned)
        {
            // �浹�� ��ġ�� �ľ��Ͽ� ��ƼŬ ���� �� ���� �ڷ�ƾ ȣ��
            Vector3 collisionPosition = other.transform.position;
            StartCoroutine(InstantiateAndDestroyParticles(collisionPosition));
        }
    }

    IEnumerator InstantiateAndDestroyParticles(Vector3 position)
    {
        // ��ƼŬ ����
        explosion = Instantiate(explosionParticlesPrefab, position, Quaternion.identity);
        isParticleSpawned = true;

        // ��� �� ��ƼŬ ����
        yield return new WaitForSeconds(particleDel);
        DestroyExplosion();
    }

    void DestroyExplosion()
    {
        // ��ƼŬ ����
        Destroy(explosion);
        isParticleSpawned = false;
    }

}
