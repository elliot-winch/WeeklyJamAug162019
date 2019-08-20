using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigablePointCube : MonoBehaviour
{
    public Transform topTransform;
    public Transform baseTransform;

    public void Scale(Vector3 scale)
    {
        baseTransform.localScale = scale;

        if(topTransform != null)
        {
            topTransform.localScale = new Vector3(scale.x, topTransform.localScale.y, scale.z);
            topTransform.localPosition = new Vector3(0f, scale.y / 2f, 0f);
        }
    }
}
