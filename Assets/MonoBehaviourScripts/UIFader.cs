using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UIFader : MonoBehaviour
{
    public float fadeDuration = 0.45f;
    private CanvasGroup cg;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        if (cg == null) 
        { 
            cg = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void FadeIn()
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(FadeCoroutine(1f));
    }

    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeCoroutine(0f, () => gameObject.SetActive(false)));
    }

    private IEnumerator FadeCoroutine(float targetAlpha, System.Action onComplete = null)
    {
        float start = cg.alpha;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, targetAlpha, t / fadeDuration);
            yield return null;
        }
        cg.alpha = targetAlpha;
        onComplete?.Invoke();
    }
}
