using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandForest : MonoBehaviour
{
    public float density;
    public int iterationPointCount; //number of points per iteration
    public float heightWeighting;
    public List<GameObject> treePrefabs;

    private IslandTop surface;
    private List<NavigablePoint> treePositions;

    public void Create(IslandTop surface)
    {
        this.surface = surface;
        float highestPoint = surface.HighestPoint;

        int d = (int)(surface.diameter / 2f) + 1;
        treePositions = Poisson.GeneratePoisson(d, d, density, iterationPointCount, (point) =>
        {
            return surface.PointMap.ContainsKey(point)
            ? Mathf.Abs(highestPoint - (surface.PointMap[point].Position.y * heightWeighting))
            : 0f;
        })
        .Where(p => surface.PointMap.ContainsKey(p))
        .Select(p =>  surface.PointMap[p])
        .ToList();
    }

    public void CreateTrees()
    {
        foreach(var p in treePositions)
        {
            GameObject t = Instantiate(treePrefabs.Random());
            t.transform.SetParent(transform);

            t.transform.position = p.Position;
            t.transform.localScale *= surface.CubeSize;
        }
    }

}
