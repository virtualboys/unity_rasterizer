
using UnityEngine;

public class OffsetAnimator : MonoBehaviour
{
    [SerializeField] private AnalogInputManager _inputManager;

    [SerializeField] private float[] _jackOscillationPeriods;

    private void Update()
    {
        for(int i = 0; i < _jackOscillationPeriods.Length; i++)
        {
            _inputManager.SetJackVal(i, GetVal(_jackOscillationPeriods[i]));
        }
    }


    private int GetVal(float period)
    {
        return (int)(512 * Mathf.Sin(2.0f * Mathf.PI * Time.time / period) + 512);
    }
}
