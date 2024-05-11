using System;
using UnityEngine;

public class ARStructureGenerator
{
    public bool wasStructureCreated = false;

    [HideInInspector] public GameObject lego;
    [HideInInspector] public ARModelCompounds structure;

    private ARStructureReferences structureReferences;
    private ARSceneController sceneController;

    private Action<GameObject> Destroy;
    private Action OnStructureCreated;
    private Action OnReset;

    private Transform structureParent;

    public void Initialize(Transform structureParent, Action<GameObject> Destroy, ARSceneController sceneController,
        ARStructureReferences structureReferences, Action OnStructureCreated, Action OnReset)
    {
        this.sceneController = sceneController;
        this.structureParent = structureParent;
        this.structureReferences = structureReferences;
        this.OnReset = OnReset;
        this.Destroy = Destroy;
        this.OnStructureCreated = OnStructureCreated;

#if UNITY_EDITOR
        Pose pose = new Pose(structureParent.position, structureParent.rotation);
        GenerateStructure(pose);
#endif
        PrepareSwappableModelsList();
    }

    public bool IsTargetTouched(GameObject GO) => structure == GO;

    public void GenerateStructure(Pose pose) => CreateNewStructure(pose);
    public void ShowStructure(bool show) => structure.gameObject.TrySetActive(show);

    public void Reset()
    {
        DestroyStructure();
        OnReset?.Invoke();
        wasStructureCreated = false;
    }

    private void DestroyStructure()
    {
        Destroy?.Invoke(lego);
        Destroy?.Invoke(structure.gameObject);
    }

    private void CreateNewStructure(Pose pose)
    {
        PrepareStructureModel(pose);
        PrepareLegoModel(pose);

        if (wasStructureCreated) return;

        OnStructureCreated?.Invoke();
        wasStructureCreated = true;
    }

    private void PrepareLegoModel(Pose pose)
    {
        if (structureReferences.willSwapTextures)
        {
            lego = structure.gameObject;
            return;
        }

        lego = sceneController.InstantiateObejct(structureReferences.legoPrefab, pose.position, Quaternion.identity);

        lego.transform.parent = structureParent;

        if (!structureReferences.isTerrain) lego.transform.localScale = Vector3.one;

        lego.transform.SetPositionAndRotation(pose.position, pose.rotation);
    }

    private void PrepareStructureModel(Pose pose)
    {
        structure = sceneController.InstantiateObejct(structureReferences.structurePrefab, pose.position, Quaternion.identity);

        if (structureReferences.willPrintModel) structure.EnablePrinting();

        structure.transform.parent = structureParent;
        structure.transform.localScale = Vector3.one;
        structure.transform.SetPositionAndRotation(pose.position, pose.rotation);
    }

    private void PrepareSwappableModelsList()
    {

        structureReferences.swappableModels.Clear();

        foreach (var item in structureReferences.modelSwapping.models)
        {
            GameObject model = sceneController.InstantiateObejct(item, structureParent.position, Quaternion.identity);

            model.transform.parent = structureParent;
            model.SetActive(false);

            structureReferences.swappableModels.Add(model);
        }
    }
}