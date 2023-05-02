using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Recipe",menuName ="Scriptables/Recipe")]
public class Recipe : ScriptableObject
{
    public ItemQuantityComposite[] leftSide;
    public ItemQuantityComposite rightSide;
}