using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataBase : SingletonBehaviour<ItemDataBase>
{
    public Item[] items;

    public static Item GetItem(string name)
    {
        for(int j = 0; j < i.items.Length; j++)
        {
            if(i.items[j].ItemName.ToUpper().Contains(name.ToUpper()))
            {
                return i.items[j];
            }
        }
        return null;
    }
}