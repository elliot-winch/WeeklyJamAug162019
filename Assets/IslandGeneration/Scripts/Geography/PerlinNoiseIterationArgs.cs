[System.Serializable]
public class PerlinNoiseIterationArgs
{
    public int iterations;
    public float startingOctave;
    public float octaveIterationModifier;
    public float startingScale;
    public float scaleIterationModifier;

    public override string ToString()
    {
        return string.Format("Iterations: {0}, Starting Octave: {1}, OIM: {2}, Starting Scale: {3}, SIM: {4}", iterations, startingOctave, octaveIterationModifier, startingScale, scaleIterationModifier);
    }
}