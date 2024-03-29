﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class RenderController : MonoBehaviour
{
    private class Rect3D
    {
        public Vector3[] Corners;

        public Rect3D(Rect viewRect, Transform cam, float dist)
        {
            Corners = new Vector3[4];
            Vector3 center = cam.position + cam.forward * dist;
            Corners[0] = center - .5f * viewRect.width * cam.right - .5f * viewRect.height * cam.up;
            Corners[1] = center - .5f * viewRect.width * cam.right + .5f * viewRect.height * cam.up;
            Corners[2] = center + .5f * viewRect.width * cam.right + .5f * viewRect.height * cam.up;
            Corners[3] = center + .5f * viewRect.width * cam.right - .5f * viewRect.height * cam.up;
        }
    }

    public static RenderController Instance { get { return _instance; } }
    private static RenderController _instance;

    [SerializeField] private RasterizerController _rasterizer;
    [SerializeField] private Camera _gameCamera;
    [SerializeField] private MeshRenderer _screenMesh;

    [SerializeField] private Color _clearColor;

    private Camera _screenCamera;
    private List<MeshManager> _meshes;

    private Plane[] _frustumPlanes;
    
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
        _frustumPlanes = new Plane[6];
    }

    public void MarkForRender(MeshManager mesh)
    {
        if(!_meshes.Contains(mesh))
        {
            _meshes.Add(mesh);
        }
    }

    private void ComputeFrustumPlanes()
    {
        Rect3D nearPlane = GetViewPlaneRect(_gameCamera, true);
        Rect3D farPlane = GetViewPlaneRect(_gameCamera, false);

        _frustumPlanes[0] = new Plane(nearPlane.Corners[0], nearPlane.Corners[1], nearPlane.Corners[2]);
        _frustumPlanes[1] = new Plane(farPlane.Corners[0], farPlane.Corners[3], farPlane.Corners[2]);
        _frustumPlanes[2] = new Plane(farPlane.Corners[0], farPlane.Corners[1], nearPlane.Corners[1]);
        _frustumPlanes[3] = new Plane(nearPlane.Corners[1], farPlane.Corners[1], farPlane.Corners[2]);
        _frustumPlanes[4] = new Plane(nearPlane.Corners[3], nearPlane.Corners[2], farPlane.Corners[2]);
        _frustumPlanes[5] = new Plane(nearPlane.Corners[0], nearPlane.Corners[3], farPlane.Corners[3]);

        //for(int i = 0; i < 4; i++)
        //{
        //    Debug.DrawLine(nearPlane.Corners[i], farPlane.Corners[i], Color.green);
        //}

        //foreach(var plane in _frustumPlanes)
        //{
        //    Debug.DrawLine(_gameCamera.transform.position, _gameCamera.transform.position + (plane.normal * 20), Color.cyan);
        //}
    }

    Rect3D GetViewPlaneRect(Camera cam, bool nearPlane)
    {
        Rect r = new Rect();
        float a = (nearPlane) ? cam.nearClipPlane : cam.farClipPlane;//get length
        float A = cam.fieldOfView * 0.5f;//get angle
        A = A * Mathf.Deg2Rad;//convert tor radians
        float h = (Mathf.Tan(A) * a);//calc height
        float w = (h / cam.pixelHeight) * cam.pixelWidth;//deduct width

        r.xMin = -w;
        r.xMax = w;
        r.yMin = -h;
        r.yMax = h;

        return new Rect3D(r, cam.transform, a);
    }

    private bool BoxInFrustum(BoundingBox bounds)
    {
        foreach(var plane in _frustumPlanes)
        {
            int outCorners = 0;
            int inCorners = 0;
            for(int i = 0; i < bounds.Corners.Length && (outCorners == 0 || inCorners == 0); i++)
            {
                var corner = bounds.Corners[i];
                if(plane.GetDistanceToPoint(corner) > 0)
                {
                    outCorners++;
                }
                else
                {
                    inCorners++;
                }
            }

            if(inCorners == 0)
            {
                return false;
            }
        }

        return true;
    }
    
    private void OnPreRender()
    {
        ComputeFrustumPlanes();

        _rasterizer.BeginFrame(_gameCamera, _clearColor);
        foreach(var mesh in _meshes)
        {
            //if(BoxInFrustum(mesh.BoundingBox))
            {
                _rasterizer.DrawModel(mesh);
            }
        }
        _rasterizer.EndFrame(_gameCamera);

        _meshes.Clear();
    }
}
