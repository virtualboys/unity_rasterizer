
using System;
using UnityEngine;
using static ShaderOffsetMap;

[Serializable]
public class InputDescription
{
    public string Name;
    public string CurrentParameter;
    public CenterMode CenterMode;

    public InputDescription(string name, string parameter, CenterMode centerMode)
    {
        Name = name;
        CurrentParameter = parameter;
        CenterMode = centerMode;
    }
}

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
    private const int CAMERA_JACK = 4;
    private const int SELECT_JACK = 5;
    
    [SerializeField] private Camera _gameCam;

    [SerializeField] private ShaderOffsetMap[] _vertexOffsets;
    [SerializeField] private ShaderOffsetMap[] _rasterizerOffsets;
    [SerializeField] private ShaderOffsetMap[] _fragmentOffsets;
    [SerializeField] private ShaderOffsetMap[] _postProcessOffsets;
    [SerializeField] private ShaderOffsetMap _cameraScaleOffset;
    [SerializeField] private ShaderOffsetMap _selectOffset;

    private float[] _jackVals;
    private float[][] _transformedVals;

    private float _camBaseFOV;

    private void Start()
    {
        _jackVals = new float[6];
        _transformedVals = new float[6][];
        _transformedVals[VERT_JACK] = new float[_vertexOffsets.Length];
        _transformedVals[RAST_JACK] = new float[_rasterizerOffsets.Length];
        _transformedVals[FRAG_JACK] = new float[_fragmentOffsets.Length];
        _transformedVals[POST_JACK] = new float[_postProcessOffsets.Length];
        _transformedVals[CAMERA_JACK] = new float[1];
        _transformedVals[SELECT_JACK] = new float[1];

        _camBaseFOV = _gameCam.fieldOfView;
    }

    public void SetJackVal(int jackInd, float val)
    {
        _jackVals[jackInd] = val;
    }

    public InputDescription GetCurrentDescription(int jackInd)
    {
        ShaderOffsetMap[] maps;
        string name;
        switch (jackInd)
        {
            case VERT_JACK:
                name = "Vertex";
                maps = _vertexOffsets;
                break;
            case RAST_JACK:
                name = "Rasterizer";
                maps = _rasterizerOffsets;
                break;
            case FRAG_JACK:
                name = "Fragment";
                maps = _fragmentOffsets;
                break;
            case POST_JACK:
                name = "Post Processing";
                maps = _postProcessOffsets;
                break;
            case CAMERA_JACK:
                name = "Rasterizer Scale";
                maps = new ShaderOffsetMap[] { _cameraScaleOffset };
                break;
            case SELECT_JACK:
                name = "Select";
                maps = new ShaderOffsetMap[] { _selectOffset };
                break;
            default:
                name = "null";
                maps = null;
                break;
        }

        int offsetInd = (int)(GetSelectVal() * (maps.Length - 1) + (.5f / maps.Length));
        Debug.Log("Select val: " + GetSelectVal() + " offsetInd: " + offsetInd + " maps len: " + maps.Length + " jack ind: " + jackInd);
        return new InputDescription(name, maps[offsetInd].Description, maps[offsetInd].Center);
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
        float select = GetSelectVal();
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
            shader.SetFloat(offsetMaps[i].PropName, val);

            _transformedVals[jackNum][i] = val;
        }
    }

    private float GetSelectVal()
    {
        float selectVal = TransformVal(_jackVals[SELECT_JACK], _selectOffset, 1);
        return Mathf.Clamp(selectVal, 0, 1);
    }

    public void SetCameraScaleOffsets()
    {
        float val = TransformVal(_jackVals[CAMERA_JACK], _cameraScaleOffset, 1);
        _gameCam.fieldOfView = _camBaseFOV + val;
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

