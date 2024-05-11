using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ARSessionInitializer))]
public class ARSceneController : MonoBehaviour
{
    [SerializeField] private Transform structureParent = default;
    [SerializeField] private GameObject lightEstimatePanel = default;
    [SerializeField] private ARStructureReferences structureReferences = default;
    [SerializeField] private ARGameLogic gameLogic = new ARGameLogic();

    private bool willUpdate;

    private ARSessionInitializer sessionInitializer;
    private ARLighting lighting;

    private readonly ARStructureGenerator structureGenerator = new ARStructureGenerator();
    private readonly ARPoseController poseController = new ARPoseController();
    private readonly ARInputControl inputControl = new ARInputControl();

    public GameObject InstantiateObejct(GameObject prefab, Vector3 position, Quaternion rotation)
        => Instantiate(prefab, position, rotation);

    public ARModelCompounds InstantiateObejct(ARModelCompounds modelCompounds, Vector3 position, Quaternion rotation)
        => Instantiate(modelCompounds, position, rotation);

    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;

        sessionInitializer = GetComponent<ARSessionInitializer>();

        sessionInitializer.InitializeSession += InitializeCompounds;
        sessionInitializer.AnchorCreated += OnAnchorCreated;
        sessionInitializer.SessionPaused += () => OnSessionPaused(true);
        sessionInitializer.SessionResumed += () => OnSessionPaused(false);

        lighting = sessionInitializer.lighting;
    }

    private void Update()
    {
        if (!sessionInitializer.willUpdate) return;

        //TODO: Track Session State
        EstimateEnvironmentLight();

        if (!willUpdate) return;

        CheckAnchorStatus();
        gameLogic.Update();
    }

    private void CheckAnchorStatus()
    {
        if (sessionInitializer.anchor != null || sessionInitializer.anchor.pending) return;
        if (poseController.isPoseValid) sessionInitializer.OnCreateAnchor(poseController.pose);
    }

    private void EstimateEnvironmentLight()
    {
        if (willUpdate || !lighting.frameChanged) return;

        if (!lighting.isBright()) lightEstimatePanel.TrySetActive(true);
        else
        {
            lightEstimatePanel.TrySetActive(false);
            willUpdate = true;
        }
    }

    private void InitializeCompounds()
    {
        lighting.StartCoroutine(lighting.WaitForCleanFrame());

        poseController.Initialize(sessionInitializer.raycastManager, sessionInitializer.sessionOrigin.camera,
            sessionInitializer.planeManager, OnStartCoroutine, () => gameLogic.ShowTapHint(true));

        inputControl.Initialize(sessionInitializer.sessionOrigin.camera);

        gameLogic.Initialize(structureParent, structureGenerator, structureReferences, poseController, inputControl,
            OnStopCoroutines, OnStartCoroutine, structureGenerator.Reset);

        structureGenerator.Initialize(structureParent, DestroyObject, this, structureReferences,
            gameLogic.OnStructureCreated, gameLogic.OnStructureRemoved);
    }

    private IEnumerator InitializeCoroutine(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

    private void OnStartCoroutine(Action action, float delay)
    {
        StartCoroutine(InitializeCoroutine(action, delay));
        gameLogic.OnCoroutineStarted(true);
    }

    private void OnStopCoroutines()
    {
        StopAllCoroutines();
        gameLogic.OnCoroutineStarted(false);
    }

    private void OnSessionPaused(bool paused) => gameLogic.OnSessionPaused(paused);
    private void OnAnchorCreated() => gameLogic.OnAnchorCreated(sessionInitializer.anchor.transform);
    private void DestroyObject(GameObject Object) => Destroy(Object);
    private void OnDestroy() => inputControl.Unsubscribe();
}