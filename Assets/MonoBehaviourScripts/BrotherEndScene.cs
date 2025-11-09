//Title: Memorable game endings
//Author: Reed Rothchild
//Date: 18-02-2017
//Code Version: New-input System
//Availability:https://www.videogamesage.com/forums/topic/4727-memorable-game-endings/

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

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
            endText.text = "Congratulations! You reached the end of Drawn Of Darkness!";
            endText.gameObject.SetActive(true);
        }

        if (endSound && audioSource)
            audioSource.PlayOneShot(endSound);

        StartCoroutine(ReturnToMainMenuAfterDelay(5f));
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

    private IEnumerator ReturnToMainMenuAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        yield return null;
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

}
