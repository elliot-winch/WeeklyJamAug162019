using System;
using UnityEngine;
using UnityEngine.UI;

public class RadialSlider : MonoBehaviour
{
    [SerializeField] private float min = 0f;
    [SerializeField] private float max = 10f;

    [SerializeField] private Image image;
    [SerializeField] private Text text;

    public void SetValue(float value)
    {
        image.fillAmount = Mathf.InverseLerp(min, max, value);

        if (text != null)
        {
            text.text = TruncateToSignificantDigits(value, 1).ToString();
        }
    }

    private double TruncateToSignificantDigits(double d, int digits)
    {
        if (d == 0)
        {
            return 0;
        }

        double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1 - digits);
        return scale * Math.Truncate(d / scale);
    }
}
