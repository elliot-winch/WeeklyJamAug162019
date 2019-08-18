using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class NavigablePoint : INavigableTile
{
    public bool IsUnlocked { get; set; }
    public float MoveIntoCost { get; set; }
    public Vector3 Position { get; set; }
    public IEnumerable<INavigableTile> Neighbours { get; set; }

    public Vector2Int GridPosition;
    public float DistanceFromCenter => Mathf.Sqrt(Position.x * Position.x + Position.z * Position.z);

    public void SetHeight(float y)
    {
        Position = new Vector3()
        {
            x = Position.x,
            z = Position.z,
            y = y
        };
    }
}

public abstract class IslandGenerator : MonoBehaviour
{
    public enum NoiseGenerationType
    {
        Smooth,
        Ridged
    }

    public float diameter = 1f;
    public int resolution = 100;

    public float CubeSize => diameter / resolution;
    public float Radius => diameter / 2;

    protected Dictionary<Vector2Int, NavigablePoint> pointMap; //for macro operations, like smoothing

    //Temp
    private void Start()
    {
        Create();
    }

    public abstract void Create();

    protected virtual List<NavigablePoint> CreateCircle()
    {
        var navPoints = new List<NavigablePoint>();
        pointMap = new Dictionary<Vector2Int, NavigablePoint>();

        for (int i = -resolution; i <= resolution; i++)
        {
            for (int j = -resolution; j <= resolution; j++)
            {
                float x = i * CubeSize;
                float z = j * CubeSize;

                //Inside the circle
                if (IsValidPoint(x, z) == false)
                {
                    continue;
                }

                var point = new NavigablePoint()
                {
                    Position = new Vector3(x, 0f, z),
                    IsUnlocked = true,
                    GridPosition = new Vector2Int(i, j)
                };

                navPoints.Add(point);

                pointMap[new Vector2Int(i, j)] = point;
            }
        }

        var neighbours = new List<Vector2Int>()
        {
            new Vector2Int(0,  1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
            new Vector2Int( 1, 0),
        };

        foreach (var p in navPoints)
        {
            p.Neighbours = navPoints.Where(q =>
            {
                foreach (var n in neighbours)
                {
                    if (q.GridPosition + n == p.GridPosition)
                    {
                        return true;
                    }
                }

                return false;
            });
        }

        return navPoints;
    }

    protected bool IsValidPoint(float x, float z)
    {
        return x * x + z * z < (Radius * Radius);
    }

    //Operates in place
    protected void AddPerlinNoise(List<NavigablePoint> points, float octave, float scale, NoiseGenerationType genType = NoiseGenerationType.Smooth, Vector2? seed = null)
    {
        if(seed.HasValue == false)
        {
            seed = new Vector2(Random.Range(-1000f, 1000f), Random.Range(-1000f, 1000f));
        }

        points.ForEach(p =>
        {
            float noise = Mathf.PerlinNoise((p.Position.x * octave) + seed.Value.x, (p.Position.z * octave) + seed.Value.y) * scale;

            if(genType == NoiseGenerationType.Ridged)
            {
                //Sink so midpoint is zero
                noise -= scale / 2;

                noise = -Mathf.Abs(noise);

                noise += scale / 2;
            }

            p.Position = new Vector3()
            {
                x = p.Position.x,
                y = p.Position.y + noise,
                z = p.Position.z
            };
        });
    }

    //Operates in place
    protected void TaperPointsFromCenter(List<NavigablePoint> points)
    {
        points.ForEach(p =>
        {
            p.Position = new Vector3(p.Position.x, (Radius - p.DistanceFromCenter) * p.Position.y, p.Position.z);
        });
    }
}
