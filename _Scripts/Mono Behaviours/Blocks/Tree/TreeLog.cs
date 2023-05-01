using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))] 
public class TreeLog : Block
{
    private int childIndex;

    private SpriteRenderer sr;
    private PolygonCollider2D col;

    [HideInInspector] public TreeSettings currentSettings;

    [HideInInspector] public TreeSettings headSettings;
    [HideInInspector] public TreeSettings bodySettings;
    [HideInInspector] public TreeSettings rootSettings;

    [HideInInspector] public TreeBrain brain;

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<PolygonCollider2D>();
    }
    private void Update()
    {
        childIndex = transform.GetSiblingIndex();

        if (childIndex == transform.parent.childCount - 1)
        {
            currentSettings = headSettings;
        }
        else if (childIndex == 0)
        {
            currentSettings = rootSettings;
        }
        else
        {
            currentSettings = bodySettings;
        }
        if (sr != null)
        {
            if (sr.sprite != currentSettings.sprite)
            {
                Destroy(col);
                sr.sprite = currentSettings.sprite;
                col = gameObject.AddComponent<PolygonCollider2D>();

            }
            sr.color = currentSettings.spriteColor;
        }

        if (col != null)
        {
            col.enabled = childIndex <= Mathf.CeilToInt(transform.parent.childCount * 0.33f);
        }

        transform.localPosition = currentSettings.relPos + Vector3.up * childIndex;
        transform.rotation = Quaternion.Euler(currentSettings.rotation);
        transform.localScale = currentSettings.scale;
    }
}
