using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticMeshManager : MeshManager
{
    private void Start()
    {
        var rend = GetComponentInChildren<MeshRenderer>();
        var mesh = GetComponentInChildren<MeshFilter>();
        if (rend == null || mesh == null)
        {
            Debug.LogError("No mesh renderer found!");
            return;
        }

        InitBuffers(mesh.sharedMesh, rend);
    }
}
