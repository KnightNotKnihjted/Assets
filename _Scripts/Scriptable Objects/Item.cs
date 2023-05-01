using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Item",menuName ="Scriptables/Item")]
public class Item : ScriptableObject
{
    private Sprite icon;
    private Sprite worldSprite;
    private int maxStackSize;
    private string itemName;
    private Vector2 tooltipOffset = Vector2.up * 12.5f;

    public void SetValue(Item storedItem)
    {
        Icon = storedItem.Icon;
        ItemName = storedItem.ItemName;
        MaxStackSize = storedItem.MaxStackSize;
        TooltipOffset = storedItem.TooltipOffset;
        WorldSprite = storedItem.WorldSprite;
    }
    public Sprite Icon { get => icon; set => icon = value; }
    public Sprite WorldSprite { get => worldSprite; set => worldSprite = value; }
    public int MaxStackSize { get => maxStackSize; set => maxStackSize = value; }
    public string ItemName { get => itemName; set => itemName = value; }
    public Vector2 TooltipOffset { get => tooltipOffset; set => tooltipOffset = value; }
}