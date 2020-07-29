
using System;
using UnityEngine;

[Serializable]
public class OffsetAnimation
{
    public bool Oscillate;
    public float Value;
    public float Period;
    
    public void Update()
    {
        if(Oscillate)
        {
            Value = (1024 * Mathf.Sin(2.0f * Mathf.PI * Time.time / Period));
        }
    }
}

public class OffsetAnimator : MonoBehaviour
{
    [SerializeField] private AnalogInputManager _inputManager;

    [SerializeField] private OffsetAnimation[] _jackOscillationPeriods;

    private void Update()
    {
        for(int i = 0; i < _jackOscillationPeriods.Length; i++)
        {
            var o = _jackOscillationPeriods[i];
            o.Update();
            _inputManager.SetJackVal(i, o.Value);
        }
    }
}
