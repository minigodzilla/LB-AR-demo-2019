using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ARLauncher : MonoBehaviour
{
    [SerializeField]
    private Button ShowroomAButton = default, ShowroomBButton = default, ShowroomCButton = default;

    [SerializeField] private Text buildText = default;

    private const string buildFormat = "BUILD VERSION: {0}";
    private const string ShowroomA = "ShowroomA";
    private const string ShowroomB = "ShowroomB";
    private const string ShowroomC = "ShowroomC";

    public void Awake()
    {
        ShowroomAButton.onClick.AddListener(() => LoadScene(ShowroomA));
        ShowroomBButton.onClick.AddListener(() => LoadScene(ShowroomB));
        ShowroomCButton.onClick.AddListener(() => LoadScene(ShowroomC));

        buildText.text = string.Format(buildFormat, Application.version);
    }

    private void LoadScene(string scene) => SceneManager.LoadScene(scene);
}
