using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LootTableValue
{
    public Item item;
    public QuantityChanceRoll[] quantities;

    public int RollQuantity()
    {
        int valueToReturn = 0;
        for(int i = 0; i < quantities.Length; i++)
        {
            if (quantities[i].Roll() == 0)
            {
                if(i > 0)
                {
                    valueToReturn = quantities[i - 1].prize;
                }
                break;
            }
            else if(i == quantities.Length - 1)
            {
                valueToReturn = quantities[i].prize;
            }
            else
            {
                continue;
            }
        }
        return valueToReturn;
    }
}