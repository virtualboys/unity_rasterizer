
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
    public bool Random;
    [Range(-2000, 2000)]
    public float Value;
    public float Period;

    public bool SelectOverride;
    [Range(-1024, 1024)]
    public float Select;

    public OSC OSCSender;
    public float OSCMaxValue = 1;

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
    [SerializeField] private OffsetAnimation _selectOscillation;

    private void Update()
    {
        UpdateOscillation(_selectOscillation, 5);
        for(int i = 0; i < _jackOscillationPeriods.Length; i++)
        {
            var o = _jackOscillationPeriods[i];
            UpdateOscillation(o, i);
        }
    }

    private void UpdateOscillation(OffsetAnimation o, int jackInd)
    {
        o.Update();

        if (o.OffsetOverride != null)
        {
            o.Value = o.OffsetOverride.Offset;
        }

        if (o.SelectOverride)
        {
            _inputManager.SetJackVal(jackInd, o.Value, o.Select);
        }
        else
        {
            _inputManager.SetJackVal(jackInd, o.Value, _selectOscillation.Value);
        }

        o.InputDescription = _inputManager.GetCurrentDescription(jackInd);

        if (o.OSCSender != null)
        {
            OscMessage message = new OscMessage();
            message.address = o.InputDescription.CurrentParameter;
            float value = o.OSCMaxValue * (.5f + o.Value / 2048);
            message.values.Add(value);
            o.OSCSender.Send(message);
        }
    }
}
