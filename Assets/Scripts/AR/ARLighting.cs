using UnityEngine;
using System.Collections;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARCameraManager))]
public class ARLighting : MonoBehaviour
{
    public float? Brightness { get; private set; }
    [HideInInspector] public bool frameChanged;

    [SerializeField] private Light mainLight = default;

    private ARCameraManager cameraManager;

    private const float delay = 0.6f;
    private const float minBrightness = 0.4f;

    public bool isBright()
    {
        if (!Brightness.HasValue) return true;

        if (Brightness.Value >= minBrightness) return true;
        else return false;
    }

    public IEnumerator WaitForCleanFrame()
    {
        yield return new WaitForSecondsRealtime(delay);
        frameChanged = true;
    }

    private void Awake() => cameraManager = GetComponent<ARCameraManager>();
    private void OnEnable() => cameraManager.frameReceived += OnFrameChanged;

    private void OnFrameChanged(ARCameraFrameEventArgs args)
    {
        if (args.lightEstimation.averageBrightness.HasValue)
        {
            Brightness = args.lightEstimation.averageBrightness.Value;
            mainLight.intensity = Brightness.Value;
        }
    }

    private void OnDisable() => cameraManager.frameReceived -= OnFrameChanged;
}
