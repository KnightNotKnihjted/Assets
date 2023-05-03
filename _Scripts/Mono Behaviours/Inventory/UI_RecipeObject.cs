using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

public class UI_RecipeObject : MonoBehaviour
{
    public Recipe myRecipe;
    public WorkBench im;

    [SerializeField] private RectTransform leftSideParent;
    [SerializeField] private RectTransform rightSideParent;

    public void Init(out int rows, out Button butt)
    {
        rows = 1;
        RectTransform currentRow = Instantiate(im.rowPrefab, leftSideParent);
        butt = null;

        for (int i = 0; i < myRecipe.leftSide.Length; i++)
        {
            ItemQuantityComposite comp = myRecipe.leftSide[i];
            //Handle Row
            if (i % 3 == 0)
            {
                if (i != 0)
                {
                    currentRow = Instantiate(im.rowPrefab, leftSideParent);
                    rows++;
                }
            }
            //Handle No Plus On 1st In Row
            else
            {
                Instantiate(im.plusObject, currentRow);
            }
            var l_obj = Instantiate(im.individualRecipe, currentRow);
            l_obj.SetValue(comp.item, comp.quantity);
            l_obj.isAutoAdd = false;
            l_obj.isReadOnly = true;

        }
        var r_obj = Instantiate(im.recipeResultSlotPrefab, rightSideParent);
        r_obj.SetValue(myRecipe.rightSide.item, myRecipe.rightSide.quantity);
        r_obj.isAutoAdd = false;
        r_obj.isReadOnly = true;

        butt = r_obj.GetComponent<Button>();
        if (butt == null)
        {
            butt = r_obj.gameObject.AddComponent<Button>();
        }
        butt.transition = Selectable.Transition.None;
    }
}
