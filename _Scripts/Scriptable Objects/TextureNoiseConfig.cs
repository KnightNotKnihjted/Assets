using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TextureNoiseConfig", menuName = "Scriptables/TextureNoiseConfig")]
public class TextureNoiseConfig : NoiseConfigBase
{
    [SerializeField] private Texture2D texture;

    [Space(20)]
    [SerializeField] private bool update;
    public override int Width
    {
        get { return texture.width; }
        set { }
    }
    public override int Height
    {
        get { return texture.height; }
        set { }
    }
    public override bool Update
    {
        get { return update; }
        set { }
    }

    public override float GetNoiseValue(int x, int y, int seed = 0)
    {
        return texture.GetPixel(x,y).grayscale;
    }
}