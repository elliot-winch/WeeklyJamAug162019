using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColorBand
{
    public float distanceAboveSeaLevel;
    public Color color;
}

//TODO:
public class IslandTopColorScheme : MonoBehaviour
{
    [Header("Height Bands")]
    public float seaLevel;
    public List<ColorBand> bands;

    public void Paint(IslandTop top)
    {
        foreach(var p in top.Points)
        {
            var color = GetColor(p.Position.y);
            p.cube.GetComponent<MeshRenderer>().material.color = color;
        }
    }
    
    private Color GetColor(float height)
    {
        ColorBand cur = bands.First();

        while(height > cur.distanceAboveSeaLevel + seaLevel || cur == bands.Last())
        {
            cur = bands.Next(cur);
        }

        return cur.color;
    }

    /*
    private Color HeightFunc(float height)
    {
    }

    private Color GradFunc(float gradient)
    {
        if(gradient > sharpGradiendThreshold)
        {
            return incline;
        }
        else
        {
            return valley;
        }
    }
    */
}
