using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlidingBody : MonoBehaviour
{
    public float rollSensitivity;
    public float pitchSensitivity;
    public float yawSensitivity;
    public float gravityAcceleration;

    public float mass;
    public float gravityAcceleartion;
    public float dragConstant;
    public float liftConstant;

    [SerializeField] Vector3 currentAcceleration;
    [SerializeField] Vector3 currentVelocity;

    public float CurrentHeight => transform.position.y;

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        UserControlUpdate();

        var frameAcceleration = new Vector3();
        frameAcceleration += CalculateGravity();
        frameAcceleration += CalculateLift();
        frameAcceleration += CalculateDrag();

        currentAcceleration = frameAcceleration;

        currentVelocity += (frameAcceleration * Time.fixedDeltaTime);

        transform.position += currentVelocity * Time.fixedDeltaTime;
    }

    private void UserControlUpdate()
    {
        float rollInput = Input.GetAxis("Horizontal");
        float pitchInput = Input.GetAxis("Vertical");

        transform.Rotate(Vector3.forward, -rollInput * rollSensitivity);
        transform.Rotate(Vector3.right, -pitchInput * pitchSensitivity);
    }

    private Vector3 CalculateGravity()
    {
        return Vector3.down * gravityAcceleration;
    }

    private Vector3 CalculateLift()
    {
        Vector3 horizontal = Vector3.ProjectOnPlane(transform.forward, Vector3.up);

        float angleOfAttack = Vector3.SignedAngle(transform.forward, horizontal, transform.right);

        return liftConstant * transform.up * Mathf.Cos(angleOfAttack * Mathf.Deg2Rad);
    }

    private Vector3 CalculateDrag()
    {
        return -transform.forward * dragConstant;
    }
}
