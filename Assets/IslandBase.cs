using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class IslandBase : IslandGenerator
{
    public GameObject cubePrefab;
    public float startingOctave = 1f;
    public float startingResolution = 2f;
    public float octaveScalar = 0.2f;
    public float resolutionScalar = 0.5f;
    public int numberOfIterations = 3;

    public override void Create()
    {
        var points = CreateCircle();

        float octave = startingOctave;
        float res = startingResolution;

        for(int i = 0; i < numberOfIterations; i++)
        {
            AddPerlinNoise(points, octave, res);

            octave /= octaveScalar;
            res *= resolutionScalar;
        }

        TaperPointsFromCenter(points);

        CreateCubesForPoints(points);
    }

    private void CreateCubesForPoints(List<NavigablePoint> points)
    {
        float maxY = points.Max(p => p.Position.y);

        foreach (var p in points)
        {
            GameObject cube = Instantiate(cubePrefab);

            cube.transform.position = new Vector3(p.Position.x, (-p.Position.y / 2f), p.Position.z);
            cube.transform.localScale = new Vector3(CubeSize, p.Position.y, CubeSize); //todo only make cube as big as it needs to be to occupy gaps

            float shade = Mathf.InverseLerp(0f, maxY, p.Position.y);
            cube.GetComponent<MeshRenderer>().material.color = new Color(shade, shade, shade);
        }
    }
}
