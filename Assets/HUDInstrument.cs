using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDInstrument : MonoBehaviour
{
    public Transform needle;
    public MinMax bounds;
    public Text text;

    public void SetValue(float value)
    {
        needle.rotation = Quaternion.Euler(new Vector3(0f, 0f, -360f * Mathf.InverseLerp(bounds.min, bounds.max, value)));

        if (text != null)
        {
            text.text = value.ToString("00.00");
        }
    }
}
