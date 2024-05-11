using UnityEngine;

[DisallowMultipleComponent]
public abstract class Instance<T> : MonoBehaviour where T : Instance<T>
{
    public static T Resources { get; protected set; }

    protected virtual void Awake()
    {
        if (null != Resources)
        {
            DestroyImmediate(gameObject);
            return;
        }

        CreateInstance();
        ApplyAppSettings();
    }

    protected virtual void CreateInstance() => Resources = (T)this;
    protected virtual void ApplyAppSettings() => Application.targetFrameRate = 30;
}
