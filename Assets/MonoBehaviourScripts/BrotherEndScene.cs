using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class BrotherEndSequence : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI promptText;     
    public TextMeshProUGUI endText;        

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
        if (promptText) promptText.gameObject.SetActive(false);
        if (endText)
        {
            endText.text = "Congratulations you reached the end of Submission 2";
            endText.gameObject.SetActive(true);
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
