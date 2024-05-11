using UnityEngine;

public class ARTerrain : MonoBehaviour
{
    [SerializeField] private Transform terrain = default;
    [SerializeField] private TerrainData terrainData = default;
    [SerializeField] private Transform root = default;

    private const float sizeOffset = 100f;
    private const float heightOffset = 1.33f;

    private void Update()
    {
        UpdateTerrainData();
    }

    private void UpdateTerrainData()
    {
        Vector3 size = new Vector3 (
            root.transform.localScale.x,
            root.transform.localScale.y * heightOffset,
            root.transform.localScale.z
            );

        size *= sizeOffset;
        terrainData.size = size;
        terrain.rotation = terrain.rotation;
    }
}
