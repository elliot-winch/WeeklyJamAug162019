using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GliderControls : MonoBehaviour
{
    public Glider glider;
    public float rollSensitivity;
    public float pitchSensitivity;

    public bool invertPitch;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        glider.Roll(rollSensitivity * Input.GetAxis("Mouse X"));

        float angle = pitchSensitivity * Input.GetAxis("Mouse Y");

        if (invertPitch)
        {
            angle *= -1;
        }

        glider.Pitch(angle);
    }
}
