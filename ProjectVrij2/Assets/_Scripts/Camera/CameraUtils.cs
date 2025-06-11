using UnityEngine;

public static class CameraUtils
{
    public static Vector3 ConvertToCameraSpace(Vector3 vector, bool isFlat = true)
    {
        Vector3 forward = Camera.main.transform.forward; 
        Vector3 right = Camera.main.transform.right;

        if (isFlat) 
        {
            forward.y = 0;
            right.y = 0;        
        }

        forward.Normalize();    
        right.Normalize();

        return vector.z * forward + vector.x * right;
    }

    public static Vector3 ConvertToTransformSpace(Vector3 vector, Transform transform, bool isFlat = true)
    {
        Vector3 forward = transform.forward;
        Vector3 right =transform.right;

        if (isFlat)
        {
            forward.y = 0;
            right.y = 0;
        }

        forward.Normalize();
        right.Normalize();

        return vector.z * forward + vector.x * right;
    }
}
