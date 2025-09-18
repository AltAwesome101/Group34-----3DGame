using UnityEngine;
using UnityEngine.UI;

public class GeneratorHoldUI : MonoBehaviour
{
    [Header("UI References")]
    public Slider holdSlider;

    public void SetProgress(float value)
    {
        if (holdSlider != null)
            holdSlider.value = Mathf.Clamp01(value);
    }

    public void Show(bool show) => gameObject.SetActive(show);
}
