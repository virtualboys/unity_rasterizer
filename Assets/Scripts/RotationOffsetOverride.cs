using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationOffsetOverride : OffsetOverride
{
    [SerializeField] private Vector3 _forward;

    public override float Offset
    {
        get
        {
            float angle = Vector3.Angle(transform.forward, _forward);
            float val = -1024 * ((angle - 90) / 90.0f);
            Debug.Log(val);
            return val;
        }
    }
}
