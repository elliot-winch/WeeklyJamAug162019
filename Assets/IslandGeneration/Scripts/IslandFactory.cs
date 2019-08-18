using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class IslandFactory : MonoBehaviour
{
    public GameObject islandPrefab;
    public IslandTopColorScheme colorScheme;
    public MinMax startingOctave;
    public MinMax startingScale;
    public MinMax oim;
    public MinMax sim;
    public MinMax iterations; 

    private GameObject currentIsland;

    private void Start()
    {
        New();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            New();
        }
    }

    public void New()
    {
        if (currentIsland != null)
        {
            Destroy(currentIsland);
        }

        currentIsland = Instantiate(islandPrefab);

        var top = currentIsland.GetComponentInChildren<IslandTop>();

        top.createSea = Random.value > 0.5f;
        top.genType = Random.value > 0.5f ? IslandGenerator.NoiseGenerationType.Smooth : IslandGenerator.NoiseGenerationType.Ridged;

        var noiseArgs = new PerlinNoiseIterationArgs()
        {
            iterations = (int)iterations.GetRandomValue(),
            startingOctave = startingOctave.GetRandomValue(),
            startingScale = startingScale.GetRandomValue(),
            scaleIterationModifier = sim.GetRandomValue(),
            octaveIterationModifier = oim.GetRandomValue()
        };

        top.noiseArgs = noiseArgs;

        Debug.Log(top.noiseArgs);

        foreach (var g in currentIsland.GetComponentsInChildren<IslandGenerator>())
        {
            g.Create();
        }

        var structures = currentIsland.GetComponentInChildren<IslandStructureGenerator>();
        
        structures.Create(top);
    }
}
