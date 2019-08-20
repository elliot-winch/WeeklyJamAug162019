using UnityEngine;

public class GliderControls : MonoBehaviour
{
    public Glider glider;
    public float rollSensitivity;
    public float pitchSensitivity;

    public bool invertPitch;

    public KeyCode level;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        UpdateRoll();
        UpdatePitch();

        if (Input.GetKeyDown(level))
        {
            glider.Level();
        }
    }

    private void UpdateRoll()
    {
        glider.Roll(rollSensitivity * Input.GetAxis("Horizontal"));
    }

    private void UpdatePitch()
    {
        float angle = pitchSensitivity * Input.GetAxis("Vertical");

        if (invertPitch)
        {
            angle *= -1;
        }

        glider.Pitch(angle);
    }
}
