using UnityEngine;

public class ARModelSwapping
{
    private float timer;
    private int Indexer = 0;

    private bool wereModelsPrepared;

    private readonly GameObject lego;
    private readonly ARModelCompounds structure;
    private readonly ARStructureReferences structureReferences;
    private readonly ARStructureGenerator structureGenerator;

    private readonly bool isInitialized;

    public ARModelSwapping(GameObject lego, ARModelCompounds structure, ARStructureReferences structureReferences, ARStructureGenerator structureGenerator)
    {
        this.lego = lego;
        this.structure = structure;
        this.structureGenerator = structureGenerator;
        this.structureReferences = structureReferences;

        isInitialized = true;
    }

    public GameObject CurrentModel() => structureReferences.swappableModels[Indexer];

    public void UpdateModelSwapping()
    {
        if (!isInitialized) return;

        bool isViewMode = structureReferences.editMode == AREditMode.View;

        UpdateSwapDuration(isViewMode);

        lego.TrySetActive(false);
        structure.gameObject.TrySetActive(false);
        PrepareSwappables();
    }

    private void PrepareSwappables()
    {
        if (!structureGenerator.wasStructureCreated) return;
        if (!structureReferences.willSwapModels) return;
        if (wereModelsPrepared) return;

        for (int i = 0; i < structureReferences.swappableModels.Count; i++)
        {
            structureReferences.swappableModels[i].TrySetActive(i == 0);

            structureReferences.swappableModels[i].transform.localPosition = lego.transform.localPosition;
            structureReferences.swappableModels[i].transform.localRotation = lego.transform.localRotation;
            structureReferences.swappableModels[i].transform.localScale = structureReferences.maximumSize * structureReferences.minimumSize * Vector3.one;
        }

        wereModelsPrepared = true;
    }

    private void UpdateSwapDuration(bool isViewMode)
    {
        if (!isViewMode)
        {
            timer = 0;
            UpdateModelIndexer(isViewMode);
            return;
        }

        if (timer <= 0)
        {
            ResetStructureTransform();
            UpdateModelIndexer(isViewMode);
            timer = structureReferences.modelSwapping.duration;
            return;
        }

        timer -= Time.deltaTime;
    }

    private void UpdateModelIndexer(bool isViewMode)
    {
        if (!isViewMode)
        {
            for (int i = 0; i < structureReferences.swappableModels.Count; i++)
            {
                if(i != Indexer) structureReferences.swappableModels[i].TrySetActive(false);
            }
        }
        else
        {
            structureReferences.swappableModels[Indexer].SetActive(false);

            Indexer++;

            if (Indexer > structureReferences.swappableModels.Count - 1) Indexer = 0;

            structureReferences.swappableModels[Indexer].SetActive(true);
        }
    }

    private void ResetStructureTransform()
    {
        for (int i = 0; i < structureReferences.swappableModels.Count; i++)
        {
            structureReferences.swappableModels[i].transform.localPosition = CurrentModel().transform.localPosition;
            structureReferences.swappableModels[i].transform.localRotation = CurrentModel().transform.localRotation;
            structureReferences.swappableModels[i].transform.localScale = CurrentModel().transform.localScale;
        }
    }
}
