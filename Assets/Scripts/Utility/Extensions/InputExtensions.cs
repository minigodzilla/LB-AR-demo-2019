using UnityEngine;

public static class InputExtensions
{
    public static float PixelsToInches(float pixels) => pixels / Screen.dpi;
    public static float InchesToPixels(float inches) => inches * Screen.dpi;

    public static bool RaycastFromCamera(Camera cam, Vector2 screenPos, out RaycastHit result)
    {
        if (cam == null) { result = new RaycastHit(); return false; }

        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out hit)) { result = hit; return true; }

        result = hit;
        return false;
    }
}
