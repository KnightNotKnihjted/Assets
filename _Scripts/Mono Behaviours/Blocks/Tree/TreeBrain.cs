using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBrain : MonoBehaviour
{
    public WoodType woodType;

    private int length;

    private void Start()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(0).gameObject);
        }

        length = Random.Range(woodType.growRange.x, woodType.growRange.y);

        for(int i = 0; i < length; i++)
        {
            GameObject go = new ($"Tree Part {i + 1}");
            go.transform.parent = transform;
            go.transform.localPosition = Vector3.zero;
            go.AddComponent<SpriteRenderer>().spriteSortPoint = SpriteSortPoint.Pivot;
            Collider2D col = go.AddComponent<PolygonCollider2D>();
            col.enabled = false;
            col.enabled = true;
            TreeLog log = go.AddComponent<TreeLog>();
            log.headSettings = woodType.headTreeSetting;
            log.bodySettings = woodType.bodyTreeSetting;
            log.rootSettings = woodType.rootTreeSetting;
            log.myLootTable = woodType.woodLootTable;
            log.brain = this;
        }
    }
}
