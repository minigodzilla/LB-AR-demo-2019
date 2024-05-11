using System;
using UnityEngine;
using UnityEngine.UI;

public static class CustomExtensions
{
    public static void TrySetText(this Text text, string newText)
    {
        if (!text.text.Equals(newText)) text.text = newText;
    }

    public static void TrySetInteractable(this Button button, bool condition)
    {
        if(button.interactable != condition) button.interactable = condition;
    }

    public static void TrySetColor(this Image image, Color color)
    {
        if (image.color != color) image.color = color;
    }

    public static void SetTimeFormatted(this Text timer, float currentSeconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(currentSeconds);
        timer.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
    }

    public static void TrySetActive(this GameObject GO, bool active)
    {
        if (GO.activeInHierarchy != active) GO.SetActive(active);
    }

    public static void TrySetEnable(this Behaviour behaviour, bool willEnabled)
    {
        if (behaviour.enabled != willEnabled) behaviour.enabled = willEnabled;
    }

    public static void TrySetEnableCollider(this Collider collider, bool willEnabled)
    {
        if (collider.enabled != willEnabled) collider.enabled = willEnabled;
    }

    public static void TryChangeColor(this Image image, Color color)
    {
        if (!image.gameObject.activeInHierarchy) return;
        if (image.color != color) image.color = color;
    }

    public static Vector3 WorldToCanvasSpace(this Canvas canvas, Vector3 screenPos)
    {
        Vector2 viewPortPos;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform, screenPos, canvas.worldCamera, out viewPortPos);

        return canvas.transform.TransformPoint(viewPortPos);
    }
}
