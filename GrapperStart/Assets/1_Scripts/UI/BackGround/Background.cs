using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Background : MonoBehaviour
{
    public GameObject imagePrefab; // UI �̹��� ������
    public int poolSize = 20; // Ǯ���� ũ��
    public float animationDuration = 1f; // �ִϸ��̼� ���� �ð�
    public int numberOfImagesToSpawn = 15; // ������ �̹��� ��

    private List<GameObject> pooledImages = new List<GameObject>();
    private Canvas canvas;
    private RectTransform canvasRect;

    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();

        InitializePool();
        StartCoroutine(SpawnImages());
    }

    void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(imagePrefab, transform);
            obj.SetActive(false);
            pooledImages.Add(obj);
        }
    }

    IEnumerator<WaitForSeconds> SpawnImages()
    {
        while (true)
        {
            // numberOfImagesToSpawn��ŭ �̹��� ����
            for (int i = 0; i < numberOfImagesToSpawn; i++)
            {
                yield return new WaitForSeconds(Random.Range(0.1f, 0.2f)); // �̹����� �����ϱ� �� ������ �ð���ŭ ���

                GameObject image = GetPooledImage();
                if (image != null)
                {
                    // ������ ��ġ�� �̹��� ��ġ
                    RectTransform rectTransform = image.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = GetRandomPosition();

                    image.SetActive(true);
                    StartCoroutine(AnimateImage(image)); // �̹��� �ִϸ��̼� ����
                }
            }

            yield return new WaitForSeconds(animationDuration); // ��� �̹����� �ִϸ��̼� ����� ���� ������ ���
        }
    }

    IEnumerator AnimateImage(GameObject image)
    {
        yield return new WaitForSeconds(animationDuration); // �ִϸ��̼� ���� �ð���ŭ ���
        image.SetActive(false); // �̹��� ��Ȱ��ȭ
    }

    Vector2 GetRandomPosition()
    {
        float x = Random.Range(-canvasRect.sizeDelta.x / 2f, canvasRect.sizeDelta.x / 2f);
        float y = Random.Range(-canvasRect.sizeDelta.y / 2f, canvasRect.sizeDelta.y / 2f);
        return new Vector2(x, y);
    }

    GameObject GetPooledImage()
    {
        foreach (GameObject obj in pooledImages)
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }
        return null;
    }
}