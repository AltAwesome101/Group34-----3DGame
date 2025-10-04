using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Camera")]
    public Transform cameraRoot;     
    public Transform menuPoint;
    public Transform settingsPoint;
    public Transform helpPoint;
    public float cameraSpeed = 2f;

    [Header("UI Faders")]
    public UIFader mainPanelFader;
    public UIFader settingsPanelFader;
    public UIFader helpPanelFader;

    [Header("Screen Fade")]
    public ScreenFader screenFader;   
    public string levelSceneName = "Level2";
    public float sceneFadeDelay = 0.2f;

    private Transform currentTarget;

    void Start()
    {
        currentTarget = menuPoint;
        if (mainPanelFader != null) { mainPanelFader.gameObject.SetActive(true); mainPanelFader.FadeIn(); }
        if (settingsPanelFader != null) settingsPanelFader.gameObject.SetActive(false);
        if (helpPanelFader != null) helpPanelFader.gameObject.SetActive(false);
    }

    void Update()
    {
        if (cameraRoot != null && currentTarget != null)
        {
            cameraRoot.position = Vector3.Lerp(cameraRoot.position, currentTarget.position, Time.deltaTime * cameraSpeed);
            cameraRoot.rotation = Quaternion.Slerp(cameraRoot.rotation, currentTarget.rotation, Time.deltaTime * cameraSpeed);
        }
    }

    public void StartGame()
    {
        if (screenFader != null)
        {
            StartCoroutine(DoFadeAndLoad(levelSceneName));
        }
        else
        {
            SceneManager.LoadScene(levelSceneName);
        }
    }

    public void OpenSettings()
    {
        if (mainPanelFader != null) mainPanelFader.FadeOut();
        if (settingsPanelFader != null)
        {
            settingsPanelFader.gameObject.SetActive(true);
            settingsPanelFader.FadeIn();
        }
        currentTarget = settingsPoint;
    }

    public void OpenHelp()
    {
        if (mainPanelFader != null) mainPanelFader.FadeOut();
        if (helpPanelFader != null)
        {
            helpPanelFader.gameObject.SetActive(true);
            helpPanelFader.FadeIn();
        }
        currentTarget = helpPoint;
    }

    public void BackToMenu()
    {
        if (settingsPanelFader != null) settingsPanelFader.FadeOut();
        if (helpPanelFader != null) helpPanelFader.FadeOut();
        if (mainPanelFader != null) mainPanelFader.FadeIn();
        currentTarget = menuPoint;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator DoFadeAndLoad(string sceneName)
    {
        if (screenFader != null)
        {
            yield return StartCoroutine(screenFader.FadeRoutine(1f)); 
            yield return new WaitForSeconds(sceneFadeDelay);
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
            yield return null;
        }
    }
}
