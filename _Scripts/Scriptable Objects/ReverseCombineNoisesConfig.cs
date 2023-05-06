using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ReverseCombineNoisesConfig", menuName = "Scriptables/ReverseCombineNoisesConfig")]
public class ReverseCombineNoisesConfig : NoiseConfigBase
{
    [SerializeField] private NoiseConfigBase noise1;
    [SerializeField] private NoiseConfigBase noise2;

    [Space(20)]
    private int width;
    private int height;
    [SerializeField] private bool update;
    public override int Width
    {
        get { return Mathf.Max(noise1.Width, noise2.Width); }
        set { }
    }
    public override int Height
    {
        get { return Mathf.Max(noise1.Height, noise2.Height); }
        set { }
    }
    public override bool Update
    {
        get { return update; }
        set { }
    }

    public override float GetNoiseValue(int x, int y, int seed = 0)
    {
        return Mathf.Max(noise1.GetNoiseValue(x, y, seed), noise2.GetNoiseValue(x, y, seed));
    }
}