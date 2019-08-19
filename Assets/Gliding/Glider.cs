using System.Collections;
using System.Collections.Generic;
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

    public float timeScale;
    public float maxGlideRatio;
    public float liftToLateralRatio;
    [SerializeField] float startingYZSpeed;

    private float R;
    private float pitchAngle; //in radians and anti-clockwise
    private float rollAngle;
    private float vy;
    private float vx;

    private float t;

    public void Roll(float angle)
    {
        rollAngle += angle;
    }

    public void Pitch(float angle)
    {
        pitchAngle += angle;
    }

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
    }

    private void UpdateXPhysics()
    {
        R = maxGlideRatio * Mathf.Cos(rollAngle);

        float acc = liftToLateralRatio * maxGlideRatio * Mathf.Sin(rollAngle);
        vx += acc * t;
    }

    private void UpdateYZPhysics()
    {
        float acc = -Mathf.Sin(pitchAngle) - (vy * vy / R);

        float angAcc = -Mathf.Cos(pitchAngle) / vy + vy;

        vy += acc * t;
        pitchAngle += angAcc * t;
        pitchAngle = Normalise(pitchAngle, 0f, Mathf.PI * 2f);
    }

    private void UpdateTransform()
    {
        float x = vx * t;
        float y = vy * Mathf.Sin(pitchAngle) * t;
        float z = vy * Mathf.Cos(pitchAngle) * t;

        transform.position += new Vector3(x, y, z);

        transform.rotation = Quaternion.Euler(new Vector3(CalcToTransformAngle(pitchAngle), 0f, CalcToTransformAngle(rollAngle)));
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

    private float Normalise(float value, float min, float max)
    {
        float width = max - min;
        float offsetValue = value - min;

        return (offsetValue - (Mathf.Floor(offsetValue / width) * width)) + min;
    }
}
