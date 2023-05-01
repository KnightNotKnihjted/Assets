using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New LootTable",menuName ="Scriptables/LootTable")]
public class LootTable : ScriptableObject
{
    public LootTableValue[] lootTableValues;

    public ItemQuantityComposite[] Roll()
    {
        List<ItemQuantityComposite> result = new();
        foreach(LootTableValue value in lootTableValues)
        {
            result.Add(new()
            {
                item = value.item,
                quantity = value.RollQuantity()
            });
        }
        return result.ToArray();
    }
}