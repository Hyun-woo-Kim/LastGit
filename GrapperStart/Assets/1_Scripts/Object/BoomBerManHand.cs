using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BoomBerManHand : MonoBehaviour
{
    public GameObject explosionParticlesPrefab; // ��ƼŬ �ý����� �Ҵ��� ������ ����
    public ParticleSystem effPunch; // �浹 ����Ʈ�� ����� ��ƼŬ �ý���

    private bool isParticleSpawned = false; // ����Ʈ�� �����Ǿ����� ���θ� ��Ÿ���� �÷���

    // �浹�� �����Ǿ��� �� ȣ��Ǵ� �޼���
    private void OnCollisionEnter(Collision collision)
    {
        // �浹�� ��ü�� �÷��̾����� Ȯ��
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("�÷��̾���� �浹");
            // ����Ʈ�� �������� �ʾҴٸ�
            if (!isParticleSpawned)
            {
                // ��ƼŬ �ý��� �������� �ν��Ͻ�ȭ�Ͽ� ����
                Debug.Log("����Ʈ ����");
                    // ��ƼŬ �ý��� �������� �ν��Ͻ�ȭ�Ͽ� ����
                    GameObject explosion = Instantiate(explosionParticlesPrefab, transform.position, Quaternion.identity);
                    // ����Ʈ ���
                    explosion.GetComponent<ParticleSystem>().Play();
                    // ����Ʈ�� �����Ǿ����� ǥ��
                    isParticleSpawned = true;
                
            }
        }
    }
}
