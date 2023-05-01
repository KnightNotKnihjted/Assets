using UnityEngine;
[System.Serializable]
public abstract class NoiseConfigBase : ScriptableObject
{
    public abstract int Width { get; set; }
    public abstract int Height { get; set; }
    public abstract bool Update { get; set; }
    public abstract float GetNoiseValue(int x, int y, int seed = 0);
}