using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))] 
public class TreeLog : Block
{
    private int childIndex;

    private SpriteRenderer sr;
    private PolygonCollider2D col;

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

        if (col != null)
        {
            col.enabled = childIndex <= Mathf.CeilToInt(transform.parent.childCount * 0.33f);
        }
    }
}
