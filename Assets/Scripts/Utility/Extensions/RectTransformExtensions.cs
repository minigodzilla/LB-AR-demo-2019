using UnityEngine;

public static class RectTransformExtensions
{
    private readonly static Vector3[] corners = new Vector3[4];

    public static Rect CanvasRect(this RectTransform rt)
    {
        rt.GetWorldCorners(corners);
        return Rect.MinMaxRect(corners[0].x, corners[0].y, corners[2].x, corners[2].y);
    }
}