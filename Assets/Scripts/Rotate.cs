using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private float _period;
    [SerializeField] private Vector3 _axis;

    [SerializeField] private float _range = 360;

    private Quaternion _startRot;
    private float _rotation;
    private bool _up;

    private void Start()
    {
        if(_range == 0)
        {
            _range = 360;
        }
    }

    private void OnEnable()
    {
        _startRot = transform.rotation;
    }

    void Update()
    {
        _axis.Normalize();
        if(_range > 359)
        {
            _rotation += 360 * Time.deltaTime / _period;
        }
        else
        {
            if (_up)
            {
                _rotation += 360 * Time.deltaTime / _period;
                if(_rotation > _range)
                {
                    _up = false;
                }
            }
            else
            {
                _rotation -= 360 * Time.deltaTime / _period;
                if (_rotation < 0)
                {
                    _up = true;
                }
            }
        }

        transform.rotation = _startRot * Quaternion.AngleAxis(_rotation, _axis);
        //transform.Rotate(_axis, 360 * Time.deltaTime / _period);
        //transform.rotation *= Quaternion.AngleAxis(360 * Time.deltaTime / _period, _axis);
    }
}
