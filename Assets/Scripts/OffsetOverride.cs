using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OffsetOverride : MonoBehaviour
{
    public abstract float Offset { get; }

    [SerializeField] private OSC _osc;
    [SerializeField] private string _oscName;

    protected void Update()
    {
        if(_osc != null)
        {
            var msg = new OscMessage();
            msg.address = _oscName;
            float value = .5f + Offset / 2048;
            msg.values.Add(value);
            _osc.Send(msg);
        }
    }
}
