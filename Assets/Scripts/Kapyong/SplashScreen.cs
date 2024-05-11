using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer = default;

    private const float eventDelay = 4.2f;
    private const int nextSceneIndex = 1;

    public void Start()
    {
        //videoPlayer.Play();
        StartCoroutine(OnLoadMainMenu());
    }

    private IEnumerator OnLoadMainMenu()
    {
        yield return new WaitForSeconds(eventDelay);
        LoadMainMenu();
    }

    private void LoadMainMenu() => SceneManager.LoadScene(nextSceneIndex);
}
