﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationOffsetController : MonoBehaviour
{
    [SerializeField] private Vector3 _axis;
    [SerializeField] private float _degreesPerSecond;
    [SerializeField] private OffsetOverride _offsetController;

    private void Start()
    {
        _axis = Vector3.Normalize(_axis);
    }

    private void Update()
    {
        if(_offsetController != null)
        {
            float val = _offsetController.Offset / 1024.0f;
            transform.RotateAroundLocal(_axis, Time.deltaTime * val * _degreesPerSecond);
        }
    }
}
