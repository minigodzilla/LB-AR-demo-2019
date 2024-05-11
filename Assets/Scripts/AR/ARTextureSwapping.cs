public class ARTextureSwapping
{
    private int Indexer;

    private readonly ARModelCompounds modelCompounds;
    private readonly ARStructureReferences structureReferences;

    private readonly bool isInitialized;

    public ARTextureSwapping(ARModelCompounds modelCompounds, ARStructureReferences structureReferences)
    {
        this.modelCompounds = modelCompounds;
        this.structureReferences = structureReferences;

        isInitialized = true;
    }

    public void SwapTexture(Compound compound)
    {
        if (!isInitialized || compound == null) return;

        foreach (var c in modelCompounds.compounds)
        {
            if (compound == c)
            {
                Indexer++;

                if (Indexer > structureReferences.textureMappings.Length - 1) Indexer = 0;

                SwitchBaseMap(c);
                SwitchNormalMap(c);
                SwitchBaseColor(c);
            }
        }
    }

    private void SwitchBaseMap(Compound compound)
    {
        if (structureReferences.textureMappings[Indexer].baseMap == null) return;

        compound.meshRenderer.material.SetTexture(
            structureReferences.textureMappings[Indexer].baseValue,
            structureReferences.textureMappings[Indexer].baseMap
            );
    }

    private void SwitchNormalMap(Compound compound)
    {
        if (structureReferences.textureMappings[Indexer].normalMap == null) return;

        compound.meshRenderer.material.SetTexture(
            structureReferences.textureMappings[Indexer].normalValue,
            structureReferences.textureMappings[Indexer].normalMap
            );
    }

    private void SwitchBaseColor(Compound compound)
    {
        compound.meshRenderer.material.SetColor(
            structureReferences.textureMappings[Indexer].colorValue,
            structureReferences.textureMappings[Indexer].baseColor
            );
    }
}
