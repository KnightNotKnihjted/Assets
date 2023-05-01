using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private RectTransform inventoryPanel;
    private List<UI_InventorySlot> slots = new();

    private void Awake()
    {
        onUpdate = (active) =>
        {
            if (Panel.activeSelf && !active)
            {
                PlayerInventoryManager.i.otherInventoryManager = this;
            }
            else if (!(PlayerInventoryManager.i.otherInventoryManager != null &&
           PlayerInventoryManager.i.otherInventoryManager != this))
            {
                //Someone is turning off
                PlayerInventoryManager.i.otherInventoryManager = null;
            }
        };
    }

    public GameObject Panel { get => panel; set => panel = value; }
    public RectTransform InventoryPanel { get => inventoryPanel; set => inventoryPanel = value; }
    public List<UI_InventorySlot> Slots { get => slots; set => slots = value; }

    public virtual IEnumerator Start()
    {
        if(Panel.activeSelf == false) UpdatePanel();
        yield return new WaitForEndOfFrame();
        UpdatePanel();
    }
    public virtual void AddItem(Item _item, int _qty, out int leftovers)
    {
        leftovers = _qty;
        int qty = _qty;
        Item item = _item;
        for (int j = 0; j < slots.Count; j++)
        {
            if (slots[j].GetItem() != null && slots[j].GetItem() != item) continue;
            if (slots[j].GetItem() != null)
            { 
                if (slots[j].GetItemQuantity() >= slots[j].GetItem().MaxStackSize) continue;
            }

            // Calculate the quantity that can be added to the target slot
            int spaceAvailable = slots[j].GetItem() == null ? item.MaxStackSize :item.MaxStackSize - slots[j].GetItemQuantity();

            if (spaceAvailable > 0)
            {
                int quantityToAdd = Mathf.Min(spaceAvailable, qty);
                slots[j].TryAddItem(item, quantityToAdd, out _);
                qty -= quantityToAdd;
            }

            leftovers = qty;

            if (qty == 0)
            {
                break;
            }
        }
    }
    public int CollectAllItemsOfType(Item targetType, int maxStackSize, UI_InventorySlot targetSlot)
    {
        int totalQuantity = targetSlot.GetItemQuantity();

        // Sum up the item counts in all slots of the targetType, except the target slot.
        foreach (UI_InventorySlot slot in Slots)
        {
            if (slot == targetSlot)
            {
                continue;
            }

            Item item = slot.GetItem();
            int quantity = slot.GetItemQuantity();

            if (item != null && item == targetType)
            {
                totalQuantity += quantity;

                // Clear the slot.
                slot.SetValue(null, 0);
            }
        }

        // Add the items back into the target slot.
        int spaceAvailable = maxStackSize - targetSlot.GetItemQuantity();
        int quantityToAdd = Mathf.Min(spaceAvailable, totalQuantity);
        AddItem(targetType, quantityToAdd, out _);

        return quantityToAdd;
    }

    public virtual bool RemoveItem(Item _item, int _qty)
    {
        bool hasSpace = false;

        int qty = _qty;
        Item item = _item;

        Action reward = new(() => { });

        for (int j = 0; j < slots.Count; j++)
        {
            if (slots[j].GetItem() != item) continue;

            // Calculate the quantity that can be added to the target slot
            int removable = Mathf.Min(qty, slots[j].GetItemQuantity());
            print((removable > 0) ? removable : null);

            qty -= removable;

            reward += () =>
            {
                slots[j].RemoveItem(removable);
                PlayerInventoryManager.DropItem(item, removable);
            };

            if (qty <= 0)
            {
                hasSpace = true;
                break;
            }
        }

        if (hasSpace)
        {
            //Give Out The Reward
            reward?.Invoke();

            Destroy(PlayerInventoryManager.i.itemDragged.gameObject);
            PlayerInventoryManager.i.itemDragged = null;
        }

        return hasSpace;
    }
    private Action<bool> onUpdate = new (_ => { });
    public virtual void UpdatePanel()
    {
        //True means we are on now! Else we are off!
        if (Panel.activeSelf)
        {
            PlayerInventoryManager.i.otherInventoryManager = null;
            PlayerInventoryManager.onUpdatePanel -= onUpdate;
        }
        else
        {
            PlayerInventoryManager.i.otherInventoryManager = this;
            PlayerInventoryManager.onUpdatePanel += onUpdate;
        }

        Panel.SetActive(!Panel.activeSelf);
    }
}
