using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GliderUI : MonoBehaviour
{
    public Glider glider;
    public HUDInstrument airspeedIndicator;
    public RadialSlider altimeter;
    public HUDCompass compass;

    private void Update()
    {
        airspeedIndicator.SetValue(glider.Airspeed);
        altimeter.SetValue(glider.transform.position.y); //- destination height

        compass.SetValue(glider.Heading);
    }
}
