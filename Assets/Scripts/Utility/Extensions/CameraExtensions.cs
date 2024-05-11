using UnityEngine;

public static class CameraExtensions
{
    public static float OrthoRight(this Camera camera) 
        => camera.transform.position.x + camera.orthographicSize * camera.aspect;
    public static float OrthoLeft(this Camera camera) 
        => camera.transform.position.x - camera.orthographicSize * camera.aspect;

    public static float PhysicalToWorldDistance(this Camera cam, float inches)
    {
        return ScreenToWorldDistance(cam, inches * Screen.dpi);
    }

    public static float ScreenToWorldDistance(this Camera cam, float distance)
    {
        // Convert the (0, radius) direction vector into world space
        var referencePoint = cam.ScreenToWorldPoint(Vector3.zero);
        var offset = Vector3.up * distance;
        offset = cam.ScreenToWorldPoint(offset);

        // World space radius is its magnitude
        return (offset - referencePoint).magnitude;
    }

    public static Vector3 ScreenCenterWS(this Camera cam) =>
        cam.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f));

    public static float WorldToScreenDistance(this Camera cam, float distance)
    {
        // Convert the (0, radius) direction vector into screen space
        var referencePoint = cam.WorldToScreenPoint(Vector3.zero);
        var offset = Vector3.up * distance;
        offset = cam.WorldToScreenPoint(offset);

        // Screen space radius is its magnitude
        return (offset - referencePoint).magnitude;
    }

    public static void TryLayerMask(this Camera camera, LayerMask layerMask)
    {
        if (camera.cullingMask != layerMask) camera.cullingMask = layerMask;
    }

    public static Vector3 RelativeWorldToScreenPoint(this Camera camera, Vector3 vector)
    {
        const float projecttionOffset = 1.01f;

        Vector3 dir = camera.transform.forward;
        Vector3 dist = vector - camera.transform.position;

        float dot = Vector3.Dot(dir, dist);

        if (dot <= 0)
        {
            Vector3 projection = dir * dot * projecttionOffset;
            vector = dist - projection + camera.transform.position;
        }

        return RectTransformUtility.WorldToScreenPoint(camera, vector);
    }
}
