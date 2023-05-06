using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRequiredBlock : Block
{
    [SerializeField] private ItemQuantityComposite[] itemsRequired;

    public override void OnMouseDown()
    {
        bool valid = true;
        foreach (ItemQuantityComposite itemRequired in itemsRequired) {
            if(PlayerInventoryManager.i.GetCountOfItemType(itemRequired.item) < itemRequired.quantity)
            {
                valid = false;
                break;
            }
        }
        if (valid)
        {
            base.OnMouseDown();
        }
    }
}
