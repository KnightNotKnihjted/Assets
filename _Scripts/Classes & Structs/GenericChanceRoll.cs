using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GenericChanceRoll<T>
{
    public T prize;
    public float chance;

    public T Roll()
    {
        return (Random.Range(0f, 1f) < chance) ? prize : default;
    }
}