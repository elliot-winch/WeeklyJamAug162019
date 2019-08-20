using UnityEngine;

public class HUDCompass : MonoBehaviour
{
    //Ensure picture is 720pixel wide
    public void SetValue(float heading)
    {
        transform.localPosition = new Vector3(heading, 0f, 0f);
    }
}
