using UnityEngine;

public class Glider : MonoBehaviour
{
    //Insepctor Variables
    public float timeScale;
    public float maxGlideRatio;
    [SerializeField] float startingYZSpeed;

    public float liftToLateralRatio;
    public float lateralDrag;
    public float vScale;
    public float thetaScale;

    //Variables for calculation
    private float pitchAngle; //in radians and anti-clockwise
    private float rollAngle;

    //Variables, refreshed frame-to-frame
    private float vy;
    private float vx;
    private float t;

    //Properties
    public Vector3 Velocity { get; private set; }
    public float Heading { get; private set; }
    //Velocity in the local frame, then get forward component
    public float Airspeed => (transform.worldToLocalMatrix * Velocity).z;

    //Public Functions
    public void Roll(float angle)
    {
        rollAngle += angle * t;
    }

    public void Pitch(float angle)
    {
        transform.Rotate(transform.right, angle);
    }

    //TODO: lerp
    public void Level()
    {
        rollAngle -= rollAngle;
    }

    //Private functions
    private void Start()
    {
        vy = startingYZSpeed;
    }

    private void FixedUpdate()
    {
        t = Time.fixedDeltaTime * timeScale;

        UpdateXPhysics();
        UpdateYZPhysics();
        UpdateTransform();
        UpdateParameters();
    }

    private void UpdateXPhysics()
    {
        float acc = (liftToLateralRatio * maxGlideRatio * Mathf.Sin(rollAngle));
        vx += acc * t;

        float latDragVel = (lateralDrag * -Mathf.Sign(vx) * vx * vx) * t;
        vx += latDragVel;
    }

    private void UpdateYZPhysics()
    {
        float acc = -Mathf.Sin(pitchAngle) - (vy * vy / maxGlideRatio);

        float angAcc = (-Mathf.Cos(pitchAngle) / vy + vy);

        vy += acc * t;
        pitchAngle += angAcc * t;
        pitchAngle = MathUtility.IntoRange(pitchAngle, 0f, Mathf.PI * 2f);
    }

    private void UpdateTransform()
    {
        float x = vx * t;
        float y = vy * Mathf.Sin(pitchAngle) * t * vScale;
        float z = vy * Mathf.Cos(pitchAngle) * t * vScale;

        Velocity = new Vector3(x, y, z);

        transform.position += Velocity;

        transform.rotation = Quaternion.Euler(new Vector3(CalcToTransformAngle(pitchAngle), 0f, CalcToTransformAngle(rollAngle)));
    }

    private void UpdateParameters()
    {
        //Update paramters
        var headingVec = new Vector3(transform.position.x, 0f, transform.position.z).normalized;
        Heading = -Mathf.Rad2Deg * MathUtility.IntoRange(Mathf.Acos(Vector3.Dot(Vector3.forward, headingVec)), -180f, 180f);
    }

    private float CalcToTransformAngle(float calcAngle)
    {
        return -Mathf.Rad2Deg * calcAngle;
    }
}
