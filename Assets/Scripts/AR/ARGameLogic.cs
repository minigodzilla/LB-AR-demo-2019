using System;
using UnityEngine;

[Serializable]
public class ARGameLogic
{
    [SerializeField] private GameObject poseIndicator = default, scanHint = default, notification = default;

    [SerializeField] private ARGameHUD gameHUD = new ARGameHUD();

    private ARStructureGenerator structureGenerator;
    private ARStructureReferences structureReferences;
    private ARPoseController poseController;
    private ARInputControl inputControl;

    private Action StopCoroutines, StructureCreated;
    private Action<Action, float> StartCoroutine;

    private Transform modelParent;

    private bool wasCoroutineStarted;

    private readonly ARStructureEditor structureEditor = new ARStructureEditor();

    public void Initialize(Transform modelParent, ARStructureGenerator structureGenerator, ARStructureReferences structureReferences,
        ARPoseController poseController, ARInputControl inputControl, Action StopCoroutines,
        Action<Action, float> StartCoroutine, Action RemoveObject)
    {
        this.poseController = poseController;
        this.structureReferences = structureReferences;
        this.inputControl = inputControl;
        this.structureGenerator = structureGenerator;
        this.modelParent = modelParent;

        this.StartCoroutine = StartCoroutine;
        this.StopCoroutines = StopCoroutines;

        AssignInputEvents();
        gameHUD.Initialize(structureReferences, RemoveObject, structureEditor.OnEnableColorCoding);

        StructureCreated = gameHUD.OnStructureCreated;
        StructureCreated += InitializeGame;
    }

    public void Update()
    {
        if (!structureGenerator.wasStructureCreated)
        {
            poseController.UpdateCentreRayPoint();
            poseController.UpdateIndicator(poseIndicator, scanHint);
        }

        structureEditor.Update();
    }

    public void OnAnchorCreated(Transform transform) => modelParent.parent = transform;
    public void OnSessionPaused(bool paused) => gameHUD.OnSessionPaused(paused);
    public void OnCoroutineStarted(bool wasStarted) => wasCoroutineStarted = wasStarted;
    public void OnStructureCreated() => StructureCreated?.Invoke();
    public void OnStructureRemoved() => AssignInputEvents();

    public void ShowTapHint(bool show)
    {
        if (!wasCoroutineStarted && show) return;
        if (wasCoroutineStarted && !show) StopCoroutines?.Invoke();

        gameHUD.ShowTapHint(show);
    }

    private void AssignInputEvents() => inputControl.AssignInputEvents(TapCompleted, null, null, null, null, null, null, false);

    private void TapCompleted(GameObject GO)
    {
        if (structureGenerator.wasStructureCreated)
        {
            if (structureGenerator.IsTargetTouched(GO))
            {
                notification.TrySetActive(false);
            }
        }
        else
        {
            if (GO == poseIndicator)
            {
                structureGenerator.GenerateStructure(poseController.pose);
                ShowTapHint(false);
                ShowIndicators(false);
            }
        }
    }

    private void ShowIndicators(bool show)
    {
        poseIndicator.TrySetActive(show);
        scanHint.TrySetActive(show);
    }

    private void InitializeGame() => structureEditor.Initialize(poseController, inputControl, structureGenerator, structureReferences, gameHUD.OnForceRemoteCallback);
}