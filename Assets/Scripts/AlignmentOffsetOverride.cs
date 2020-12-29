using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignmentOffsetOverride : OffsetOverride
{
    [SerializeField] private Transform _bone1;
    [SerializeField] private Transform _bone2;
    [SerializeField] private VBUtils.Axis _axis;
    [SerializeField] private float _rampStart;
    [SerializeField] private bool _clamp;

    public override float Offset
    {
        get
        {
            Vector3 dir1 = VBUtils.GetDirection(_bone1, _axis);
            Vector3 dir2 = VBUtils.GetDirection(_bone2, _axis);
            float angle = Vector3.Angle(dir1, dir2);
            angle -= 90;
            if(Mathf.Abs(angle) < _rampStart || (angle < 0  && _clamp))
            {
                return 0;
            }

            angle += (angle < 0) ? _rampStart : -_rampStart;
            float val = 1024.0f * angle / (90 - _rampStart);
            return val;
        }
    }
}
