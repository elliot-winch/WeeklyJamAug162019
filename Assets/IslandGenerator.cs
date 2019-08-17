using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public abstract class IslandGenerator : MonoBehaviour
{
    public float diameter = 1f;
    public int resolution = 100;

    public float CubeSize => diameter / resolution;
    public float Radius => diameter / 2;

    //Temp
    private void Start()
    {
        Create();
    }

    public abstract void Create();

    protected List<Vector3> CreateCircle()
    {
        List<Vector3> points = new List<Vector3>();

        for (int i = -resolution; i <= resolution; i++)
        {
            for (int j = -resolution; j <= resolution; j++)
            {
                float x = i * CubeSize;
                float z = j * CubeSize;

                //Inside the circle
                if (x * x + z * z >= Radius)
                {
                    continue;
                }

                points.Add(new Vector3(x, 0f, z));
            }
        }

        return points;
    }

    protected List<Vector3> AddPerlinNoise(List<Vector3> points, float octave, float scale, Vector2 seed)
    {
        return points.Select(p =>
        {
            return new Vector3()
            {
                x = p.x,
                y = p.y + Mathf.PerlinNoise((p.x * octave) + seed.x, (p.z * octave) + seed.y) * scale,
                z = p.z
            };
        }).ToList();
    }

    protected List<Vector3> TaperPointsFromCenter(List<Vector3> points)
    {
        return points.Select(p => {
            return new Vector3(p.x, (Radius - (p.x * p.x + p.z * p.z)) * p.y, p.z);
        }).ToList();
    }
}
