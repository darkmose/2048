using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(ColorProvider2048), menuName = "Game Configs/" + nameof(ColorProvider2048))]
public class ColorProvider2048 : ScriptableObject
{
    //[System.Serializable]
    //public class ColorMap
    //{
    //    public int value;
    //    public Color color;
    //}

    //[SerializeField]
    //private List<ColorMap> baseColorMappings = new List<ColorMap>()
    //{
    //    new ColorMap { value = 2, color = new Color(0.933f, 0.894f, 0.855f) },
    //    new ColorMap { value = 4, color = new Color(0.929f, 0.878f, 0.784f) },
    //    new ColorMap { value = 8, color = new Color(0.949f, 0.694f, 0.475f) },
    //    new ColorMap { value = 16, color = new Color(0.961f, 0.584f, 0.388f) },
    //    new ColorMap { value = 32, color = new Color(0.965f, 0.486f, 0.373f) },
    //    new ColorMap { value = 64, color = new Color(0.965f, 0.369f, 0.231f) },
    //    new ColorMap { value = 128, color = new Color(0.929f, 0.812f, 0.447f) },
    //    new ColorMap { value = 256, color = new Color(0.929f, 0.800f, 0.380f) },
    //    new ColorMap { value = 512, color = new Color(0.929f, 0.784f, 0.314f) },
    //    new ColorMap { value = 1024, color = new Color(0.929f, 0.773f, 0.247f) },
    //    new ColorMap { value = 2048, color = new Color(0.929f, 0.761f, 0.180f) }
    //};

    [Header("Color Settings")]
    [SerializeField] private float baseHue = 30f; 
    [SerializeField] private float saturation = 0.8f;
    [SerializeField] private float lightness = 0.6f; 
    [SerializeField] private Color emptyCellColor = new Color(0.804f, 0.757f, 0.706f);

    public Color GetCellColor(int value)
    {
        if (value == 0) return emptyCellColor;

        float logValue = Mathf.Log(value, 2);
        float hue = baseHue + (logValue - 1) * 15f; 

        float normalizedHue = hue % 360f;

        return HSLToRGB(normalizedHue, saturation, lightness);
    }

    private Color HSLToRGB(float h, float s, float l)
    {
        h /= 360f;

        float r, g, b;

        if (s == 0f)
        {
            r = g = b = l;
        }
        else
        {
            float HueToRGB(float p, float q, float t)
            {
                if (t < 0f) t += 1f;
                if (t > 1f) t -= 1f;
                if (t < 1f / 6f) return p + (q - p) * 6f * t;
                if (t < 1f / 2f) return q;
                if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6f;
                return p;
            }

            float q = l < 0.5f ? l * (1f + s) : l + s - l * s;
            float p = 2f * l - q;

            r = HueToRGB(p, q, h + 1f / 3f);
            g = HueToRGB(p, q, h);
            b = HueToRGB(p, q, h - 1f / 3f);
        }

        return new Color(r, g, b);
    }

    public Color GetTextColor(int value)
    {
        if (value == 0) return Color.clear;

        Color cellColor = GetCellColor(value);
        float brightness = (cellColor.r * 299 + cellColor.g * 587 + cellColor.b * 114) / 1000;
        return brightness > 0.5f ? new Color(0.467f, 0.431f, 0.396f) : Color.white;
    }
}