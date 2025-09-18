using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI Sliders")]
    public Slider sensitivitySlider;
    public Slider headBobSlider;

    [Header("Target")]
    public LookScript lookScript; 

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
    }
}
