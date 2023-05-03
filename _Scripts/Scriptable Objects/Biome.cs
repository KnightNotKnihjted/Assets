using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Biome", menuName = "Scriptables/Biome")]
public class Biome : ScriptableObject
{
    public TileBase landTile;
    public float minThreshold;
    public float maxThreshold;
}