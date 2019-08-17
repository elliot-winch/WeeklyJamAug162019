using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class IslandBase : IslandGenerator
{
    public GameObject cubePrefab;
    public Vector2 seed;
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
            points = AddPerlinNoise(points, octave, res, new Vector2(Random.Range(-1000f, 1000f), Random.Range(-1000f, 1000f)));

            octave /= octaveScalar;
            res *= resolutionScalar;
        }

        points = TaperPointsFromCenter(points);

        CreateCubesForPoints(points);
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
