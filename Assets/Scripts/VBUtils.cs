


using UnityEngine;

public static class VBUtils
{
    public enum Axis
    {
        x,
        y,
        z
    }

    public static float GetComponent(Vector3 vec, Axis axis)
    {
        switch(axis)
        {
            case Axis.x:
                return vec.x;
            case Axis.y:
                return vec.y;
            case Axis.z:
                return vec.z;
        }

        throw new System.Exception("U messed up");
    }

    public static Vector3 GetDirection(Transform transform, Axis axis)
    {
        switch(axis)
        {
            case Axis.x:
                return transform.right;
            case Axis.y:
                return transform.up;
            case Axis.z:
                return transform.forward;
        }

        throw new System.Exception("u messed up");
    }
}