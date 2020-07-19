
using UnityEngine;

public class AnalogInputManager : MonoBehaviour
{
    // Analog Jack Arrangement:
    // A0 Vert 1
    // A1 Rast 1
    // A2 Frag 1
    // A3 Frag 2
    // A4 Clear
    // A5 Select (blend between different shader values)

    // Values sent should be an int from -1024 to 1024
    private const int VERT_JACK = 0;
    private const int RAST_JACK = 1;
    private const int FRAG1_JACK = 2;
    private const int FRAG2_JACK = 3;
    private const int CLEAR_JACK = 4;
    private const int SELECT_JACK = 5;

    [SerializeField] private ShaderOffsetMap _clearOffset;
    [SerializeField] private ShaderOffsetMap _selectOffset;
    [SerializeField] private ShaderOffsetMap[] _vertexOffsets;
    [SerializeField] private ShaderOffsetMap[] _rasterizerOffsets;
    [SerializeField] private ShaderOffsetMap[] _fragmentOffsets;

    private int[] _jackVals;
    private float[] _transformedVals;

    private void Start()
    {
        _jackVals = new int[6];
        _transformedVals = new float[6];
    }

    public void SetJackVal(int jackInd, int val)
    {
        _jackVals[jackInd] = val;
    }

    private float TransformVal(int val, ShaderOffsetMap map, float scale)
    {
        float f = map.Amplitude * scale * (val / 1024.0f);
        if(map.Center == ShaderOffsetMap.CenterMode.One)
        {
            f += 1.0f;
        }
        else if(map.Center == ShaderOffsetMap.CenterMode.ZeroAbs)
        {
            f = Mathf.Abs(f);
        }
        else if(map.Center == ShaderOffsetMap.CenterMode.OneMinusZeroAbs)
        {

            f = 1.0f - Mathf.Abs(f);
        }
        return f;
    }

    private void SetScaledOffsets(int jackNum, ComputeShader shader, ShaderOffsetMap[] offsetMaps)
    {
        float select = TransformVal(_jackVals[SELECT_JACK], _selectOffset, 1);
        float val1 = TransformVal(_jackVals[jackNum], offsetMaps[0], select);
        float val2 = TransformVal(_jackVals[jackNum], offsetMaps[1], 1.0f - select);
        _transformedVals[jackNum] = val1;
        shader.SetFloat(offsetMaps[0].PropName, val1);
        shader.SetFloat(offsetMaps[1].PropName, val2);
    }

    public void SetClearOffset(ComputeShader shader)
    {
        float val = TransformVal(_jackVals[CLEAR_JACK], _clearOffset, 1);
        _transformedVals[CLEAR_JACK] = val;
        shader.SetFloat(_clearOffset.PropName, val);
    }

    public void SetVertexOffsets(ComputeShader shader)
    {
        SetScaledOffsets(VERT_JACK, shader, _vertexOffsets);
    }

    public void SetRasterizerOffsets(ComputeShader shader)
    {
        SetScaledOffsets(RAST_JACK, shader, _rasterizerOffsets);
    }

    public void SetFragmentOffsets(ComputeShader shader)
    {
        SetScaledOffsets(FRAG1_JACK, shader, _fragmentOffsets);
    }
}

