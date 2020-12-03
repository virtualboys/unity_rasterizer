using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeshManager : MonoBehaviour
{
    private Renderer _renderer;

    private int[] _inds;
    private int _numFaces;
    private Vector3[] _vertices;
    private Vector3[] _normals;
    private Vector2[] _uvs;

    public BoundingBox BoundingBox { get { return _boundingBox; } }
    private BoundingBox _boundingBox;

    public int NumVerts { get { return _vertices.Length; } }
    public int NumFaces { get { return _numFaces; } }

    public virtual Matrix4x4 ModelMatrix { get { return _renderer.transform.localToWorldMatrix; } }

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

    protected virtual void OnRender() { }

    protected void InitBuffers(Mesh mesh, Renderer renderer)
    {
        _renderer = renderer;
        _boundingBox = new BoundingBox();

        InitCPUBuffers(mesh);
        InitGPUBuffers();
    }

    public Bounds GetWorldBounds()
    {
        return _renderer.bounds;
    }

    private void InitCPUBuffers(Mesh mesh)
    {
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

    protected void UpdateBuffers(Mesh mesh)
    {
        _vertices = mesh.vertices;
        _vertexInBuffer.SetData(_vertices);

        _normals = mesh.normals;
        _normalInBuffer.SetData(_normals);
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
        OnRender();

        _boundingBox.UpdateCorners(_renderer.bounds);
        //for(int i = 0; i < 8; i++)
        //{
        //    Debug.DrawLine(_boundingBox.Corners[i], _boundingBox.Corners[(i + 1) % 8], Color.red);
        //}
        RenderController.Instance.MarkForRender(this);
    }
}
