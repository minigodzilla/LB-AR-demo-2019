using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Kapyong
{
    public class Launcher : MonoBehaviour
    {
        [SerializeField] private Instruction instruction = new Instruction();
        [SerializeField] private Text buildText = default;

        [SerializeField] private Button arButton = default;
        [SerializeField] private Button infoButton = default;
        [SerializeField] private Button tourButton = default;
        [SerializeField] private Button feedbackButton = default;
        [SerializeField] private Button InfoReturnButton = default;
        [SerializeField] private Button feedbackReturnButton = default;
        [SerializeField] private Button emailButton = default;
        [SerializeField] private Button websiteButton = default;


        [SerializeField] private Animation launcher = default;
        [SerializeField] private Animation mainPanel = default;
        [SerializeField] private Animation infoPanel = default;
        [SerializeField] private Animation feedbackPanel = default;
        [SerializeField] private Animation instructionPanel = default;
        [SerializeField] private Animation logoPanel = default;

        [SerializeField] private VideoPlayer videoPlayer = default;

        private const float eventDelay = 4.2f;
        private const float eventGap = 0.5f;

        private const string arScene = "Kapyong_AR";
        private const string tourScene = "Kapyong_Scrollytelling";
        private const string buildFormat = "BUILD VERSION: {0}";
        private const string tweenIn = "CanvasGroupIn";
        private const string tweenOut = "CanvasGroupOut";

        private const string emailSubject = "Feedback";

        private const string email = "inquiries@treatyno1.ca";
        private const string website = "https://www.treaty1.ca";

        private void Start()
        {
            instruction.Initialize(LoadARScene);

            StartCoroutine(Startup());

            AssignRuntimeEvents();
            InitializeBuildVersion();
            tourButton.onClick.AddListener(LoadTourScene);
        }

        private void LoadARScene() => SceneManager.LoadScene(arScene);
        private void LoadTourScene() => SceneManager.LoadScene(tourScene);
        private void OpenURL(string url) => Application.OpenURL(url);

        private void InitializeBuildVersion() => buildText.text = string.Format(buildFormat, Application.version);

        private void AssignRuntimeEvents()
        {
            arButton.onClick.AddListener(InitializeFeaturePanel);

            infoButton.onClick.AddListener(() => ShowAppInfoPanel(true));
            InfoReturnButton.onClick.AddListener(() => ShowAppInfoPanel(false));

            feedbackButton.onClick.AddListener(() => ShowFeedbackPanel(true));
            feedbackReturnButton.onClick.AddListener(() => ShowFeedbackPanel(false));

            websiteButton.onClick.AddListener(() => OpenURL(website));
            emailButton.onClick.AddListener(() => SendEmail(email));
        }

        private void ShowAppInfoPanel(bool show)
        {
            infoPanel.Play(TweenCurve(show));
            mainPanel.Play(TweenCurve(!show));
        }

        private void ShowFeedbackPanel(bool show)
        {
            mainPanel.Play(TweenCurve(!show));
            feedbackPanel.Play(TweenCurve(show));
        }

        private void InitializeFeaturePanel()
        {
            mainPanel.Play(TweenCurve(false));
            instructionPanel.Play(TweenCurve(true));
        }

        private string TweenCurve(bool show) => show ? tweenIn : tweenOut;

        private void SendEmail(string email)
        {
            string subject = MyEscapeURL(emailSubject);
            Application.OpenURL("mailto:" + email + "?subject=" + subject);
        }

        private string MyEscapeURL(string url) => WWW.EscapeURL(url).Replace("+", "%20");

        private IEnumerator Startup()
        {
            if (Home.Resources.WasFirstLaunchExecuted)
            {
                videoPlayer.gameObject.SetActive(false);
                logoPanel.gameObject.SetActive(false);
                launcher.Play(TweenCurve(true));
            }
            else
            {
                Home.Resources.FirstLaunchExecuted();
                videoPlayer.Play();

                yield return new WaitForSeconds(eventDelay);
                logoPanel.Play(TweenCurve(false));

                yield return new WaitForSeconds(eventGap);
                launcher.Play(TweenCurve(true));
            }
        }
    }
}
