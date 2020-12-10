using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// randomize offset map order
// patterns with oddity?
// rim lighting

public class RasterizerController : MonoBehaviour
{
    private const string KERNEL_NAME = "CSMain";

    private const int NUM_POLYS_PER_TILE = 20;
    private const int TILE_SIZE = 4;
    private const int NUM_THREADS_PER_GROUP = 32;

    private const int WIDTH = 800;
    private const int HEIGHT = 600;

    private unsafe struct PolyList
    {
        public uint Count;
        public fixed uint Polys[NUM_POLYS_PER_TILE];
    }

    [SerializeField] private Light _light;
    [SerializeField] private AnalogInputManager _inputManager;

    [SerializeField] private bool _postProcessingEnabled;

    [SerializeField] private ComputeShader _clearShader;
    [SerializeField] private ComputeShader _vertexShader;
    [SerializeField] private ComputeShader _tilerShader;
    [SerializeField] private ComputeShader _rasterizerShader;
    [SerializeField] private ComputeShader _fragmentShader;
    [SerializeField] private ComputeShader _postProcessShader;

    public Texture ScreenTex { get { return _screen; } }
    public int Width { get { return WIDTH; } }
    public int Height { get { return HEIGHT; } }

    private int _numPixels;
    private int _numTiles;
    private int _numTilesX;
    private int _numTilesY;

    private int _clearKernel;
    private int _vertexKernel;
    private int _tilerKernel;
    private int _rasterizerKernel;
    private int _fragmentKernel;
    private int _postProcessKernel;

    private RenderTexture _screen;
    private ComputeBuffer _zBuffer;
    private ComputeBuffer _normalBuffer;
    private ComputeBuffer _uvBuffer;
    private ComputeBuffer _tilePolyListsBuffer;

    private PolyList[] _emptyPolyLists;

    private Matrix4x4 _viewMatrix;
    private Matrix4x4 _projectionMatrix;
    private Matrix4x4 _viewportMatrix;

    public void Init()
    {
        if(WIDTH % TILE_SIZE != 0 || HEIGHT % TILE_SIZE != 0)
        {
            Debug.LogError("Width & height must be a multiple of TILE_SIZE: " + TILE_SIZE);
            return;
        }
        
        Application.targetFrameRate = 60;

        _numPixels = WIDTH * HEIGHT;
        _numTilesX = WIDTH / TILE_SIZE;
        _numTilesY = HEIGHT / TILE_SIZE;
        _numTiles = _numTilesX * _numTilesY;

        _clearKernel = _clearShader.FindKernel(KERNEL_NAME);
        _vertexKernel = _vertexShader.FindKernel(KERNEL_NAME);
        _tilerKernel = _tilerShader.FindKernel(KERNEL_NAME);
        _rasterizerKernel = _rasterizerShader.FindKernel(KERNEL_NAME);
        _fragmentKernel = _fragmentShader.FindKernel(KERNEL_NAME);
        _postProcessKernel = _postProcessShader.FindKernel(KERNEL_NAME);

        _screen = new RenderTexture(WIDTH, HEIGHT, 0, RenderTextureFormat.ARGBFloat);
        _screen.enableRandomWrite = true;
        _screen.filterMode = FilterMode.Point;
        _screen.Create();

        _zBuffer = new ComputeBuffer(_numPixels, sizeof(float));
        _normalBuffer = new ComputeBuffer(_numPixels, sizeof(float) * 3);
        _uvBuffer = new ComputeBuffer(_numPixels, sizeof(float) * 2);

        _emptyPolyLists = new PolyList[_numTiles];
        _tilePolyListsBuffer = new ComputeBuffer(_numTiles, sizeof(uint) + NUM_POLYS_PER_TILE * sizeof(uint));

        _viewportMatrix = Matrix4x4.identity;
        _viewportMatrix[3, 0] = WIDTH / 2.0f;
        _viewportMatrix[3, 1] = HEIGHT / 2.0f;
        _viewportMatrix[3, 2] = 1.0f;
        _viewportMatrix[0, 0] = WIDTH / 2.0f;
        _viewportMatrix[1, 1] = HEIGHT / 2.0f;
        _viewportMatrix[2, 2] = 0;
    }

    private void OnDestroy()
    {
        _screen.Release();
        _zBuffer.Dispose();
        _normalBuffer.Dispose();
        _uvBuffer.Dispose();
        _tilePolyListsBuffer.Dispose();
    }

    public void BeginFrame(Camera camera, Color clearColor)
    {
        _inputManager.SetCameraScaleOffsets();
        _viewMatrix = camera.worldToCameraMatrix;
        _projectionMatrix = camera.projectionMatrix;

        _clearShader.SetVector("ClearColor", clearColor);
        _clearShader.SetInt("Width", WIDTH);
        _clearShader.SetInt("Height", HEIGHT);
        
        _clearShader.SetTexture(_clearKernel, "Screen", _screen);
        _clearShader.SetBuffer(_clearKernel, "ZBuffer", _zBuffer);
        _clearShader.SetBuffer(_clearKernel, "Normals", _normalBuffer);

        _clearShader.Dispatch(_clearKernel, _numPixels / NUM_THREADS_PER_GROUP, 1, 1);
    }

    public void DrawModel(MeshManager mesh)
    {
        Matrix4x4 modelMatrix = mesh.ModelMatrix;
        Matrix4x4 MVPMatrix = _projectionMatrix * _viewMatrix * modelMatrix;
        _vertexShader.SetMatrix("MVPMat", MVPMatrix);
        _vertexShader.SetMatrix("ModelMat", modelMatrix);
        
        _vertexShader.SetBuffer(_vertexKernel, "VertsIn", mesh.VertexInBuffer);
        _vertexShader.SetBuffer(_vertexKernel, "NormalsIn", mesh.NormalInBuffer);
        _vertexShader.SetBuffer(_vertexKernel, "VertsOut", mesh.VertexOutBuffer);
        _vertexShader.SetBuffer(_vertexKernel, "NormalsOut", mesh.NormalOutBuffer);

        _inputManager.SetVertexOffsets(_vertexShader);

        _vertexShader.Dispatch(_vertexKernel, mesh.NumVerts / NUM_THREADS_PER_GROUP, 1, 1);
        
        _tilePolyListsBuffer.SetData(_emptyPolyLists);
        _tilerShader.SetInt("Width", WIDTH);
        _tilerShader.SetInt("Height", HEIGHT);
        _tilerShader.SetInt("TilesX", _numTilesX);
        _tilerShader.SetInt("TilesY", _numTilesY);
        _tilerShader.SetInt("TileSize", TILE_SIZE);
        _tilerShader.SetMatrix("ViewportMat", _viewportMatrix);
        _tilerShader.SetBuffer(_tilerKernel, "VertsIn", mesh.VertexOutBuffer);
        _tilerShader.SetBuffer(_tilerKernel, "IndsIn", mesh.IndexBuffer);
        _tilerShader.SetBuffer(_tilerKernel, "TilePolyLists", _tilePolyListsBuffer);
        _tilerShader.Dispatch(_tilerKernel, mesh.NumFaces / NUM_THREADS_PER_GROUP, 1, 1);
        
        _rasterizerShader.SetInt("Width", WIDTH);
        _rasterizerShader.SetInt("Height", HEIGHT);
        _rasterizerShader.SetInt("TilesX", _numTilesX);
        _rasterizerShader.SetInt("TilesY", _numTilesY);
        _rasterizerShader.SetInt("TileSize", TILE_SIZE);

        _rasterizerShader.SetMatrix("ViewportMat", _viewportMatrix);
        _rasterizerShader.SetBuffer(_rasterizerKernel, "VertsIn", mesh.VertexOutBuffer);
        _rasterizerShader.SetBuffer(_rasterizerKernel, "NormalsIn", mesh.NormalOutBuffer);
        _rasterizerShader.SetBuffer(_rasterizerKernel, "UVsIn", mesh.UVBuffer);
        _rasterizerShader.SetBuffer(_rasterizerKernel, "IndsIn", mesh.IndexBuffer);
        _rasterizerShader.SetBuffer(_rasterizerKernel, "TilePolyLists", _tilePolyListsBuffer);
        _rasterizerShader.SetBuffer(_rasterizerKernel, "ZBuffer", _zBuffer);
        _rasterizerShader.SetBuffer(_rasterizerKernel, "NormalsOut", _normalBuffer);
        _rasterizerShader.SetBuffer(_rasterizerKernel, "UVsOut", _uvBuffer);

        _inputManager.SetRasterizerOffsets(_rasterizerShader);
        //_inputManager.SetRasterizerScaleOffset(_rasterizerShader);

        _rasterizerShader.Dispatch(_rasterizerKernel, _numTiles / NUM_THREADS_PER_GROUP, 1, 1);
    }

    public void EndFrame(Camera camera)
    {
        _fragmentShader.SetInt("Width", WIDTH);
        _fragmentShader.SetInt("Height", HEIGHT);

        _fragmentShader.SetVector("ViewDir", camera.transform.forward);
        _fragmentShader.SetVector("LightDir", _light.transform.forward);
        _fragmentShader.SetFloat("Time", Time.time);
        _fragmentShader.SetBuffer(_fragmentKernel, "ZBuffer", _zBuffer);
        _fragmentShader.SetTexture(_fragmentKernel, "Screen", _screen);
        _fragmentShader.SetBuffer(_fragmentKernel, "Normals", _normalBuffer);
        _fragmentShader.SetBuffer(_fragmentKernel, "UVs", _uvBuffer);

        _inputManager.SetFragmentOffsets(_fragmentShader);

        _fragmentShader.Dispatch(_fragmentKernel, _numPixels / NUM_THREADS_PER_GROUP, 1, 1);

        if(_postProcessingEnabled)
        {
            _postProcessShader.SetInt("Width", WIDTH);
            _postProcessShader.SetInt("Height", HEIGHT);

            _postProcessShader.SetTexture(_postProcessKernel, "Screen", _screen);

            _inputManager.SetPostProcessOffsets(_postProcessShader);

            _postProcessShader.Dispatch(_postProcessKernel, _numPixels / NUM_THREADS_PER_GROUP, 1, 1);
        }
    }
}
