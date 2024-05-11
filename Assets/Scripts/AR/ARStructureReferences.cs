using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StructureReferences", menuName = "References/Structure")]
public class ARStructureReferences : ScriptableObject
{
    [Serializable]
    public class ModelSwapping
    {
        public float duration;
        public GameObject[] models;
    }

    [Header("Insert Structure References")]
    public bool isTerrain = false;
    public GameObject legoPrefab;
    public ARModelCompounds structurePrefab;

    [Header("Insert Structure Rendering References")]
    public float minimumSize = 0.1f;
    public float maximumSize = 1f;

    [Header("Insert Model Swapping References")]
    public bool willSwapModels;
    public ModelSwapping modelSwapping;

    [Header("Insert Texture Swapping References")]
    public bool willSwapTextures = false;
    public TextureMapping[] textureMappings;

    [Header("Insert 3D Printing References")]
    public bool willPrintModel;
    public float printDuration;
    public float maxPrintHeight;
    public float minPrintHeight;
    public string printValue = "_CutoffHeight";

    [HideInInspector] public AREditMode editMode = AREditMode.EditPosition;
    [HideInInspector] public List<GameObject> swappableModels = new List<GameObject>();
}