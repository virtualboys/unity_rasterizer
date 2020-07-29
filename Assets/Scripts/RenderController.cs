using System.Collections;
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
        _frustumPlanes[1] = new Plane(farPlane.Corners[0], farPlane.Corners[1], farPlane.Corners[2]);
        _frustumPlanes[2] = new Plane(farPlane.Corners[0], farPlane.Corners[1], nearPlane.Corners[1]);
        _frustumPlanes[3] = new Plane(nearPlane.Corners[1], farPlane.Corners[1], farPlane.Corners[2]);
        _frustumPlanes[4] = new Plane(nearPlane.Corners[3], nearPlane.Corners[2], farPlane.Corners[2]);
        _frustumPlanes[5] = new Plane(nearPlane.Corners[0], nearPlane.Corners[3], farPlane.Corners[3]);
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

    //int BoxInFrustum()

 //   int FrustumG::boxInFrustum(Box &b)
 //   {

 //       int result = INSIDE, out,in;

 //       // for each plane do ...
 //       for (int i = 0; i < 6; i++)
 //       {

	//	// reset counters for corners in and out
	//	out= 0;in= 0;
 //           // for each corner of the box do ...
 //           // get out of the cycle as soon as a box as corners
 //           // both inside and out of the frustum
 //           for (int k = 0; k < 8 && (in== 0 || out== 0) ; k++) {

 //           // is the corner outside or inside
 //           if (pl[i].distance(b.getVertex(k)) < 0)
	//			out++;
	//		else
	//			in++;
 //       }
 //       //if all corners are out
 //       if (!in)
	//		return (OUTSIDE);
	//	// if some corners are out and others are in
	//	else if (out)
	//		result = INTERSECT;
 //   }
	//return(result);
 //}


private void OnPreRender()
    {
        //_nearPlane = GetPlaneDimensions(_gameCamera, true);
        //_farPlane = GetPlaneDimensions(_gameCamera, false);

        _rasterizer.BeginFrame(_gameCamera, _clearColor);
        foreach(var mesh in _meshes)
        {
            _rasterizer.DrawModel(mesh);
        }
        _rasterizer.EndFrame(_gameCamera);

        _meshes.Clear();
    }
}
