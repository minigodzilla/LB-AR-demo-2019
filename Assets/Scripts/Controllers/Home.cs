using System;
using UnityEngine;

[DisallowMultipleComponent]
public class Home : Instance<Home>
{
    public InputControl Input { get; private set; }
    public DeviceConfigControl DeviceConfig { get; private set; }
    public bool WasFirstLaunchExecuted { get; private set; }

    public bool AreResourcesReady { get; private set; }

    public event Action<Home> InitializeResources;

    [SerializeField] private GameObject input = null;
    [SerializeField] private GameObject deviceConfig = null;

    private int initializationSteps;

    public void SubscribeInitializer(Action<Home> onInitialized)
    {
        if (AreResourcesReady) onInitialized(this);
        else InitializeResources += onInitialized;
    }

    public bool FirstLaunchExecuted() => WasFirstLaunchExecuted = true;

    protected override void CreateInstance()
    {
        base.CreateInstance();
        InitializeControllers(SpawnControllers());
        DontDestroyOnLoad(gameObject);
    }

    private IControl[] SpawnControllers()
    {
        var spawnedManagers = new IControl[]
        {
            Input = Instantiate(input, transform).GetComponent<InputControl>(),
            DeviceConfig = Instantiate(deviceConfig, transform).GetComponent<DeviceConfigControl>()
        };

        return spawnedManagers;
    }

    private void InitializeControllers(IControl[] controllers)
    {
        initializationSteps = controllers.Length;
        foreach (IControl controller in controllers) controller.Initialize(this, OnControllersInitialized);
    }

    private void OnControllersInitialized()
    {
        --initializationSteps;
        if (initializationSteps == 0)
        {
            AreResourcesReady = true;
            InitializeResources?.Invoke(this);
            InitializeResources = null;
        }
    }
}

public interface IControl { void Initialize(Home home, Action onInitialized); }