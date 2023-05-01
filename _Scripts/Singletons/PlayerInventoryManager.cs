using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;
using TMPro;

public class InventoryManagerSingleton<T> : InventoryManager where T : class
{
    public static T i;

    public virtual void Awake()
    {
        if (i == null)
        {
            i = this as T;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }
}

public class PlayerInventoryManager : InventoryManagerSingleton<PlayerInventoryManager>
{
    public Tooltip TooltipPrefab;

    public InventoryManager otherInventoryManager;
    [SerializeField] private PlayerController player;
    [SerializeField] private ItemWorldObject itemDropPrefab;
    [SerializeField] private UI_ItemObject ui_ItemObjectPrefab;

    public static bool isDraggingItem;
    [HideInInspector] public UI_ItemObject itemDragged;
    public override void Awake()
    {
        base.Awake();
        StartCoroutine(AwakeFunc());
    }
    public IEnumerator AwakeFunc()
    {
        yield return new WaitForEndOfFrame();
        GlobalInputManager.InputMaster.Player.OpenInventory.performed += _ => UpdatePanel();
    }
    public static Action<bool> onUpdatePanel = new ((x) => { });
    public override void UpdatePanel()
    {
        if (isDraggingItem && Panel.activeSelf)
        {
            DropItem(itemDragged.myItem, itemDragged.quantityHeld);
            onStopDraggingItem?.Invoke(itemDragged);
        }
        onUpdatePanel?.Invoke(Panel.activeSelf);
        Panel.SetActive(!Panel.activeSelf);
    }
    public static Action<UI_ItemObject> s_onStopDraggingItem = new ((x) => { });
    public static Action<UI_ItemObject> onStopDraggingItem = new ((x) => { });
    private void Update()
    {
        onStopDraggingItem = new((x) => {
            s_onStopDraggingItem?.Invoke(x);
            Destroy(itemDragged.gameObject);
            itemDragged = null;
        });

        isDraggingItem = itemDragged != null;
        if (isDraggingItem)
        {
            Vector2 mousePos = GlobalInputManager.InputMaster.Player.MousePos.ReadValue<Vector2>();
            Vector2 localMousePosition = InventoryPanel.InverseTransformPoint(mousePos);
            itemDragged.transform.position = Input.mousePosition;
            if (!InventoryPanel.rect.Contains(localMousePosition))
            {
                if (otherInventoryManager != null)
                {
                    Vector2 _otherMousePos = otherInventoryManager.InventoryPanel.InverseTransformPoint(mousePos);
                    if (!otherInventoryManager.InventoryPanel.rect.Contains(_otherMousePos))
                    {
                        if (GlobalInputManager.InputMaster.Player.DropItem.IsPressed() ||
                            GlobalInputManager.InputMaster.Player.RightClick.IsPressed())
                        {
                            DropItem(itemDragged.myItem, 1);
                            itemDragged.quantityHeld--;
                            if (itemDragged.quantityHeld <= 0)
                            {
                                onStopDraggingItem?.Invoke(itemDragged);
                            }
                        }
                        else if (GlobalInputManager.InputMaster.Player.LeftClick.IsPressed())
                        {
                            DropItem(itemDragged.myItem, itemDragged.quantityHeld);
                            onStopDraggingItem?.Invoke(itemDragged);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < otherInventoryManager.Slots.Count; i++)
                        {
                            if (otherInventoryManager.Slots[i] == null) continue;
                            if (itemDragged == null) continue;
                            if (!otherInventoryManager.Slots[i].IsMouseOver()) continue;
                            //Handle SwapItem
                            if (otherInventoryManager.Slots[i].GetItem() != itemDragged.myItem && otherInventoryManager.Slots[i].GetItem() != null)
                            {
                                //Check If they Clicked
                                if (!(GlobalInputManager.InputMaster.Player.LeftClick.WasPerformedThisFrame() ||
                                    GlobalInputManager.InputMaster.Player.RightClick.WasPerformedThisFrame())) continue;

                                //Swap Item Logic
                                Item itm = itemDragged.myItem;
                                int qty = itemDragged.quantityHeld;
                                itemDragged.myItem = Slots[i].GetItem();
                                itemDragged.quantityHeld = Slots[i].GetItemQuantity();
                                Slots[i].SetValue(itm, qty);
                            }
                            if (otherInventoryManager.Slots[i].GetItem() != null && otherInventoryManager.Slots[i].GetItem() != itemDragged.myItem) continue;

                            if (GlobalInputManager.InputMaster.Player.LeftClick.WasPerformedThisFrame())
                            {
                                otherInventoryManager.Slots[i].TryAddItem(itemDragged.myItem, itemDragged.quantityHeld, out int left);
                                if (left > 0)
                                {
                                    itemDragged.quantityHeld = left;
                                    continue;
                                }
                                else
                                {
                                    onStopDraggingItem?.Invoke(itemDragged);
                                    break;
                                }
                            }
                            else  if (GlobalInputManager.InputMaster.Player.RightClick.WasPerformedThisFrame())
                            {
                                otherInventoryManager.Slots[i].TryAddItem(itemDragged.myItem, 1, out int left);
                                itemDragged.quantityHeld--;

                                if (left > 0)
                                {
                                    itemDragged.quantityHeld = left;
                                    continue;
                                }
                                else
                                {
                                    onStopDraggingItem?.Invoke(itemDragged);
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < Slots.Count; i++)
            {
                if (Slots[i] == null) continue;
                if (itemDragged == null) continue;
                if (!Slots[i].IsMouseOver()) continue;
                if (Slots[i].GetItem() != itemDragged.myItem && Slots[i].GetItem() != null)
                {
                    //Check If they Clicked
                    if (!(GlobalInputManager.InputMaster.Player.LeftClick.WasPerformedThisFrame() ||
                        GlobalInputManager.InputMaster.Player.RightClick.WasPerformedThisFrame())) continue;

                    //Swap Item Logic
                    Item itm = itemDragged.myItem;
                    int qty = itemDragged.quantityHeld;
                    itemDragged.myItem = Slots[i].GetItem();
                    itemDragged.quantityHeld = Slots[i].GetItemQuantity();
                    Slots[i].SetValue(itm, qty);
                }
                if (Slots[i].GetItem() != null && Slots[i].GetItem() != itemDragged.myItem) continue;

                if (GlobalInputManager.InputMaster.Player.LeftClick.WasPerformedThisFrame())
                {
                    // Calculate the quantity that can be added to the target slot
                    int spaceAvailable = Slots[i].GetItem() == null ? itemDragged.myItem.MaxStackSize : itemDragged.myItem.MaxStackSize - Slots[i].GetItemQuantity();

                    if (spaceAvailable > 0)
                    {
                        int quantityToAdd = Mathf.Min(spaceAvailable, itemDragged.quantityHeld);
                        Slots[i].TryAddItem(itemDragged.myItem, quantityToAdd, out _);
                        itemDragged.quantityHeld -= quantityToAdd;
                    }

                    if (itemDragged.quantityHeld == 0)
                    {
                        onStopDraggingItem?.Invoke(itemDragged);
                    }
                }
                else if (GlobalInputManager.InputMaster.Player.RightClick.WasPerformedThisFrame())
                {
                    // Calculate the quantity that can be added to the target slot
                    int spaceAvailable = Slots[i].GetItem() == null ? itemDragged.myItem.MaxStackSize : itemDragged.myItem.MaxStackSize - Slots[i].GetItemQuantity();

                    if (spaceAvailable > 0)
                    {
                        Slots[i].TryAddItem(itemDragged.myItem, 1, out _);
                        itemDragged.quantityHeld -= 1;
                    }

                    if (itemDragged.quantityHeld == 0)
                    {
                        onStopDraggingItem?.Invoke(itemDragged);
                    }
                }


            }
        }
    }
    public static void SpawnItem(Item itm, int qty, Vector3 pos)
    {
        ItemWorldObject obj = Instantiate(i.itemDropPrefab,
            pos, Quaternion.identity);
        obj.Setup(itm, qty, 3.5f);
    }
    public static void DropItem(Item itm, int qty)
    {
        SpawnItem(itm, qty, (Vector3)i.player.m_lastInput * 1.3f + i.player.transform.position);
    }
    public static bool GrabItem(UI_InventorySlot slot, Item item, int quantity)
    {
        bool fits = isDraggingItem == false;
        if (fits)
        {
            UI_ItemObject obj = Instantiate(i.ui_ItemObjectPrefab, i.InventoryPanel);
            i.itemDragged = obj;
            if (obj != null)
            {
                i.itemDragged.myItem = item;
                i.itemDragged.quantityHeld = quantity;
                i.itemDragged.origin = slot;
            }
        }

        return fits;
    }
}