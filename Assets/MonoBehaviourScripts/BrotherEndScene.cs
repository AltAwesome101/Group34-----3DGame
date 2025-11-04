using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class BrotherEndSequence : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI promptText;
    public TextMeshProUGUI endText;
    private Animator broAnimator;

    private bool playerNearby;

    private bool messageShown;

    void Start()
    {
        if (promptText)
        {
            promptText.gameObject.SetActive(false);
        }
        if (endText)
        {
            endText.gameObject.SetActive(false);
        }

        broAnimator = GetComponent<Animator>();

    }

    void Update()
    {
        if (playerNearby && !messageShown && Keyboard.current.eKey.wasPressedThisFrame)
        {
            triggerAnimation();
            ShowEndMessage();
        }
    }

    void ShowEndMessage()
    {
        messageShown = true;
        if (promptText) promptText.gameObject.SetActive(false);
        if (endText)
        {
            endText.text = "Congratulations you reached the end of Submission 3";
            endText.gameObject.SetActive(true);
        }
    }

    void triggerAnimation()
    {
        if(broAnimator != null)
        {
            broAnimator.SetTrigger("ScenePlay");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!messageShown && other.CompareTag("Player"))
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
            if (promptText) promptText.gameObject.SetActive(false);
        }
    }
}
