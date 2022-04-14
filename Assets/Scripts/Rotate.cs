using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private float _period = 15;
    [SerializeField] private Vector3 _axis = Vector3.forward;

    [SerializeField] private float _range = 360;
    [SerializeField] private bool _randomStartRot;

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
        _rotation = (_randomStartRot) ? Random.Range(0, _range) : 0;
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

        transform.rotation = Quaternion.AngleAxis(_rotation, _axis) * _startRot;
        //transform.Rotate(_axis, 360 * Time.deltaTime / _period);
        //transform.rotation *= Quaternion.AngleAxis(360 * Time.deltaTime / _period, _axis);
    }
}
