using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PerlinNoiseConfig", menuName = "Scriptables/PerlinNoiseConfig")]
public class PerlinNoiseConfig : NoiseConfigBase
{
    [SerializeField] private int offsetX;
    [SerializeField] private int offsetY;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float scale;

    [SerializeField] private bool update;

    public override float GetNoiseValue(int x, int y, int seed = 0)
    {
        System.Random random = new (seed);

        float offsetX = (random.Next(-100000, 100000) + seed) + (float)this.offsetX / width;
        float offsetY = (random.Next(-100000, 100000) + seed) + (float)this.offsetY / height;

        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }
    public override int Width
    {
        get { return width; }
        set { }
    }
    public override int Height
    {
        get { return height; }
        set { }
    }
    public override bool Update
    {
        get { return update; }
        set { }
    }
}