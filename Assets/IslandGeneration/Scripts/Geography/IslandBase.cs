using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class IslandBase : IslandGenerator
{
    [SerializeField]
    public PerlinNoiseIterationArgs noiseArgs;
    /*
    public float startingOctave = 1f;
    public float startingResolution = 2f;
    public float octaveScalar = 0.2f;
    public float resolutionScalar = 0.5f;
    public int numberOfIterations = 3;
    */

    public override void Create()
    {
        var points = CreateCircle();

        AddPerlinNoise(points, noiseArgs);

        TaperPointsFromCenter(points);

        CreateCubesForPoints(points);
    }

    protected override void OnCreateCubeForPoint(GameObject cube, NavigablePoint point)
    {
        base.OnCreateCubeForPoint(cube, point);

        cube.transform.position = new Vector3(point.Position.x, (-point.Position.y / 2f), point.Position.z);
        cube.transform.localScale = new Vector3(CubeSize, point.Position.y, CubeSize); //todo only make cube as big as it needs to be to occupy gaps
    }
}
