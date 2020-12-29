
using System;
using UnityEngine;
using static ShaderOffsetMap;

[Serializable]
public class OffsetAnimation
{
    [Space(20)]
    [Header("Offset Animation Configuration")]
    public InputDescription InputDescription;

    public bool Oscillate;
    [Range(-8000, 8000)]
    public float Value;
    public float Period;

    [Header("Debug")]
    public bool IsInDebugMode;
    public int DebugInputInd;
    public bool LogValue;

    public OffsetOverride OffsetOverride;

    public void Update()
    {
        if(Oscillate)
        {
            Value = (1024 * Mathf.Sin(2.0f * Mathf.PI * Time.time / Period));
        }
        if(Mathf.Abs(Value) > 7800)
        {
            Value = float.NaN;
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

            if(o.OffsetOverride != null)
            {
                o.Value = o.OffsetOverride.Offset;
            }

            if (o.IsInDebugMode)
            {
                _inputManager.SetJackVal(i, o.Value, o.DebugInputInd);
            }
            else
            {
                _inputManager.SetJackVal(i, o.Value);
            }

            o.InputDescription = _inputManager.GetCurrentDescription(i);

            if (o.LogValue)
            {
                Debug.Log(o.InputDescription.CurrentParameter + ": " + o.Value);
            }
        }
    }
}
