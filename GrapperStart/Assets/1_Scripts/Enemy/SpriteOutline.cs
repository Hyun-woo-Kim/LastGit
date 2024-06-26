using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class SpriteOutline : MonoBehaviour
{
    public Color color = Color.red;

    [Range(0, 16)]
    public int outlineSize;

    private SpriteRenderer spriteRenderer;

    void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateOutline(true);
    }

    //private void Start()
    //{
    //    spriteRenderer = GetComponent<SpriteRenderer>();

    //    UpdateOutline(true);
    //}
    void OnDisable()
    {
        UpdateOutline(false);
    }


    void UpdateOutline(bool outline)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_Outline", outline ? 1f : 0);
        mpb.SetColor("_OutlineColor", color);
        mpb.SetFloat("_OutlineSize", outlineSize);
        spriteRenderer.SetPropertyBlock(mpb);
    }
}
