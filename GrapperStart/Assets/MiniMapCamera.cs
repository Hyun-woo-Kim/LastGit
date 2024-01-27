using UnityEngine;

public class MiniMapCamera : MonoBehaviour
{
    public Transform player; // 플레이어의 Transform 참조
    public GameObject playerIcon; // 플레이어 아이콘 오브젝트

    public RenderTexture miniMapTexture; // 미니맵 렌더 텍스처

    void LateUpdate()
    {
        if (player != null)
        {
            // 플레이어의 위치를 따라가도록 미니맵 카메라의 위치를 설정
            transform.position = new Vector3(player.position.x, transform.position.y, -10.0f);

            // 미니맵 렌더 텍스처에 렌더링
            if (miniMapTexture != null)
            {
                GetComponent<Camera>().targetTexture = miniMapTexture;
            }

            // 플레이어 아이콘을 미니맵에 그리기
            if (playerIcon != null)
            {
                // 미니맵 텍스처 내에서 플레이어의 위치로 아이콘 위치를 조정
                Vector2 iconPosition = WorldToMiniMap(player.position);
                playerIcon.transform.position = iconPosition;
            }
        }
    }

    Vector2 WorldToMiniMap(Vector3 worldPosition)
    {
        // 플레이어의 위치를 미니맵 텍스처 상의 위치로 변환
        // 여기에 적절한 계산을 통해 월드 좌표를 미니맵 텍스처 좌표로 변환해야 합니다.
        // 예시로 화면 비율에 따라 플레이어의 위치를 변환하는 방법을 제시합니다.
        Vector2 miniMapPosition = new Vector2(worldPosition.x, worldPosition.z);
        miniMapPosition /= 10; // 적절한 비율로 나누어 좌표를 조정
        return miniMapPosition;
    }
}