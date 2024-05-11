using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPoseController
{
    public bool isPoseValid;
    public Pose pose;

    private Camera currentCamera;
    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;

    private Action<Action, float> StartCoroutine;
    private Action ShowTapHint;

    private bool canShowMessage = true;
    private float scanUpdateTimer = 11f;

    private const float acosValidAngleThreshold = 0.2f;
    private const float showTapHintDelay = 3f;

    private readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();

    public void Initialize(ARRaycastManager raycastManager, Camera currentCamera,
        ARPlaneManager planeManager, Action<Action, float> StartCoroutine, Action ShowTapHint)
    {
        this.raycastManager = raycastManager;
        this.planeManager = planeManager;
        this.currentCamera = currentCamera;

        this.StartCoroutine = StartCoroutine;
        this.ShowTapHint = ShowTapHint;
    }

    public void UpdateCentreRayPoint()
    {
        Vector3 ScreenRayPoint = currentCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        UpdatePose(ScreenRayPoint);
    }

    public void UpdatePointerRayPoint(Vector3 ScreenRayPoint) => UpdatePose(ScreenRayPoint);

    public void UpdateIndicator(GameObject indicatorObject, GameObject scanHint)
    {
        if (isPoseValid)
        {
            UpdateIndicator(indicatorObject, true);
            UpdateScanHint(scanHint, false);
            indicatorObject.transform.SetPositionAndRotation(pose.position, pose.rotation);
        }
        else
        {
            UpdateIndicator(indicatorObject, false);
            UpdateScanHint(scanHint, true);
        }
    }

    private void UpdateIndicator(GameObject indicatorObject, bool show)
    {
        indicatorObject.TrySetActive(show);

        if (show && canShowMessage)
        {
            StartCoroutine?.Invoke(ShowTapHint, showTapHintDelay);
            canShowMessage = false;
        }
    }

    private void UpdateScanHint(GameObject scanHint, bool show)
    {
        const float exitTime = 10f;

        if (scanHint == null) return;
        if (show && scanHint.activeInHierarchy) return;

        if (!show)
        {
            scanHint.TrySetActive(false);
            return;
        }

        if (scanUpdateTimer > exitTime)
        {
            scanUpdateTimer = 0f;
            canShowMessage = true;
            scanHint.TrySetActive(true);
        }
        else scanUpdateTimer += Time.deltaTime;
    }

    private void UpdatePose(Vector3 ScreenRayPoint)
    {
        raycastManager.Raycast(ScreenRayPoint, hits, TrackableType.PlaneWithinPolygon);

        if (hits.Count > 0)
        {
            TrackableId planeID = hits[0].trackableId;
            ARPlane plane = planeManager.GetPlane(planeID);

            isPoseValid = Mathf.Abs(plane.normal.y) > acosValidAngleThreshold;
        }
        else
        {
            isPoseValid = false;
            return;
        }

        pose = hits[0].pose;
        Vector3 poseForward = currentCamera.transform.forward;
        Vector3 poseNormal = new Vector3(poseForward.x, 0, poseForward.z).normalized;
        pose.rotation = Quaternion.LookRotation(poseNormal);
    }
}