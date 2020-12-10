using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityOffsetOverride : OffsetOverride
{
    private const int NUM_FRAMES = 3;

    [SerializeField] private Vector3 _axis;
    [SerializeField] private float _sensitivity;

    private Vector3[] _lastPositions;

    public override float Offset { get { return _offset; } }
    private float _offset;

    void Start()
    {
        _lastPositions = new Vector3[NUM_FRAMES];
        for (int i = 0; i < NUM_FRAMES; i++)
        {
            _lastPositions[i] = transform.position;
        }
    }
    
    void Update()
    {
        Vector3 lastPos = Vector3.zero;
        for (int i = NUM_FRAMES - 1; i > 0; i--)
        {
            _lastPositions[i] = _lastPositions[i - 1];
            lastPos += _lastPositions[i];
        }

        _lastPositions[0] = transform.position;

        lastPos += transform.position;
        lastPos /= NUM_FRAMES;

        Vector3 d = transform.position - lastPos;
        _offset = _sensitivity * 1024.0f * Vector3.Dot(d, _axis);
    }
}
