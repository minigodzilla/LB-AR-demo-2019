using UnityEngine;
using UnityEngine.UI;

public class DebugTools : MonoBehaviour
{
    [SerializeField] private Canvas HUD = default;
    [SerializeField] private Button hideButton = default;

    private void Start()
    {
        hideButton.onClick.AddListener(OnHideCanvas);
    }

    private void OnHideCanvas() => HUD.enabled = !HUD.enabled;
}
