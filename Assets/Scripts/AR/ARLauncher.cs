using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ARLauncher : MonoBehaviour
{
    [SerializeField] private Button DemoTestButton = default, ShowroomAButton = default,
        ModelSwappingButton = default, TextureSwappingButton = default, MeshPrintingOneShotButton = default,
        MeshPrintingStagedButton = default, ShowroomBButton = default;

    [SerializeField] private Text buildText = default;

    private const string buildFormat = "BUILD VERSION: {0}";
    private const string DemoScene = "DemoScene";
    private const string ShowroomA = "ShowroomA";
    private const string ShowroomB = "ShowroomB";
    private const string ModelSwapping = "ModelSwapping";
    private const string TextureSwapping = "TextureSwapping";
    private const string MeshPrintingOneShot = "3DPrintingOneShot";
    private const string MeshPrintingStaged = "3DPrintingStaged";

    public void Awake()
    {
        DemoTestButton.onClick.AddListener(() => LoadScene(DemoScene));
        ShowroomAButton.onClick.AddListener(() => LoadScene(ShowroomA));
        ShowroomBButton.onClick.AddListener(() => LoadScene(ShowroomB));
        ModelSwappingButton.onClick.AddListener(() => LoadScene(ModelSwapping));
        TextureSwappingButton.onClick.AddListener(() => LoadScene(TextureSwapping));
        MeshPrintingStagedButton.onClick.AddListener(() => LoadScene(MeshPrintingStaged));
        MeshPrintingOneShotButton.onClick.AddListener(() => LoadScene(MeshPrintingOneShot));

        buildText.text = string.Format(buildFormat, Application.version);
    }

    private void LoadScene(string scene) => SceneManager.LoadScene(scene);
}
