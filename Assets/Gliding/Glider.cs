using UnityEngine;

public class Glider : MonoBehaviour
{
    /*
    public float mass;
    public float airForcesFactor; //surface area of wings * density of air * 0.5
    public float liftCoefficient;
    public float dragCoefficient;
    public float gravityAcceleation;
    */

    //Insepctor Variables
    public float timeScale;
    public float maxGlideRatio;
    [SerializeField] float startingYZSpeed;

    public float liftToLateralRatio;
    public float lateralDrag;

    //Variables for calculation
    private float pitchAngle; //in radians and anti-clockwise
    private float rollAngle;

    //Variables, refreshed frame-to-frame
    private float R;
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
        pitchAngle += angle * t;
    }

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
        R = maxGlideRatio * Mathf.Cos(rollAngle);

        float acc = (liftToLateralRatio * maxGlideRatio * Mathf.Sin(rollAngle));
        vx += acc * t;

        float latDragVel = (lateralDrag * -Mathf.Sign(vx) * vx * vx) * t;
        vx += latDragVel;
    }

    private void UpdateYZPhysics()
    {
        float acc = -Mathf.Sin(pitchAngle) - (vy * vy / R);

        float angAcc = -Mathf.Cos(pitchAngle) / vy + vy;

        vy += acc * t;
        pitchAngle += angAcc * t;
        pitchAngle = MathUtility.IntoRange(pitchAngle, 0f, Mathf.PI * 2f);
    }

    private void UpdateTransform()
    {
        float x = vx * t;
        float y = vy * Mathf.Sin(pitchAngle) * t;
        float z = vy * Mathf.Cos(pitchAngle) * t;

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

    /*
    private void FixedUpdate()
    {
        float V = currentVelocity.z;
        float t = Time.fixedDeltaTime;

        forwardAcceleration = ((-mass * gravityAcceleation * Mathf.Sin(angleOfAttack)) - Drag(V)) / mass;

        angularAccRadians = ((-mass * gravityAcceleation * Mathf.Cos(angleOfAttack)) + Lift(V, angleOfAttack * Mathf.Rad2Deg)) / (mass * V);

        currentAcceleration = forwardAcceleration * transform.forward;
        currentVelocity += currentAcceleration * t;
        glideRatio = currentVelocity.z / currentVelocity.y;

        transform.position += currentVelocity * t;

        this.angleOfAttack = Normalise(angleOfAttack + (angularAccRadians * Time.fixedDeltaTime), 0f, 360f);

        transform.rotation = Quaternion.Euler(new Vector3(TransformToCalcAngle(this.angleOfAttack), 0f, 0f));
    }

    private float Lift(float forwardVelocity, float degAngleOfAttack)
    {
        return (Mathf.Max(0, LiftCoefficient(degAngleOfAttack))) * airForcesFactor * forwardVelocity * forwardVelocity;
    }

    private float LiftCoefficient(float degAngleOfAttack)
    {
        return 0.6394558f + 0.1117347f * degAngleOfAttack - 0.003231293f * degAngleOfAttack * degAngleOfAttack;
    }

    private float Drag(float forwardVelocity)
    {
        return dragCoefficient * airForcesFactor * forwardVelocity * forwardVelocity;
    }

    private float GetTransformAngle()
    {
        Vector3 horizontal = Vector3.ProjectOnPlane(transform.forward, Vector3.up);

        return Vector3.SignedAngle(transform.forward, horizontal, -transform.right);
    }

    private float TransformToCalcAngle(float transformAngle)
    {
        return -Normalise(Mathf.Deg2Rad * transformAngle, 0f, Mathf.PI * 2f);
    }
    */

    private float CalcToTransformAngle(float calcAngle)
    {
        return -Mathf.Rad2Deg * calcAngle;
    }
}
