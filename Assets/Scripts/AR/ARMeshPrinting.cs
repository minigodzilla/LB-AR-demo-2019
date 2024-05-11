using UnityEngine;

public class ARMeshPrinting
{
    private int Indexer = -1;

    private readonly ARModelCompounds modelCompounds;
    private readonly ARStructureReferences structureReferences;

    private float maxPrintHeight;
    private float minPrintHeight;
    private float heightVelocity;
    private float printDuration;

    private readonly bool isInitialized;

    public ARMeshPrinting(ARModelCompounds modelCompounds, ARStructureReferences structureReferences)
    {
        this.modelCompounds = modelCompounds;
        this.structureReferences = structureReferences;

        isInitialized = true;
    }

    public void PrintModel()
    {
        Indexer++;

        if (Indexer > modelCompounds.printables.Length - 1) return;

        modelCompounds.printableGroups.Add(modelCompounds.printables[Indexer]);
    }

    public void UpdateVisuals(Transform blueprint)
    {
        if (!modelCompounds.WillPrintModel() || !isInitialized) return;

        if (structureReferences.editMode != AREditMode.View)
        {
            ResetPrintables(blueprint);
            return;
        }

        if (modelCompounds.printableGroups.Count <= 0) PrintModel();

        StartPrintingModel();
    }

    private void StartPrintingModel()
    {
        for (int i = 0; i < modelCompounds.printableGroups.Count; i++) OnPrintModel(i);
    }

    private void OnPrintModel(int Index)
    {
        for (int i = 0; i < modelCompounds.printableGroups[Index].materials.Length; i++)
        {
            float height = Mathf.SmoothDamp(modelCompounds.printableGroups[Index].materials[i].GetFloat(structureReferences.printValue),
                maxPrintHeight, ref heightVelocity, printDuration);

            modelCompounds.printableGroups[Index].materials[i].SetFloat(structureReferences.printValue, height);
        }
    }

    private void ResetPrintables(Transform blueprint)
    {
        if (modelCompounds.printableGroups.Count > 0) modelCompounds.printableGroups.Clear();

        printDuration = structureReferences.printDuration * blueprint.localScale.y;
        minPrintHeight = structureReferences.minPrintHeight + blueprint.position.y;
        maxPrintHeight = (blueprint.localScale.y * structureReferences.maxPrintHeight) + blueprint.position.y;

        heightVelocity = 0;
        Indexer = -1;

        for (int i = 0; i < modelCompounds.printables.Length; i++)
        {
            for (int a = 0; a < modelCompounds.printables[i].materials.Length; a++)
            {
                modelCompounds.printables[i].materials[a].SetFloat(
                    structureReferences.printValue, minPrintHeight);
            }
        }
    }
}
