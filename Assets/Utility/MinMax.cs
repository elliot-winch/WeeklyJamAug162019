using UnityEngine;

[System.Serializable]
public struct MinMax
{
    public float min;
    public float max;

    public float GetRandomValue()
    {
        return Mathf.Lerp(min, max, Random.value);
    }
}
