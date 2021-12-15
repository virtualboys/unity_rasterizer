
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
    public LeanTweenType EaseType = LeanTweenType.easeInOutQuad;
    [Range(-2000, 2000)]
    public float Value;
    public float Period;

    public bool SelectOverride;
    [Range(-1024, 1024)]
    public float Select;

    //public OSC OSCSender;
    //public float OSCMaxValue = 1;

    //public OffsetOverride OffsetOverride;

    private int _tweenId;

    public void Update()
    {
        if(Oscillate)
        {
            if(Random)
            {
                if(_tweenId == -1)
                {
                    RandomTween();
                }
            }
            else
            {
                if(_tweenId != -1)
                {
                    LeanTween.cancel(_tweenId);
                    _tweenId = -1;
                }
                Value = (1024 * Mathf.Sin(2.0f * Mathf.PI * Time.time / Period));
            }
        }
        else if(_tweenId != -1)
        {
            LeanTween.cancel(_tweenId);
            _tweenId = -1;
        }
        
        if(Mathf.Abs(Value) > 1800)
        {
            Value = float.NaN;
        }
    }

    private void RandomTween()
    {
        _tweenId = LeanTween.value(OffsetAnimator.AnimatorObject, UpdateValue, Value, UnityEngine.Random.Range(-1024, 1024), Period + Period * UnityEngine.Random.value).setOnComplete(RandomTween).id;
    }

    private void UpdateValue(float val)
    {
        Value = val;
    }
}

public class OffsetAnimator : MonoBehaviour
{
    public static GameObject AnimatorObject;

    [SerializeField] private AnalogInputManager _inputManager;

    [SerializeField] private OffsetAnimation[] _jackOscillationPeriods;
    [SerializeField] private OffsetAnimation _selectOscillation;

    private void Awake()
    {
        AnimatorObject = gameObject;
    }

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

        //if (o.OffsetOverride != null)
        //{
        //    o.Value = o.OffsetOverride.Offset;
        //}

        if (o.SelectOverride)
        {
            _inputManager.SetJackVal(jackInd, o.Value, o.Select);
        }
        else
        {
            _inputManager.SetJackVal(jackInd, o.Value, _selectOscillation.Value);
        }

        o.InputDescription = _inputManager.GetCurrentDescription(jackInd);

        //if (o.OSCSender != null)
        //{
        //    OscMessage message = new OscMessage();
        //    message.address = o.InputDescription.CurrentParameter;
        //    float value = o.OSCMaxValue * (.5f + o.Value / 2048);
        //    message.values.Add(value);
        //    o.OSCSender.Send(message);
        //}
    }
}
