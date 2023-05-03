using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class UI_InventorySlot : MonoBehaviour
{
    public InventoryManager im;

    [SerializeField] private TMP_Text quantityLabelRef;
    public Image iconDisplay;
    private Item storedItem;
    private int itemQuantity = 0;

    private static Tooltip currentTooltip;
    private Tooltip myTooltip;

    private static readonly Regex numberRegex = new(@"\d+");
    public int Extract(string text)
    {
        Match match = numberRegex.Match(text);
        if (match.Success)
        {
            return int.Parse(match.Value);
        }
        else
        {
            return 0; // or throw an exception, or return a default value
        }
    }
    public bool isReadOnly;
    public bool isAutoAdd = true;
    private void Update()
    {
        if (itemQuantity == 0) storedItem = null;

        if (im != null && isAutoAdd)
        {
            if (im.Slots.Contains(this) == false)
            {
                im.Slots.Add(this);
                im.Slots.Sort((x, y) =>
                {
                    int xNumber = Extract(x.name);
                    int yNumber = Extract(y.name);

                    // Compare the numerical values
                    return xNumber.CompareTo(yNumber);
                });
            }
        }

        Color c = iconDisplay.color;
        iconDisplay.color = (storedItem == null) ? new(c.r, c.g, c.b, 0f) : new(c.r, c.g, c.b, 1f);

        if (storedItem != null)
        {
            quantityLabelRef.text = itemQuantity.ToString();
            iconDisplay.sprite = storedItem.Icon;
        }
        else
        {
            quantityLabelRef.text = "";
            iconDisplay.sprite = null;
        }

        if (isReadOnly) return;
        if (IsMouseOver())
        {
            //Tooltip
            if (storedItem != null)
            {
                if (myTooltip == null)
                {
                    myTooltip = Instantiate(PlayerInventoryManager.i.TooltipPrefab,
                        GlobalInputManager.InputMaster.Player.MousePos.ReadValue<Vector2>() + storedItem.TooltipOffset,
                        Quaternion.identity, transform
                        );
                    myTooltip.text = storedItem.ItemName;
                    myTooltip.lifetime = 1f;
                    if (currentTooltip != null)
                    {
                        Destroy(currentTooltip.gameObject);
                    }
                    currentTooltip = myTooltip;
                    currentTooltip.onDestroy += () =>
                    {
                        currentTooltip = null;
                        myTooltip = null;
                    };
                }
                else
                {
                    myTooltip.transform.position = GlobalInputManager.InputMaster.Player.MousePos.ReadValue<Vector2>() + storedItem.TooltipOffset;
                    myTooltip.lifetime = 1f;
                }
            }
            else
            {
                if (myTooltip != null)
                {
                    Destroy(myTooltip.gameObject);
                }
            }

            //Gather All  2Tap L-Click
            if (GlobalInputManager.DoubleTap(GlobalInputManager.InputMaster.Player.LeftClick))
            {
                if (storedItem != null && im != null)
                {
                    int originalQuantity = itemQuantity;
                    int r = im.CollectAllItemsOfType(storedItem, storedItem.MaxStackSize, this);
                    if (r > 0)
                    {
                        var drag = PlayerInventoryManager.i.itemDragged;
                        if (drag != null)
                        {
                            var org = drag.origin;
                            if (org == this)
                            {
                                drag.quantityHeld = r - originalQuantity;
                            }
                        }
                    }
                }
            }
            //Drag Handle    L-Click
            else if (GlobalInputManager.InputMaster.Player.LeftClick.WasPressedThisFrame())
            {
                if (PlayerInventoryManager.isDraggingItem == false)
                {
                    if (storedItem != null)
                    {
                        if (PlayerInventoryManager.GrabItem(this, storedItem, itemQuantity))
                        {
                            RemoveItem(itemQuantity);
                        }
                    }
                }
            }
            //Grab Half      R-Click
            else if (GlobalInputManager.InputMaster.Player.RightClick.WasPressedThisFrame())
            {
                if (PlayerInventoryManager.isDraggingItem == false)
                {
                    if (storedItem != null)
                    {
                        if (PlayerInventoryManager.GrabItem(this, storedItem, Mathf.CeilToInt(itemQuantity * 0.5f)))
                        {
                            RemoveItem(Mathf.CeilToInt(itemQuantity * 0.5f));
                        }
                    }
                }
            }
            //Drop Item         Q
            else if (GlobalInputManager.InputMaster.Player.DropItem.WasPressedThisFrame())
            {
                if (PlayerInventoryManager.isDraggingItem == false && storedItem != null)
                {
                    PlayerInventoryManager.DropItem(storedItem, 1);
                    RemoveItem(1);
                }
            }
        }
    }
    public Item GetItem()
    {
        return storedItem;
    }
    public int GetItemQuantity()
    {
        return itemQuantity;
    }
    public bool TryAddItem(Item item, int amount, out int leftovers)
    {
        bool canAdd = true;

        leftovers = 0;

        if (storedItem != null)
        {
            if (item != storedItem) canAdd = false;
            if (itemQuantity >= storedItem.MaxStackSize) canAdd = false;
        }

        if (canAdd)
        {
            if (storedItem == null)
            {
                storedItem = item;
            }

            if (amount == storedItem.MaxStackSize && itemQuantity > 0)
            {
                leftovers = amount;
                canAdd = false;
            }
            else if (itemQuantity + amount > storedItem.MaxStackSize)
            {
                leftovers = itemQuantity + amount - storedItem.MaxStackSize;
                itemQuantity = storedItem.MaxStackSize;
            }
            else if (itemQuantity + amount == storedItem.MaxStackSize)
            {
                itemQuantity = storedItem.MaxStackSize;
            }
            else
            {
                itemQuantity += amount;
            }
        }
        else
        {
            leftovers = amount; // Set leftovers to the amount when the item cannot be added
        }

        return canAdd;
    }
    public int RemoveItem(int amount)
    {
        int leftovers = 0;

        if (itemQuantity > amount)
        {
            itemQuantity -= amount;
        }
        else if(itemQuantity == amount)
        {
            itemQuantity = 0;
            storedItem = null;
        }
        else
        {
            itemQuantity = 0;
            storedItem = null;
            leftovers -= itemQuantity;
        }

        return leftovers;
    }
    public bool IsMouseOver()
    {
        RectTransform rectTrans = GetComponent<RectTransform>();
        Vector2 localMousePosition = rectTrans.InverseTransformPoint(GlobalInputManager.InputMaster.Player.MousePos.ReadValue<Vector2>());
        return rectTrans.rect.Contains(localMousePosition);
    }
    public void SetValue(Item itm, int qty)
    {
        storedItem = itm;
        itemQuantity = qty;
    }
}
