using System;
using UnityEngine;

public class ARStructureEditor
{
    private ARStructureReferences structureReferences;
    private ARStructureGenerator structureGenerator;
    private ARPoseController poseController;
    private ARTextureSwapping textureSwapping;
    private ARInputControl inputControl;

    private GameObject lego;
    private ARModelCompounds legoCompound;
    private ARModelCompounds structure;
    private ARModelSwapping modelSwapping;
    private ARMeshPrinting meshPrinting;

    private Vector3 dragOffset, pinchOffset, rotationOffset;

    private Action<AREditMode> ForceRemoteCallback;

    private bool isPinching;
    private bool isDragging;
    private bool isInitialized;

    private float initialeScreenPos;

    private const float pinchSmoothTime = 1.2f;
    private const float maxRotationSpeed = 0.5f;

    public void Initialize(ARPoseController poseController, ARInputControl inputControl, ARStructureGenerator structureGenerator,
        ARStructureReferences structureReferences, Action<AREditMode> ForceRemoteCallback)
    {
        this.poseController = poseController;
        this.structureReferences = structureReferences;
        this.structureGenerator = structureGenerator;
        this.inputControl = inputControl;
        this.ForceRemoteCallback = ForceRemoteCallback;

        structure = structureGenerator.structure;
        lego = structureGenerator.lego;

        meshPrinting = new ARMeshPrinting(structure, structureReferences);
        textureSwapping = new ARTextureSwapping(structure, structureReferences);
        modelSwapping = new ARModelSwapping(lego, structure, structureReferences, structureGenerator);

        inputControl.AssignInputEvents(ProccessHit, DragStarted, DragMoved, OnDragEnded, OnPinchStarted, OnPinchChanged, OnPinchEnded, false);

        PrepareColorCoding();

        isInitialized = true;
    }

    public void Update()
    {
        if (!isInitialized) return;

        UpdateVisuals();
    }

    public void UpdateStructureRefs(GameObject lego, ARModelCompounds structure = null)
    {
        this.lego = lego;
        if (structure != null) this.structure = structure;
    }

    public void OnEnableColorCoding(bool enable)
    {
        if (legoCompound == null) return;

        legoCompound.colorCodedRenderer.materials = enable ? legoCompound.colorCodedMaterials : legoCompound.originalMaterails;
    }

    private void UpdateVisuals()
    {
        if (Target() == null || structure == null) return;

        if (structureReferences.willPrintModel) meshPrinting.UpdateVisuals(Target().transform);

        if (!structureReferences.willSwapModels) UpdateModelTransition();
        else modelSwapping.UpdateModelSwapping();
    }

    private void UpdateModelTransition()
    {
        bool isViewMode = structureReferences.editMode == AREditMode.View ? true : false;

        if (structureReferences.willSwapTextures)
        {
            structure.mainCollider.TrySetEnableCollider(!isViewMode);
            structure.EnableTextureMapping(isViewMode);

            return;
        }

        if (isViewMode) ResetStructureTransform();

        lego.TrySetActive(!isViewMode);
        structure.gameObject.TrySetActive(isViewMode);
    }

    private void ProccessHit(GameObject GO)
    {
        if (structureReferences.editMode != AREditMode.View)
        {
            if (structureGenerator.IsTargetTouched(GO)) lego = GO;
        }
        else
        {
            if (structureReferences.willSwapModels) ForceRemoteCallback?.Invoke(AREditMode.EditRotation);

            if (structureReferences.willSwapTextures)
            {
                Compound c = structure.TargetCompound(GO);
                textureSwapping.SwapTexture(c);
            }

            if (structureReferences.willPrintModel && GO == structure.gameObject) meshPrinting.PrintModel();
        }
    }

    private void DragStarted(GameObject GO, Vector3 screenPos)
    {
        OnDragStartedPoistion(GO, screenPos);
        OnDragStartedRotation(GO, screenPos);
    }

    private void DragMoved(Vector3 screenPos)
    {
        ProcessNewPoistion(screenPos);
        ProcessNewRotation(screenPos);
    }

    private void OnDragEnded() => isDragging = false;

    private void OnPinchStarted()
    {
        if (structureReferences.editMode != AREditMode.EditScale) return;
        isDragging = false;
        isPinching = true;

        if (Target() != null) pinchOffset = Target().transform.localScale;
    }

    private void OnPinchChanged(Pinch pinch)
    {
        if (structureReferences.editMode != AREditMode.EditScale) return;
        if (Target() == null || !isPinching) return;

        Target().transform.localScale += (-pinch.amount) * pinchOffset * pinchSmoothTime;

        if (Target().transform.localScale.x < structureReferences.minimumSize)
            Target().transform.localScale = SetStructureSize(structureReferences.minimumSize);

        if (Target().transform.localScale.x > structureReferences.maximumSize)
            Target().transform.localScale = SetStructureSize(structureReferences.maximumSize);
    }

    private void OnPinchEnded() => isPinching = false;

    private void ResetStructureTransform()
    {
        if (structure == null) return;

        structure.transform.localPosition = Target().transform.localPosition;
        structure.transform.localRotation = Target().transform.localRotation;
        structure.transform.localScale = Target().transform.localScale;
    }

    private void OnDragStartedPoistion(GameObject GO, Vector3 screenPos)
    {
        if (structureReferences.editMode != AREditMode.EditPosition) return;

        if (isPinching || Target() != GO) return;

        isDragging = true;
        poseController.UpdatePointerRayPoint(screenPos);

        if (poseController.isPoseValid)
        {
            Vector3 desiredPos = poseController.pose.position;
            dragOffset = Target().transform.position - desiredPos;
        }
    }

    private void OnDragStartedRotation(GameObject GO, Vector3 screenPos)
    {
        if (structureReferences.editMode != AREditMode.EditRotation) return;

        if (isPinching || Target() != GO) return;

        isDragging = true;
        initialeScreenPos = screenPos.x;
        rotationOffset = Target().transform.eulerAngles;
    }

    private void ProcessNewPoistion(Vector3 screenPos)
    {
        if (structureReferences.editMode != AREditMode.EditPosition) return;
        if (Target() == null || isPinching || !isDragging) return;

        poseController.UpdatePointerRayPoint(screenPos);

        if (poseController.isPoseValid)
        {
            Vector3 desiredPos = poseController.pose.position + dragOffset;
            Target().transform.position = new Vector3(desiredPos.x, desiredPos.y, desiredPos.z);
        }
    }

    private void ProcessNewRotation(Vector3 screenPos)
    {
        if (structureReferences.editMode != AREditMode.EditRotation) return;
        if (Target() == null || isPinching || !isDragging) return;

        if (initialeScreenPos > screenPos.x) rotationOffset.y += maxRotationSpeed * inputControl.dragFrequency;
        if (initialeScreenPos < screenPos.x) rotationOffset.y -= maxRotationSpeed * inputControl.dragFrequency;

        Target().transform.eulerAngles = rotationOffset;
        initialeScreenPos = screenPos.x;
    }

    private void PrepareColorCoding()
    {
        if(lego.TryGetComponent(out ARModelCompounds c))
        {
            if (c.followCameraForward)
            {
                Vector3 dir = Camera.main.transform.forward;

                dir.y = 0;
                dir.Normalize();

                c.gameObject.transform.forward = dir;
            }

            if (c.WillColorCode())
            {
                legoCompound = c;
                legoCompound.PrepareColorCoding();
            }
        }
    }

    private GameObject Target() => structureReferences.willSwapModels ? modelSwapping.CurrentModel() : lego;
    private Vector3 SetStructureSize(float size) => new Vector3(size, size, size);
}