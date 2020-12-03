using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshManager : MeshManager
{
    private SkinnedMeshRenderer _skinnedRenderer;
    private Mesh _bakedMesh;

    // transform is baked in vertices
    //public override Matrix4x4 ModelMatrix { get { return Matrix4x4.identity; } }

    private void Start()
    {
        _skinnedRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if (_skinnedRenderer == null)
        {
            Debug.LogError("No skinned mesh renderer found!");
            return;
        }

        _bakedMesh = new Mesh();
        InitBuffers(_skinnedRenderer.sharedMesh, _skinnedRenderer);
    }

    protected override void OnRender()
    {
        _skinnedRenderer.BakeMesh(_bakedMesh);
        UpdateBuffers(_bakedMesh);
    }
}
