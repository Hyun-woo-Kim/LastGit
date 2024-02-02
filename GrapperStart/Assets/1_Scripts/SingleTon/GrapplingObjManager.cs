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

    public float brightness = 0.1f; // �����ϰ��� �ϴ� �� ��

    public void brightnessUp(Collider2D obj)
    {
        spriteRenderer = obj.GetComponent<SpriteRenderer>();
        // ���� ��������Ʈ�� ������ ������ �� ���� �����Ͽ� �ٽ� �����մϴ�.
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
