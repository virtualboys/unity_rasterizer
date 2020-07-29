using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshManager : MonoBehaviour
{
    private MeshFilter _meshFilter;
    private MeshRenderer _renderer;

    private int[] _inds;
    private int _numFaces;
    private Vector3[] _vertices;
    private Vector3[] _normals;
    private Vector2[] _uvs;

    public int NumVerts { get { return _vertices.Length; } }
    public int NumFaces { get { return _numFaces; } }
    public Matrix4x4 ModelMatrix { get { return _meshFilter.transform.localToWorldMatrix; } }

    public ComputeBuffer IndexBuffer { get { return _indexBuffer; } }
    private ComputeBuffer _indexBuffer;
    public ComputeBuffer UVBuffer { get { return _uvBuffer; } }
    private ComputeBuffer _uvBuffer;
    public ComputeBuffer VertexInBuffer { get { return _vertexInBuffer; } }
    private ComputeBuffer _vertexInBuffer;
    public ComputeBuffer NormalInBuffer { get { return _normalInBuffer; } }
    private ComputeBuffer _normalInBuffer;
    public ComputeBuffer VertexOutBuffer { get { return _vertexOutBuffer; } }
    private ComputeBuffer _vertexOutBuffer;
    public ComputeBuffer NormalOutBuffer { get { return _normalOutBuffer; } }
    private ComputeBuffer _normalOutBuffer;

    private void Start()
    {
        _meshFilter = GetComponentInChildren<MeshFilter>();
        _renderer = GetComponentInChildren<MeshRenderer>();
        if(_meshFilter == null)
        {
            Debug.LogError("No mesh renderer found!");
            return;
        }

        InitCPUBuffers();
        InitGPUBuffers();
    }

    private void InitCPUBuffers()
    {
        var mesh = _meshFilter.sharedMesh;
        _inds = mesh.triangles;
        _vertices = mesh.vertices;
        _normals = mesh.normals;
        _uvs = mesh.uv;

        _numFaces = _inds.Length / 3;
        
        Debug.Log(string.Format("Initialized the cpu buffers with {0} vertices, for the compute shader", _vertices.Length));
    }

    private void InitGPUBuffers()
    {
        _indexBuffer = new ComputeBuffer(_numFaces, sizeof(int) * 3);
        _indexBuffer.SetData(_inds);

        _uvBuffer = new ComputeBuffer(_uvs.Length, sizeof(float) * 2);
        _uvBuffer.SetData(_uvs);

        _vertexInBuffer = new ComputeBuffer(_vertices.Length, sizeof(float) * 3);
        _vertexInBuffer.SetData(_vertices);

        _normalInBuffer = new ComputeBuffer(_normals.Length, sizeof(float) * 3);
        _normalInBuffer.SetData(_normals);

        _vertexOutBuffer = new ComputeBuffer(_vertices.Length, sizeof(float) * 4);

        _normalOutBuffer = new ComputeBuffer(_normals.Length, sizeof(float) * 3);
    }

    private void OnDestroy()
    {
        _indexBuffer.Dispose();
        _uvBuffer.Dispose();
        _vertexInBuffer.Dispose();
        _normalInBuffer.Dispose();
        _vertexOutBuffer.Dispose();
        _normalOutBuffer.Dispose();
    }

    private void LateUpdate()
    {
        RenderController.Instance.MarkForRender(this);
    }
}
