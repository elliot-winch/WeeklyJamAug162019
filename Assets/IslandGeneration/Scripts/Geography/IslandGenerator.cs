using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    X,
    Y,
    Z
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
    public GameObject cubePrefab;
    public int frameCreateLimit = 25;
    public bool heightMapShading;

    public float CubeSize => diameter / resolution;
    public float Radius => diameter / 2;

    public Dictionary<Vector2Int, NavigablePoint> PointMap => pointMap;
    public List<NavigablePoint> Points => pointMap.Values.ToList();

    protected Dictionary<Vector2Int, NavigablePoint> pointMap; //for macro operations, like smoothing

    public abstract void Create();
    protected virtual void OnCreateCubeForPoint(GameObject cube, NavigablePoint point) { }

    private static List<Vector2Int> XNeighbours = new List<Vector2Int>()
    {
        new Vector2Int(-1, 0),
        new Vector2Int( 1, 0),
    };

    private static List<Vector2Int> ZNeighbours = new List<Vector2Int>()
    {
        new Vector2Int(0,  1),
        new Vector2Int(0, -1),
    };

    protected List<NavigablePoint> CreateCircle()
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
                    GridPosition = new Vector2Int(i, j),
                };

                navPoints.Add(point);

                pointMap[new Vector2Int(i, j)] = point;
            }
        }

        foreach (var p in navPoints)
        {
            p.Neighbours = navPoints.Where(q =>
            {
                foreach (var n in XNeighbours.Union(ZNeighbours))
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

    protected void CreateCubesForPoints(List<NavigablePoint> points)
    {
        StartCoroutine(CreateCubesCoroutine(points));
    }

    protected IEnumerator CreateCubesCoroutine(List<NavigablePoint> points) {

        float maxY = points.Max(p => p.Position.y);

        int createCount = 0;

        foreach (var p in points)
        {
            GameObject cube = Instantiate(cubePrefab);

            p.cube = cube;

            OnCreateCubeForPoint(cube, p);

            cube.transform.SetParent(transform);

            if (heightMapShading)
            {
                float shade = Mathf.InverseLerp(0f, maxY, p.Position.y);
                cube.GetComponent<MeshRenderer>().material.color = new Color(shade, shade, shade);
            }

            createCount++;

            if(createCount > frameCreateLimit)
            {
                yield return null;
                createCount = 0;
            }
        }
    }

    //Operates in place
    protected void AddPerlinNoise(List<NavigablePoint> points, PerlinNoiseIterationArgs args, NoiseGenerationType genType = NoiseGenerationType.Smooth, Vector2? seed = null)
    {
        float octave = args.startingOctave;
        float res = args.startingScale;

        for (int i = 0; i < args.iterations; i++)
        {
            AddPerlinNoise(points, octave, res, genType);

            octave *= args.octaveIterationModifier;
            res *= args.scaleIterationModifier;
        }
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

    public float MaxGrad(NavigablePoint point)
    {
        return Mathf.Max(Gradient(point, Direction.X), Gradient(point, Direction.Z));
    }

    public float Gradient(NavigablePoint point, Direction d)
    {
        Vector2Int gpa = Vector2Int.zero, gpb = Vector2Int.zero;

        switch (d)
        {
            case Direction.X:
                gpa = point.GridPosition + XNeighbours[0];
                gpa = point.GridPosition + XNeighbours[1];
                break;
            case Direction.Z:
                gpa = point.GridPosition + ZNeighbours[0];
                gpa = point.GridPosition + ZNeighbours[1];
                break;
        }

        if (pointMap.ContainsKey(gpa) && pointMap.ContainsKey(gpb))
        {
            return pointMap[gpb].Position.y - pointMap[gpa].Position.y;
        }
        else
        {
            //TODO: better border policy
            return 0f;
        }
    }
}
