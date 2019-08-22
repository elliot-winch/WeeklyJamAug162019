using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IslandVillage: MonoBehaviour
{ 
    [Header("Paths")]
    public int pathClusterCount;
    public IslandPath islandPathPrefab;
    public int maxPathIterations;
    public int minIntersectionSeparation;
    [Tooltip("Measured in Cubes")]
    public MinMax pathLength;
    public GameObject pathCubePrefab;

    [Header("Buildings")]
    public List<IslandBuilding> buildingPrefabs;
    public int maxBuildingPerIteration;
    public int possibleBuildingSitesFactor;

    private Dictionary<Vector2Int, IslandStructure> structureMap = new Dictionary<Vector2Int, IslandStructure>();
    private IslandTop surface;

    public void Create(IslandTop surface)
    {
        this.surface = surface;

        List<IslandBuilding> buildings = new List<IslandBuilding>();

        //List of all paths created
        List<IslandPath> paths = new List<IslandPath>();

        //For fast location of a new potential intersection
        //Current intersections recorded to avoid overlaps / close intersections
        List<Vector2Int> intersections = new List<Vector2Int>();

        //Initial intersection point chosen randomly
        //First crossing point is not added to intersections as it is not where two roads meet, but a randomly chosen point
        Vector2Int intersectionPoint = surface.PointMap.Values.ToList().Random().GridPosition;
        //Initial direction chosen randomly
        Direction roadDir = Random.value > 0.5f ? Direction.X : Direction.Z;

        //Create paths until max iterations are no valid space for a new intersection
        for (int i = 0; i < maxPathIterations; i++)
        {
            //Create the path
            IslandPath p = Instantiate(islandPathPrefab);
            p.transform.SetParent(transform);

            p.Create(surface, pathLength, intersectionPoint, roadDir);

            //Add to list of created paths
            paths.Add(p);

            //For each tile occupied, record that the tile's position points to this path - or if it belongs to another path,
            //this is a new intersection
            p.Tiles.ForEach(x =>
            {
                if (structureMap.ContainsKey(x) == false)
                {
                    structureMap[x] = p;
                }
                else
                {
                    intersections.Add(x);
                }
            });

            //Create buildings while creating roads to crate more integrated villages
            PopulateBuildings(paths);

            //Find the next intersection from all possible roads
            //Assess every point on every path
            intersectionPoint = structureMap.Keys.Where(pathTile =>
            {
                //If the point is too close to any existing intersection, reject that point
                foreach (var intersection in intersections)
                {
                    if (Vector2Int.Distance(pathTile, intersection) < minIntersectionSeparation)
                    {
                        return false;
                    }
                }

                return true;
            })
            .ToList()
            //Choose random point from the survivors
            .Random();

            //Case where no space for a new intersection
            if (intersectionPoint == null)
            {
                break;
            }

            //Find which path the new intersection belongs to 
            var nextCrossingPath = structureMap[intersectionPoint] as IslandPath;

            //Flip direction for next path to opposite of selected path
            roadDir = (nextCrossingPath.Direction == Direction.X) ? Direction.Z : Direction.X;
        }

        //Front end
        paths.ForEach(x => CreatePathCubes(x, surface.CubeSize));
    }

    private void PopulateBuildings(List<IslandPath> paths)
    {
        int buildingCount = (int)(Random.value * maxBuildingPerIteration);

        for (int j = 0; j < buildingCount; j++)
        {
            IslandBuilding b = buildingPrefabs.Random();
            List<NavigablePoint> possiblePoints = PathAdjacentTiles(paths).Random(possibleBuildingSitesFactor);

            foreach (var pp in possiblePoints)
            {
                if (CanPlaceBuilding(b, pp.GridPosition))
                {
                    CreateBuildingObject(b, pp);
                }
            }
        }
    }

    private List<NavigablePoint> PathAdjacentTiles(List<IslandPath> paths)
    {
        var tiles = new List<Vector2Int>();

        foreach(var p in paths)
        {
            tiles.AddRange(p.AdjacentTiles());
        }

        return tiles.Where(t => surface.PointMap.ContainsKey(t)).Select(t => surface.PointMap[t]).ToList();
    }

    private bool CanPlaceBuilding(IslandBuilding building, Vector2Int coord)
    {
        //TODO: sea level
        return building.TryPlace(coord).All(x => 
            //Tiles exists
            surface.PointMap.ContainsKey(x) && 
            //No other structure overlapping
            (structureMap.ContainsKey(x) == false)
        );
    }

    private void CreateBuildingObject(IslandBuilding prefab, NavigablePoint point)
    {
        var building = Instantiate(prefab);
        building.transform.SetParent(transform);

        building.transform.localScale = building.transform.localScale * surface.CubeSize;

        building.transform.position = new Vector3(point.Position.x, Mathf.Max(surface.seaLevel, point.Position.y), point.Position.z);

        building.Place(point.GridPosition);
    }

    private void CreatePathCubes(IslandPath path, float cubeSize)
    {
        foreach (var p in path.Tiles)
        {
            GameObject cube = Instantiate(pathCubePrefab);

            cube.transform.SetParent(path.transform);

            float pathThickness = cube.transform.localScale.y * cubeSize;
            var pos = surface.PointMap[p].Position;

            cube.transform.position = new Vector3(pos.x, Mathf.Max(surface.seaLevel, pos.y) + pathThickness / 2f, pos.z);
            cube.transform.localScale = new Vector3(cubeSize, pathThickness, cubeSize);
        }
    }
}