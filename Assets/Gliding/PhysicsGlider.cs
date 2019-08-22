using UnityEngine;

[System.Serializable]
public class PhysicsGliderAxis
{
    public float userSensitivity;
    public float physicsSensitivity;
    public float velocityScale;
    public MinMax angleVelocityBounds; //in radians
    public MinMax angleBounds; //in radians
    public MinMax velocityBounds; //in m/s

    //Variables for calculation
    public float Angle; //in radians and anti-clockwise
    public float Velocity;

    public virtual void Update(float userInput, float time) { }
}

[System.Serializable]
public class PhysicsGliderAxisForward : PhysicsGliderAxis
{
    public float glideRatio;
    public float dragFactor;

    public float ForwardVelocity => Velocity * Mathf.Cos(Angle);

    public override void Update(float userInput, float time)
    {
        base.Update(userInput, time);
        
        float acc = -Mathf.Sin(Angle) - dragFactor * (Velocity * Velocity / glideRatio);

        float userAngVel = userInput * userSensitivity;

        float physicsAngVel = (1 - Mathf.Abs(userInput)) * (-Mathf.Cos(Angle) / Velocity + Velocity) * physicsSensitivity;
        float angVel = angleVelocityBounds.Clamp((userAngVel + physicsAngVel));

        Angle = angleBounds.Clamp(Angle + (angVel * time));
        Velocity = velocityBounds.Clamp(Velocity + (acc * time));
    }
}

public class PhysicsGlider : MonoBehaviour
{
    //Insepctor Variables
    public float k; //0.5 * air density * co lift * surface of wings / mass -> constant
    public float timeScale;
    public float positionScale;

    public PhysicsGliderAxisForward yz;
    public float horizontalUserSensitivty;
    public float bankingFactor;

    //Variables, refreshed frame-to-frame
    private float t;
    private float rollAngle;

    //Properties
    public Vector3 Velocity { get; private set; }
    public float Heading { get; private set; }
    //Velocity in the local frame, then get forward component
    public float Airspeed => (transform.worldToLocalMatrix * Velocity).z;

    private void FixedUpdate()
    {
        t = Time.fixedDeltaTime * timeScale;

        yz.Update(Input.GetAxis("Vertical"), t);
        UpdateTransformYZ();

        UpdateX(Input.GetAxis("Horizontal"), t);

        transform.rotation = Quaternion.Euler(new Vector3(CalcToTransformAngle(yz.Angle), 0f, CalcToTransformAngle(rollAngle)));

        UpdateParameters();
    }
    

     private void UpdateX(float userInput, float time)
     {
        float userAngVel = userInput * horizontalUserSensitivty;

        rollAngle += userAngVel;

        float angularVel = k * Mathf.Sin(yz.Angle) * Mathf.Sin(rollAngle) * yz.ForwardVelocity;

        if (angularVel == 0f)
        {
            return;
        }

        float angle = time * angularVel * bankingFactor; //angle of banking circle
        float radius = yz.ForwardVelocity / angularVel;

        transform.position += new Vector3()
        {
            x = -Mathf.Abs(radius) * Mathf.Sin(angle) * positionScale,
        };
     }

    private void UpdateTransformYZ()
    {
        Velocity = new Vector3()
        {
            y = yz.Velocity * Mathf.Sin(yz.Angle),
            z = yz.ForwardVelocity
        };

        transform.position += Velocity * positionScale * t;
    }

    private void UpdateParameters()
    {
        //Update paramters
        var headingVec = new Vector3(transform.position.x, 0f, transform.position.z).normalized;
        Heading = -Mathf.Rad2Deg * MathUtility.Mod(Mathf.Acos(Vector3.Dot(Vector3.forward, headingVec)), -180f, 180f);
    }

    private float CalcToTransformAngle(float calcAngle)
    {
        return -Mathf.Rad2Deg * calcAngle;
    }
}
