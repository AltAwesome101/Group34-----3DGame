using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image fadeImage;         
    public float fadeDuration = 1f;

    public static ScreenFader Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (fadeImage == null)
            fadeImage = GetComponentInChildren<Image>();

        if (fadeImage != null)
        {
            fadeImage.color = new Color(0f, 0f, 0f, 1f);
            StartCoroutine(FadeRoutine(0f, fadeDuration));
        }
    }

    public void FadeOut(float duration = -1f) => Fade(1f, duration);
    public void FadeIn(float duration = -1f) => Fade(0f, duration);

    public void Fade(float targetAlpha, float duration = -1f)
    {
        if (fadeImage == null) return;
        if (duration < 0) duration = fadeDuration;
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(targetAlpha, duration));
    }

    public IEnumerator FadeRoutine(float targetAlpha, float duration = -1f)
    {
        if (fadeImage == null) yield break;
        if (duration < 0) duration = fadeDuration;

        float start = fadeImage.color.a;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(start, targetAlpha, t / duration);
            fadeImage.color = new Color(0f, 0f, 0f, a);
            yield return null;
        }
        fadeImage.color = new Color(0f, 0f, 0f, targetAlpha);
    }
}
