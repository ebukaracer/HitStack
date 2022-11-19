using UnityEngine;
using static Racer.Utilities.SingletonPattern;

[DefaultExecutionOrder(-2)]
internal class ColorGenerator : StaticInstance<ColorGenerator>
{
    private float _hue;

    protected override void Awake()
    {
        base.Awake();

        _hue = Random.Range(0, 11) / 10.0f;
    }

    public Color GetColor(float s = .5f, float v = .7f)
    {
        return Color.HSVToRGB(_hue, s, v);
    }
}
