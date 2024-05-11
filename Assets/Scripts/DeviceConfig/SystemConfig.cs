using System;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "_SystemConfig", menuName = "Configuration/System Config")]
public class SystemConfig : ScriptableObject
{
    [Serializable]
    public class MemeroySize
    {
        public int low;
        public int high;
    }

    [Header("System Infromation (Memory Size)")]
    public MemeroySize memeroySize = new MemeroySize();

    [Header("Universal Render Pipeline")]
    public RenderPipelineAsset low_RP;
    public RenderPipelineAsset medium_RP;
    public RenderPipelineAsset high_RP;
}
