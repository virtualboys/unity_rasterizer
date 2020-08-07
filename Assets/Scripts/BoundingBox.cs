using UnityEngine;

public class BoundingBox
{
    public Vector3[] Corners;

    public BoundingBox()
    {
        Corners = new Vector3[8];
    }

    public void UpdateCorners(Bounds bounds)
    {
        Corners[0] = bounds.min;
        Corners[1] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
        Corners[2] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
        Corners[3] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
        Corners[4] = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);
        Corners[5] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
        Corners[6] = bounds.max;
        Corners[7] = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
    }
}