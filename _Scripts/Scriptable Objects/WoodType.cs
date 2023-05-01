using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName ="New WoodType",menuName ="Scriptables/Wood Type")]
public class WoodType : Item
{
    public Vector2Int growRange = new (2, 5 + 1);
    public Vector2 generateBounds = new(0.5f, 0.6f);

    public float ratioOfTotalLandTaken = 0.1f;
    public float densityPerTileSquared = 0.25f;
    public TileBase treeTile;

    public string woodName;

    public LootTable woodLootTable;
    public TreeSettings headTreeSetting;
    public TreeSettings bodyTreeSetting;
    public TreeSettings rootTreeSetting;
}