using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI Sliders")]
    public Slider sensitivitySlider;
    public Slider headBobSlider;

    [Header("Target")]
    public LookScript lookScript;

    [Header("Navigation")]
    public Button backButton;
    public GameObject previousPanel;

    private void Start()
    {
        float savedSensitivity = PlayerPrefs.GetFloat("LookSensitivity", lookScript.lookSensitivity);
        float savedBob = PlayerPrefs.GetFloat("BobAmplitude", lookScript.bobAmplitude);

        sensitivitySlider.value = savedSensitivity;
        headBobSlider.value = savedBob;

        lookScript.SetLookSensitivity(savedSensitivity);
        lookScript.SetHeadBobAmount(savedBob);

        sensitivitySlider.onValueChanged.AddListener(v =>
        {
            lookScript.SetLookSensitivity(v);
            PlayerPrefs.SetFloat("LookSensitivity", v);
        });

        headBobSlider.onValueChanged.AddListener(v =>
        {
            lookScript.SetHeadBobAmount(v);
            PlayerPrefs.SetFloat("BobAmplitude", v);
        });

       
        if (backButton != null)
            backButton.onClick.AddListener(HandleBackButton);
    }

    private void HandleBackButton()
    {
       
        if (previousPanel != null)
            previousPanel.SetActive(true);

     
        gameObject.SetActive(false);

     
        PlayerPrefs.Save();

    }
}
