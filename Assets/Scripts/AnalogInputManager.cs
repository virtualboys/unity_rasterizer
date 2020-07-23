
using UnityEngine;

public class AnalogInputManager : MonoBehaviour
{
    // Analog Jack Arrangement:
    // A0 Vert 
    // A1 Rast 
    // A2 Frag 
    // A3 Post 
    // A4 Clear
    // A5 Select (blend between different shader values)

    // Values sent should be an int from -1024 to 1024
    private const int VERT_JACK = 0;
    private const int RAST_JACK = 1;
    private const int FRAG_JACK = 2;
    private const int POST_JACK = 3;
    private const int CLEAR_JACK = 4;
    private const int SELECT_JACK = 5;

    [SerializeField] private ShaderOffsetMap _clearOffset;
    [SerializeField] private ShaderOffsetMap _selectOffset;
    [SerializeField] private ShaderOffsetMap[] _vertexOffsets;
    [SerializeField] private ShaderOffsetMap[] _rasterizerOffsets;
    [SerializeField] private ShaderOffsetMap[] _fragmentOffsets;
    [SerializeField] private ShaderOffsetMap[] _postProcessOffsets;

    private float[] _jackVals;
    private float[][] _transformedVals;

    private void Start()
    {
        _jackVals = new float[6];
    }

    public void SetJackVal(int jackInd, float val)
    {
        _jackVals[jackInd] = val;
    }

    private float TransformVal(float val, ShaderOffsetMap map, float scale)
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
        float tRange = 1.0f / (offsetMaps.Length - 1);
        int ind1 = (int)(select / tRange);
        for(int i = 0; i < offsetMaps.Length; i++)
        {
            float t = 0;
            if (i == ind1)
            {
                t = 1.0f - (select - (tRange * ind1)) / tRange;
            }
            else if (i == ind1 + 1)
            {
                t = (select - (tRange * ind1)) / tRange;
            }

            float val = TransformVal(_jackVals[jackNum], offsetMaps[i], t);
            shader.SetFloat(offsetMaps[i].PropName, t);
        }
    }

    public void SetClearOffset(ComputeShader shader)
    {
        float val = TransformVal(_jackVals[CLEAR_JACK], _clearOffset, 1);
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
        SetScaledOffsets(FRAG_JACK, shader, _fragmentOffsets);
    }

    public void SetPostProcessOffsets(ComputeShader shader)
    {
        SetScaledOffsets(POST_JACK, shader, _postProcessOffsets);
    }
}

