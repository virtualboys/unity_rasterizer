using UnityEngine;

[CreateAssetMenu(fileName = "ShaderOffsetMap", menuName = "ScriptableObjects/ShaderOffsetMap", order = 1)]
public class ShaderOffsetMap : ScriptableObject
{
    public enum CenterMode
    {
        Zero,
        ZeroAbs,
        OneMinusZeroAbs,
        One
    }

    public string PropName;
    public CenterMode Center;
    public float Amplitude;
}

