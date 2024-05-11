using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ARModelCompounds : MonoBehaviour
{
    [Serializable]
    public class PrintableGroup
    {
        public Material[] materials;
    }

    [Serializable]
    public class ColorCoding
    {
        public int Index;
        public Material material;
    }

    [Header("3D Texture Swapping")]
    public Collider mainCollider;
    public Compound[] compounds;
    public PrintableGroup[] printables;

    [Header("Color Coding")]
    public ColorCoding[] colorCodings;
    public MeshRenderer colorCodedRenderer;

    [Header("Spawn Conditions")]
    public bool followCameraForward;

    [HideInInspector] public Material[] originalMaterails;
    [HideInInspector] public Material[] colorCodedMaterials;

    private bool willPrintModel;

    public readonly List<PrintableGroup> printableGroups = new List<PrintableGroup>();

    public void PrepareColorCoding()
    {
        if (!WillColorCode()) return;

        originalMaterails = colorCodedRenderer.materials;

        List<Material> m = new List<Material>();

        for (int i = 0; i < originalMaterails.Length; i++) m.Add(originalMaterails[i]);

        for (int i = 0; i < originalMaterails.Length; i++)
        {
            foreach (var item in colorCodings)
            {
                if (i == item.Index) m[i] = item.material;
            }
        }

        colorCodedMaterials = m.ToArray();

        m.Clear();
    }

    public Compound TargetCompound(GameObject group)
    {
        Compound c = null;

        foreach (var compound in compounds)
        {
            if (group == compound.group) c = compound;
        }

        return c;
    }

    public void EnableTextureMapping(bool enable)
    {
        foreach (var compound in compounds)
        {
            compound.collider.TrySetEnableCollider(enable);
        }
    }

    public void EnablePrinting() => willPrintModel = true;

    public bool WillPrintModel() => willPrintModel;
    public bool WillColorCode() => colorCodings.Length > 0;

    private void OnDestroy()
    {
        printableGroups.Clear();
    }
}