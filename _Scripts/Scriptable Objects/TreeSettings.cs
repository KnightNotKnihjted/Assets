using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New TreeSettings",menuName ="Scriptables/TreeSettings")]
public class TreeSettings : ScriptableObject
{
    public Sprite sprite;
    public Color spriteColor = Color.white;

    public Vector3 relPos = Vector3.zero;
    public Vector3 scale = Vector3.one;
    public Vector3 rotation = Vector3.zero;
}
