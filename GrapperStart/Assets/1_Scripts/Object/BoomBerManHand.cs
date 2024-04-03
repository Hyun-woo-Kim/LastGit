using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoomBerManHand : MonoBehaviour
{
    public ParticleSystem explosionParticlesPrefab; // ��ƼŬ �ý����� �Ҵ��� ����
    private ParticleSystem explosionParticles;
    public float particleDuration = 1.0f;
    private void OnEnable()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // �浹�� ��ġ�� �ľ�
            Vector3 collisionPosition = other.transform.position;

            // �浹 ��ġ�� ��ƼŬ ����
            if (explosionParticles == null)
            {
                // ��ƼŬ �ý����� �浹 ��ġ�� �����ϰ� ���
                if (explosionParticlesPrefab != null)
                {
                    Debug.Log("����Ʈ");

                    // ��ƼŬ �ý��� �������� �ν��Ͻ�ȭ�Ͽ� �浹 ��ġ�� ����
                    explosionParticles = Instantiate(explosionParticlesPrefab, collisionPosition, Quaternion.identity);

                    // ��ƼŬ ���
                    explosionParticles.Play();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // �浹�� ������ ��ƼŬ ����
            if (explosionParticles != null)
            {
                Destroy(explosionParticles.gameObject);
                explosionParticles = null;
            }
        }
    }


}
