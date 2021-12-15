using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private float _period;
    [SerializeField] private Vector3 _axis;

    void Update()
    {
        _axis.Normalize();
        transform.Rotate(_axis, 360 * Time.deltaTime / _period);
        //transform.rotation *= Quaternion.AngleAxis(360 * Time.deltaTime / _period, _axis);
    }
}
