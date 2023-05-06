using System.Collections.Generic;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;

public class WorkBench : InventoryManager
{

    [Header("WORK BENCH SPECIALS")]
    public RectTransform rowPrefab;
    public GameObject plusObject;
    public UI_InventorySlot individualRecipe;
    public UI_InventorySlot recipeResultSlotPrefab;
    [SerializeField] private UI_RecipeObject recipeObjectPrefab;
    public UI_InventorySlot resultSlot;
    public RectTransform contentRect;
    [SerializeField] private Recipe[] recipes;

    public override IEnumerator Start()
    {
        DialogueManager.i.questWorkbench = this;
        StartCoroutine(base.Start());
        yield return null;
        float y = 0;
        for (int i = 0; i < recipes.Length; i++)
        {
            UI_RecipeObject recipeObj = Instantiate(recipeObjectPrefab, contentRect);
            recipeObj.myRecipe = recipes[i];
            recipeObj.im = this;
            recipeObj.Init(out int x, out UnityEngine.UI.Button resultButton);
            if (resultButton != null)
            {
                Recipe currentRecipe = recipes[i];
                resultButton.onClick.AddListener(() => TryCraftItem(currentRecipe));
            }

            y += x * 100;
        }
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, y);
    }
    public virtual void Craft()
    {
        if (resultSlot.GetItem() == null)
        {
            foreach (Recipe recipe in recipes)
            {
                TryCraftItem(recipe);
            }
        }
    }
    public virtual void TryCraftItem(Recipe recipe)
    {
        bool canCraft = true;

        // Check if the required items are available in WorkBench or Player inventory
        foreach (ItemQuantityComposite itemQuantity in recipe.leftSide)
        {
            int totalAvailable = GetCountOfItemType(itemQuantity.item) +
                ((!PlayerInventoryManager.i.active)? 0 : PlayerInventoryManager.i.GetCountOfItemType(itemQuantity.item));
            if (totalAvailable < itemQuantity.quantity)
            {
                canCraft = false;
                break;
            }
        }

        // If the required items are not available, return
        if (!canCraft)
        {
            return;
        }

        // Remove the required items from WorkBench and Player inventory
        foreach (ItemQuantityComposite itemQuantity in recipe.leftSide)
        {
            int removeFromWorkbench = Math.Min(GetCountOfItemType(itemQuantity.item), itemQuantity.quantity);
            int removeFromPlayer = itemQuantity.quantity - removeFromWorkbench;

            if (removeFromWorkbench > 0)
            {
                RemoveItem(itemQuantity.item, removeFromWorkbench, false);
            }

            if (removeFromPlayer > 0)
            {
                PlayerInventoryManager.i.RemoveItem(itemQuantity.item, removeFromPlayer, false);
            }
        }

        // Set the crafted item in the result slot
        resultSlot.TryAddItem(recipe.rightSide.item, recipe.rightSide.quantity, out int leftovers);
        int qty = leftovers;
        if(qty > 0)
        {
            PlayerInventoryManager.i.AddItem(recipe.rightSide.item, leftovers, out qty);
            if (qty > 0) {
                AddItem(recipe.rightSide.item, qty, out qty);
                if (qty > 0)
                {
                    PlayerInventoryManager.DropItem(recipe.rightSide.item, qty);
                }
            }
        }
        QuestManager.ItemCollected(recipe.rightSide.item, recipe.rightSide.quantity - qty);
    }

}
