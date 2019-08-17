using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class IslandTop : IslandGenerator
{
    public GameObject cubePrefab;

    public override void Create()
    {
        var points = CreateCircle();

        
    }

    private void CreateCubesForPoints(List<Vector3> points)
    {
        float maxY = points.Max(p => p.y);

        foreach (var p in points)
        {
            GameObject cube = Instantiate(cubePrefab);

            cube.transform.position = new Vector3(p.x, (-p.y / 2f), p.z);
            cube.transform.localScale = new Vector3(CubeSize, p.y, CubeSize); //todo only make cube as big as it needs to be to occupy gaps

            float shade = Mathf.InverseLerp(0f, maxY, p.y);
            cube.GetComponent<MeshRenderer>().material.color = new Color(shade, shade, shade);
        }
    }
}
