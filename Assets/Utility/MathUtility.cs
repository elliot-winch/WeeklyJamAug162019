using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathUtility
{
    public static float Mod(float value, float min, float max)
    {
        float width = max - min;
        float offsetValue = value - min;

        return (offsetValue - (Mathf.Floor(offsetValue / width) * width)) + min;
    }
}
