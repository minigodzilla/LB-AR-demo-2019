using UnityEngine;
using UnityEngine.Rendering;

public class PlatformInfo
{
    public ResolutionTierType resolutionTier = ResolutionTierType.None;
    public RenderPipelineAsset low_RP, medium_RP, high_RP;

    private int low, high;

    public void GetPlatformInfo(SystemConfig sc)
    {
        low = sc.memeroySize.low;
        high = sc.memeroySize.high;

        low_RP = sc.low_RP;
        medium_RP = sc.medium_RP;
        high_RP = sc.high_RP;

        GetSystemInfo();
    }

    private void GetSystemInfo()
    {
        int availableMemory = SystemInfo.systemMemorySize;
        GetTier(availableMemory);
    }

    private void GetTier(int ram)
    {
        if (ram <= low) resolutionTier = ResolutionTierType.Low;
        if (ram > low && ram < high) resolutionTier = ResolutionTierType.Medium;
        if (ram >= high) resolutionTier = ResolutionTierType.High;
    }
}
