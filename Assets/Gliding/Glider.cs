using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GliderParameter
{
    public float max;
    public float level;
    public float min;

    public float ConvertFrom(float paramVal, GliderParameter gliderParams)
    {
        float val = 0f;

        if (paramVal >= gliderParams.level)
        {
            val = Mathf.Lerp(level, max, Mathf.InverseLerp(level, gliderParams.max, gliderParams.level));
        }
        else
        {
            val = Mathf.Lerp(min, level, Mathf.InverseLerp(gliderParams.min, gliderParams.level, paramVal));
        }

        return val;
    }
}

public class Glider : MonoBehaviour
{
    public float timeScale;
    public GliderParameter pitchAngle;
    public GliderParameter forwardVelocity;
    public GliderParameter downwardVelocity;

    private float currentPitchAngle;
    private float rollAngle;

    private float t;

    //Properties
    public Vector3 Velocity { get; private set; }
    public float Heading { get; private set; }
    //Velocity in the local frame, then get forward component
    public float Airspeed => (transform.worldToLocalMatrix * Velocity).z;

    //Public Functions
    public void Roll(float angle)
    {
    }

    public void Pitch(float angle)
    {
        currentPitchAngle += angle;
        currentPitchAngle = Mathf.Clamp(currentPitchAngle, pitchAngle.min, pitchAngle.max);
    }

    //TODO: lerp
    public void Level()
    {
    }

    private void FixedUpdate()
    {
        t = Time.fixedDeltaTime * timeScale;

        UpdateTransform();
        UpdateParameters();
    }

    private void UpdateTransform()
    {
        float y = -downwardVelocity.ConvertFrom(currentPitchAngle, pitchAngle);
        float z = forwardVelocity.ConvertFrom(currentPitchAngle, pitchAngle);

        Velocity = new Vector3(0f, y, z);

        transform.position += Velocity * Time.fixedDeltaTime;

        transform.rotation = Quaternion.Euler(new Vector3(currentPitchAngle, 0f, rollAngle));
    }

    private void UpdateParameters()
    {
        //Update paramters
        var headingVec = new Vector3(transform.position.x, 0f, transform.position.z).normalized;
        Heading = -Mathf.Rad2Deg * MathUtility.Mod(Mathf.Acos(Vector3.Dot(Vector3.forward, headingVec)), -180f, 180f);
    }
}
