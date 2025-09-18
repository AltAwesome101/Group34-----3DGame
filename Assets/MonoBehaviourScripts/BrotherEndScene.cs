using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class BrotherEndSequence : MonoBehaviour
{
    [Header("References")]
    public Animator brotherAnimator;
    public string animationTrigger = "PlayAnim";
    public CanvasGroup fadePanel;           // CanvasGroup on the black panel
    public TextMeshProUGUI endText;         // Text to show after animation
    [Space]
    [Header("Prompt")]
    public TextMeshProUGUI promptText;      // <-- Add a TMP text for “Press E”

    [Header("Settings")]
    public float fadeDuration = 2f;
    public float delayBeforeText = 1f;
    public float textFadeDuration = 1f;

    private bool playerNearby;
    private bool sequenceStarted;

    void Start()
    {
        if (fadePanel) fadePanel.alpha = 0;

        if (endText)
        {
            endText.text = "Game over for now till next Submission";
            endText.alpha = 0;
        }

        if (promptText)
            promptText.gameObject.SetActive(false); // Hide at start
    }

    void Update()
    {
        if (playerNearby && !sequenceStarted && Keyboard.current.eKey.wasPressedThisFrame)
        {
            StartCoroutine(PlayEnd());
        }
    }

    IEnumerator PlayEnd()
    {
        sequenceStarted = true;
        if (promptText) promptText.gameObject.SetActive(false); // hide prompt

        MissionManager.Instance.CompleteMission("Find your brother");

        // Fade screen to black
        if (fadePanel)
            yield return StartCoroutine(FadeCanvasGroup(fadePanel, 0, 1, fadeDuration));

        // Trigger brother animation (brother remains visible)
        if (brotherAnimator)
            brotherAnimator.SetTrigger(animationTrigger);

        yield return new WaitForSeconds(delayBeforeText);

        // Fade in the "Game over..." text
        if (endText)
            yield return StartCoroutine(FadeTMP(endText, 0, 1, textFadeDuration));
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        cg.alpha = to;
    }

    IEnumerator FadeTMP(TextMeshProUGUI tmp, float from, float to, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            tmp.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        tmp.alpha = to;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!sequenceStarted && other.CompareTag("Player"))
        {
            playerNearby = true;
            if (promptText)
            {
                promptText.text = "Press E to interact";
                promptText.gameObject.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            if (promptText)
                promptText.gameObject.SetActive(false);
        }
    }
}
