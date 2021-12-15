using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    [SerializeField] private Rigidbody[] _targets;
    [SerializeField] private float _magnitude;
    [SerializeField] private float _cycleTime;

    private float _timer;
    
    void Update()
    {
        if(_cycleTime > 0)
        {
            _timer -= Time.deltaTime;
            if (_timer < _cycleTime)
            {
                foreach (var t in _targets)
                {
                    if (t != null)
                    {
                        t.AddForce(_magnitude * transform.forward);
                    }
                }

                if (_timer < 0)
                {
                    _timer = 2 * _cycleTime;
                }
            }
        }
        else
        {
            foreach (var t in _targets)
            {
                if (t != null)
                {
                    t.AddForce(_magnitude * transform.forward);
                }
            }
        }
    }
}
