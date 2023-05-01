using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public LootTable myLootTable;
    public BlockType myBlockType;
    public int endurance = 2;
    public Action onBreak = new (() => { });
    public Action<int> onUpdateEndurance = new((_) => { });

    public virtual void OnMouseDown()
    {
        endurance--;
        if (endurance <= 0)
        {
            onBreak?.Invoke();
            if (myLootTable != null)
            {
                Vector3 dir = new(UnityEngine.Random.Range(-.75f, .75f), UnityEngine.Random.Range(-1.75f, -.25f));
                foreach (ItemQuantityComposite itemComp in myLootTable.Roll())
                {
                    PlayerInventoryManager.SpawnItem(itemComp.item, itemComp.quantity, transform.position + dir);
                }
            }
            Destroy(gameObject);
        }
        else
        {
            onUpdateEndurance?.Invoke(endurance);
        }
    }
}
