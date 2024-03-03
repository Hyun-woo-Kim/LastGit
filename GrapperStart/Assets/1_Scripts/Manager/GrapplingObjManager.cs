using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingObjManager : SingleTonGeneric<GrapplingObjManager>
{
   public void ObjManagerSingleTonSet()
    {

    }

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    public float brightness = 0.1f; // 조절하고자 하는 명도 값

    public void brightnessUp(Collider2D obj)
    {
        spriteRenderer = obj.GetComponent<SpriteRenderer>();
        // 현재 스프라이트의 색상을 가져온 후 명도를 조절하여 다시 설정합니다.
        originalColor = spriteRenderer.color;
        Color newColor = new Color(originalColor.r + brightness, originalColor.g + brightness, originalColor.b + brightness, originalColor.a);
        spriteRenderer.color = newColor;
    }
    public void brightnessDown(Collider2D obj)
    {
        spriteRenderer = obj.GetComponent<SpriteRenderer>();
        spriteRenderer.color = originalColor;
    }
}
