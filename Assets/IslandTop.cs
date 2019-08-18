using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class IslandTop : IslandGenerator
{
    public GameObject cubePrefab;

    [Header("Geography")]
    public float octave;
    public float scale;
    public NoiseGenerationType genType;

    [Header("Sea")]
    public bool createSea;
    public GameObject seaPrefab;
    public float seaLevel = 0f;

    [Header("Bowl Edge")]
    public bool bowlEdge;
    public float bowlEdgeRadius;
    public float bowlEdgeHeight;
    public float bowlSmoothRange;

    [Header("River")]
    public int riverCount = 2;
    public float erosionFactor = 1f;
    public float riverMaxSourceDistance = 0.3f;
    public GameObject riverCubePrefab;

    [Header("Paths - TODO")]
    public GameObject pathCubePrefah;
    [Range(0f, 1f)]
    public float pathSpawnChance;
    public float pathAvoidHighgroundFactor = 10f;
    public float pathEndThreshold;

    public override void Create()
    {
        var points = CreateCircle();

        if (bowlEdge)
        {
            float bowlEdgeRange = Radius - bowlEdgeRadius;

            var edgePoints = points.Where(p => p.DistanceFromCenter >= bowlEdgeRange);

            var middlePoints = points.Except(edgePoints).ToList();

            AddPerlinNoise(middlePoints, octave, scale, genType);

            TaperPointsFromCenter(middlePoints);

            CreateBowlEdge(edgePoints.ToList());

            var middleBowlMeetingPoints = points
                .Where(p => p.DistanceFromCenter >= bowlEdgeRange - bowlSmoothRange && p.DistanceFromCenter <= bowlEdgeRange + bowlSmoothRange)
                .ToList();

            Smooth(middleBowlMeetingPoints);
        }
        else
        {
            AddPerlinNoise(points, octave, scale, genType);

            TaperPointsFromCenter(points);
        }

        if (createSea)
        {
            GameObject sea = Instantiate(seaPrefab);
            sea.transform.SetParent(transform);

            sea.transform.position = new Vector3(0f, seaLevel, 0f);
            sea.transform.localScale = new Vector3(Radius * 0.99f, sea.transform.localScale.y, Radius* 0.99f);
        }

        for(int i = 0; i < riverCount; i++)
        {
            var river = CreateRiver(points);

            foreach(var p in river)
            {

                GameObject rCube = Instantiate(riverCubePrefab);
                rCube.transform.position = p.Position + new Vector3(0f, CubeSize * 0.01f, 0f);
                rCube.transform.localScale = Vector3.one * CubeSize;
            }
        }

        /*
        if(Random.value < riverSpawnChance)
        {
            var river = CreateRiver(points);

            if (river == null)
            {
                Debug.Log("Tried creating river: failed as no path found");
            }
            else
            {
                CreateRiverCubes(river);
            }
        }
        */

        CreateCubesForPoints(points);
    }

    private void CreateCubesForPoints(List<NavigablePoint> points)
    {
        float maxY = points.Max(p => p.Position.y);

        foreach (var p in points)
        {
            GameObject cube = Instantiate(cubePrefab);

            cube.transform.position = new Vector3(p.Position.x, (p.Position.y / 2f), p.Position.z);
            cube.transform.localScale = new Vector3(CubeSize, p.Position.y, CubeSize); //todo only make cube as big as it needs to be to occupy gaps

            float shade = Mathf.InverseLerp(0f, maxY, p.Position.y);
            cube.GetComponent<MeshRenderer>().material.color = new Color(shade, shade, shade);
        }
    }

    private void CreateRiverCubes(List<Vector3> riverPoints)
    {
        foreach (var p in riverPoints)
        {
            GameObject cube = Instantiate(pathCubePrefah);

            cube.transform.position = new Vector3(p.x, p.y, p.z);
            cube.transform.localScale = new Vector3(CubeSize, CubeSize, CubeSize);
        }
    }

    /// <returns>Series of points along which the river flows</returns>
    private List<Vector3> CreatePath(List<NavigablePoint> navPoints)
    {
        NavigablePoint pointA = navPoints.Random();
        NavigablePoint pointB = navPoints.Where(p => p.Position.y < pathEndThreshold).ToList().Random();

        AStarPath path = new AStarPath(navPoints.Cast<INavigableTile>().ToList(), pointA, pointB);

        return path.ValidPath?.Select(p => p.Position).ToList();
    }

    private void CreateBowlEdge(List<NavigablePoint> edgePoints)
    {
        float heightSqrt = Mathf.Sqrt(bowlEdgeHeight);

        edgePoints.ForEach(p =>
        {
            var d = ((p.DistanceFromCenter - bowlEdgeRadius) / (Radius - bowlEdgeRadius)) * heightSqrt; 

            p.Position = new Vector3()
            {
                x = p.Position.x,
                z = p.Position.z,
                y = d * d 
            };
        });
    }

    private class Filter
    {
        int[,] filter;

        int total = -1;
        public int Total
        {
            get
            {
                if(total < 0)
                {
                    total = 0;

                    for(int i = 0; i < filter.GetLength(0); i++)
                    {
                        for(int j = 0; j < filter.GetLength(1); j++)
                        {
                            total += filter[i,j];
                        }
                    }
                }

                return total;
            }
        }

        public Vector2Int FilterSize => new Vector2Int(filter.GetLength(0), filter.GetLength(1));

        public Filter(int[,] filter)
        {
            this.filter = filter;
        }

        public float Apply(float[,] input)
        {
            float val = 0;

            for (int i = 0; i < filter.GetLength(0); i++)
            {
                for (int j = 0; j < filter.GetLength(1); j++)
                {
                    val += input[i, j] * filter[i, j];
                }
            }

            return val / Total;
        }
    }

    private void Smooth(List<NavigablePoint> pointsToSmooth)
    {
        Filter gaussian = new Filter(new int[5, 5]{
            {1,  4,  7,  4, 1 },
            {4, 16, 26, 16, 4 },
            {7, 26, 41, 26, 7 },
            {4, 16, 26, 16, 4 },
            {1,  4,  7,  4, 1 },
        });

        //values are applied at the end to make smoothing operations order-independent
        Dictionary<NavigablePoint, float> valuesToApply = new Dictionary<NavigablePoint, float>();

        Debug.Log("Smoothing points. Count: " + pointsToSmooth.Count);

        foreach(var p in pointsToSmooth)
        {
            float[,] surroundingValues = new float[gaussian.FilterSize.x, gaussian.FilterSize.y];

            for (int i = 0; i < gaussian.FilterSize.x; i++)
            {
                for (int j = 0; j < gaussian.FilterSize.y; j++)
                {
                    var gridPos = p.GridPosition - new Vector2Int(i - gaussian.FilterSize.x / 2, j - gaussian.FilterSize.y / 2);

                    if(pointMap.ContainsKey(gridPos) == false)
                    {
                        //TODO: better border policy
                        surroundingValues[i, j] = 0f;
                    }
                    else
                    {
                        surroundingValues[i, j] = pointMap[gridPos].Position.y;
                    }
                }
            }

            valuesToApply[p] = gaussian.Apply(surroundingValues);
        }

        foreach(var v in valuesToApply)
        {
            v.Key.Position = new Vector3()
            {
                x = v.Key.Position.x,
                z = v.Key.Position.z,
                y = v.Value
            };
        }
    }

    private List<NavigablePoint> CreateRiver(List<NavigablePoint> points)
    {
        var source = points
            .Where(x => x.Position.y > seaLevel && x.DistanceFromCenter <= riverMaxSourceDistance)
            .ToList()
            .Random();

        List<NavigablePoint> riverPoints = new List<NavigablePoint>();

        NavigablePoint current = source;

        //Gradient descent - either reach sea level or local minimum
        while(current.Position.y > seaLevel)
        {
            riverPoints.Add(current);

            //Get lowest neighbour
            NavigablePoint next = current.Neighbours.OrderBy(n => n.Position.y).First() as NavigablePoint;

            //River reached local minimum (create lake?)
            if(next.Position.y > current.Position.y)
            {
                break;
            }

            current = next;
        }

        for(int i = 0; i < riverPoints.Count; i++)
        {
            var p = riverPoints[i];

            //More erosion further down river
            p.SetHeight(p.Position.y - CubeSize * erosionFactor * (i / riverPoints.Count));
        }

        Smooth(riverPoints);

        return riverPoints;
    }
}
