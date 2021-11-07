using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateChildren : MonoBehaviour
{
    [SerializeField] private float _period;
    [SerializeField] private Vector3 _axis;

    private Transform[] _children;
    private Quaternion[] _rots;

    private void Start()
    {
        _children = new Transform[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
        {
            _children[i] = transform.GetChild(i);
        }
        _rots = new Quaternion[_children.Length];
        for(int i = 0; i < _children.Length; i++)
        {
            _rots[i] = _children[i].rotation;
        }
    }

    void Update()
    {
        _axis.Normalize();
        transform.rotation *= Quaternion.AngleAxis(360 * Time.deltaTime / _period, _axis);

        for(int i = 0; i < _children.Length; i++)
        {
            _children[i].rotation = _rots[i];
        }
    }
}
