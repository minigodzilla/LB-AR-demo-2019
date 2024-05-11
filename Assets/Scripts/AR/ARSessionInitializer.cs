using System;
using UnityEngine;
using System.Collections;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARSession), typeof(ARInputManager))]
public class ARSessionInitializer : MonoBehaviour
{
    [HideInInspector] public bool willUpdate;

    public ARRaycastManager raycastManager;
    public ARSessionOrigin sessionOrigin;
    public ARPlaneManager planeManager;
    public ARAnchorManager anchorManager;
    public ARLighting lighting;

    public Action InitializeSession;
    public Action AnchorCreated;
    public Action SessionPaused;
    public Action SessionResumed;

    public ARAnchor anchor;

    private ARSession session;
    private ARInputManager ARInput;

    private readonly LayerMask defaultCameraLM = -1;
    private readonly LayerMask restoreCameraLM = ~1 << 0;

    private bool restoreSession = false;
    private const float sessionRestoreTimer = 3f;

    public void OnCreateAnchor(Pose pose) => StartCoroutine(CreateAnchor(pose));

    public void OnTrackSessionState()
    {
        if (ARSession.state == ARSessionState.SessionTracking)
        {
            sessionOrigin.camera.TryLayerMask(defaultCameraLM);
            SessionResumed?.Invoke();
            return;
        }

        if (!restoreSession)
        {
            StartCoroutine(OnRestoreSession());
            restoreSession = true;
        }
    }

    private void Awake()
    {
        sessionOrigin.gameObject.TrySetActive(false);
        session = GetComponent<ARSession>();
        ARInput = GetComponent<ARInputManager>();
    }

    private void Start()
    {
        sessionOrigin.gameObject.SetActive(true);

        session.enabled = true;
        ARInput.enabled = true;

        if (ARSession.state == ARSessionState.None ||
            ARSession.state == ARSessionState.CheckingAvailability)
        {
            StartCoroutine(ARSession.CheckAvailability());
            ARSession.stateChanged += TryInitialize;
        }

        InitializeSession?.Invoke();

        willUpdate = true;
    }

    private void TryInitialize(ARSessionStateChangedEventArgs e)
    {
        if (e.state == ARSessionState.None || e.state == ARSessionState.CheckingAvailability) return;
        else if (e.state == ARSessionState.Unsupported)
        {
            Debug.LogWarning("Not Compatible");
            return;
        }
        else ARSession.stateChanged -= TryInitialize;
    }

    private IEnumerator CreateAnchor(Pose pose)
    {
        anchor = anchorManager.AddAnchor(pose);

        while (anchor == null || anchor.pending) yield return null;

        AnchorCreated?.Invoke();
    }

    private IEnumerator OnRestoreSession()
    {
        yield return new WaitForSeconds(sessionRestoreTimer);

        if (ARSession.state != ARSessionState.SessionTracking)
        {
            sessionOrigin.camera.TryLayerMask(restoreCameraLM);
            SessionPaused?.Invoke();
        }
        else restoreSession = false;
    }
}