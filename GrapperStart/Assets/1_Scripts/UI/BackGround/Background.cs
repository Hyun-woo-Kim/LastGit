using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Background : MonoBehaviour
{
    public GameObject imagePrefab; // UI 이미지 프리팹
    public int poolSize = 20; // 풀링의 크기
    public float animationDuration = 1f; // 애니메이션 지속 시간
    public int numberOfImagesToSpawn = 15; // 생성할 이미지 수

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
            // numberOfImagesToSpawn만큼 이미지 생성
            for (int i = 0; i < numberOfImagesToSpawn; i++)
            {
                yield return new WaitForSeconds(Random.Range(0.1f, 0.2f)); // 이미지를 생성하기 전 임의의 시간만큼 대기

                GameObject image = GetPooledImage();
                if (image != null)
                {
                    // 랜덤한 위치에 이미지 배치
                    RectTransform rectTransform = image.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = GetRandomPosition();

                    image.SetActive(true);
                    StartCoroutine(AnimateImage(image)); // 이미지 애니메이션 시작
                }
            }

            yield return new WaitForSeconds(animationDuration); // 모든 이미지의 애니메이션 재생이 끝날 때까지 대기
        }
    }

    IEnumerator AnimateImage(GameObject image)
    {
        yield return new WaitForSeconds(animationDuration); // 애니메이션 지속 시간만큼 대기
        image.SetActive(false); // 이미지 비활성화
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