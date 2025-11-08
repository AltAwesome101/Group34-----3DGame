using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class BrotherEndSequence : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI promptText;
    public TextMeshProUGUI endText;
    public Animator broAnimator;

    [Header("Audio")]
    public AudioClip endSound;  
    private AudioSource audioSource;

    private bool playerNearby;
    private bool messageShown;

    void Start()
    {

        if (promptText) promptText.gameObject.SetActive(false);
        if (endText) endText.gameObject.SetActive(false);

        broAnimator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (playerNearby && !messageShown && Keyboard.current.eKey.wasPressedThisFrame)
        {
            ShowEndMessage();
        }
    }

    void ShowEndMessage()
    {
        messageShown = true;

        if (promptText)
            promptText.gameObject.SetActive(false);

        if (endText)
        {
            triggerAnimation();
            endText.text = "Congratulations! You reached the end of Dawn Of Darkness!";
            endText.gameObject.SetActive(true);
        }

        if (endSound && audioSource)
            audioSource.PlayOneShot(endSound);
    }

    void triggerAnimation()
    {
        if (broAnimator)
            broAnimator.SetTrigger("ScenePlay");
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
            if (promptText)
                promptText.gameObject.SetActive(false);
        }
    }
}
