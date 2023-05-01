using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New FalloffMapConfig", menuName = "Scriptables/FalloffMapConfig")]
public class FalloffMapConfig : NoiseConfigBase
{
    [SerializeField] private float a = 3.0f;
    [SerializeField] private float b = 2.2f;

    [Space(20)]
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private bool update;
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
    public override float GetNoiseValue(int x, int y, int seed = 0)
    {
        float xCoord = (float)x / width * 2 - 1;
        float yCoord = (float)y / height * 2 - 1;

        float value = Mathf.Max(Mathf.Abs(xCoord), Mathf.Abs(yCoord));
        return Evaluate(value);
    }
    private float Evaluate(float value)
    {
        return 1 - Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
}
