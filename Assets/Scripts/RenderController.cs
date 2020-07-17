using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class RenderController : MonoBehaviour
{
    public static RenderController Instance { get { return _instance; } }
    private static RenderController _instance;

    [SerializeField] private RasterizerController _rasterizer;
    [SerializeField] private Camera _gameCamera;
    [SerializeField] private MeshRenderer _screenMesh;

    [SerializeField] private Color _clearColor;

    private Camera _screenCamera;
    private List<MeshManager> _meshes;

    private void Awake()
    {
        if(_instance != null)
        {
            Debug.LogError("Cannot have more than one RenderController!");
            GameObject.Destroy(gameObject);
            return;
        }

        _instance = this;
    }

    private void Start()
    {
        _screenCamera = GetComponent<Camera>();
        _rasterizer.Init();
        _screenMesh.material.mainTexture = _rasterizer.ScreenTex;
        _meshes = new List<MeshManager>();
    }

    public void MarkForRender(MeshManager mesh)
    {
        if(!_meshes.Contains(mesh))
        {
            _meshes.Add(mesh);
        }
    }

    private void OnPreRender()
    {
        _rasterizer.BeginFrame(_gameCamera, _clearColor);
        foreach(var mesh in _meshes)
        {
            _rasterizer.DrawModel(mesh);
        }
        _rasterizer.EndFrame(_gameCamera);

        _meshes.Clear();
    }
}
