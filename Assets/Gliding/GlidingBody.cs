using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlidingBody : MonoBehaviour
{
    [Header("Controls")]
    public float rollSensitivity;
    public float pitchSensitivity;
    public float rollGravity;
    public float pitchGravity;

    [Header("Physics")]
    public Vector3 maximumVelocity;
    public float gravityAcceleration;
    public float dragConstant;
    public float liftConstant;

    [SerializeField] Vector3 currentAcceleration;
    [SerializeField] Vector3 currentVelocity;

    public float CurrentHeight => transform.position.y;

    private void FixedUpdate()
    {
        UserControlUpdate();

        var frameAcceleration = new Vector3();
        frameAcceleration += CalculateGravity();
        frameAcceleration += CalculateLift();
        frameAcceleration += LateralDrag();
        //frameAcceleration += CalculateDrag();

        currentAcceleration = frameAcceleration;

        currentVelocity += frameAcceleration * Time.fixedDeltaTime;

        currentVelocity = Limit(currentVelocity, maximumVelocity);

        transform.position += currentVelocity * Time.fixedDeltaTime;
    }

    private Vector3 Limit(Vector3 value, Vector3 max)
    {
        return new Vector3(Limit(value.x, max.x), Limit(value.y, max.y), Limit(value.z, max.z));
    }

    private float Limit(float value, float max)
    {
        return Mathf.Sign(value) * (Mathf.Min(Mathf.Abs(value), max));
    }

    private void UserControlUpdate()
    {
        float rollInput = Input.GetAxis("Horizontal");
        float pitchInput = Input.GetAxis("Vertical");

        transform.Rotate(Vector3.forward, -rollInput * rollSensitivity);
        transform.Rotate(Vector3.right, pitchInput * pitchSensitivity);

        transform.Rotate(Vector3.forward, rollGravity * RollAngle());
        transform.Rotate(Vector3.right, pitchGravity * AngleOfAttack());
    }

    private Vector3 CalculateGravity()
    {
        return Vector3.down * gravityAcceleration;
    }

    private Vector3 CalculateLift()
    {
        return liftConstant * transform.up * Mathf.Cos(AngleOfAttack() * Mathf.Deg2Rad);
    }

    private Vector3 CalculateDrag()
    {
        return -transform.forward * dragConstant;
    }

    private Vector3 LateralDrag()
    {
        float v = currentVelocity.x;
        return transform.right * - 4f * v;
    }

    private float AngleOfAttack()
    {
        Vector3 horizontal = Vector3.ProjectOnPlane(transform.forward, Vector3.up);

        return Vector3.SignedAngle(transform.forward, horizontal, transform.right);
    }

    private float RollAngle()
    {
        Vector3 horizontal = Vector3.ProjectOnPlane(transform.right, Vector3.up);

        return Vector3.SignedAngle(transform.right, horizontal, transform.forward);
    }

    /*
    private Vector3 CalculateRollLateralForce()
    {
        Vector3 horizontal = Vector3.ProjectOnPlane(transform.right, Vector3.up);

        float rollAngle = Vector3.SignedAngle(transform.right, horizontal, transform.forward);


    }
    */
}
