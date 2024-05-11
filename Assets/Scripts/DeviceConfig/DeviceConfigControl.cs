using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class DeviceConfigControl : MonoBehaviour, IControl
{
    [SerializeField] private SystemConfig
        systemConfig_Android = default, systemConfig_iOS = default;

    private readonly PlatformInfo info = new PlatformInfo();

    public void Initialize(Home home, Action onInitialized)
    {
        onInitialized?.Invoke();
        OnStart();
    }

    private void OnStart()
    {
        if(IsAndroid()) info.GetPlatformInfo(systemConfig_Android);
        else info.GetPlatformInfo(systemConfig_iOS);

        StartCoroutine(GetTier(info.resolutionTier));
    }

    private IEnumerator GetTier(ResolutionTierType resolutionTier)
    {
        while (resolutionTier.Equals(ResolutionTierType.None))
            yield return null;

        SetTier(resolutionTier);

    }

    private void SetTier(ResolutionTierType resolutionTier)
    {
        switch (resolutionTier)
        {
            case ResolutionTierType.Low:
                Low();
                break;
            case ResolutionTierType.Medium:
                Medium();
                break;
            case ResolutionTierType.High:
                High();
                break;
        }
    }

    private void Low() => GraphicsSettings.renderPipelineAsset = info.low_RP;

    private void Medium() => GraphicsSettings.renderPipelineAsset = info.medium_RP;

    private void High() => GraphicsSettings.renderPipelineAsset = info.high_RP;

    private bool IsAndroid()
    {
    #if UNITY_ANDROID
        return true;
    #else
        return false;
    #endif
    }
}

public enum ResolutionTierType
{
    None,
    High,
    Medium,
    Low
}
