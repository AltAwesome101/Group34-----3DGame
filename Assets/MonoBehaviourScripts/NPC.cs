using UnityEngine;
using TMPro;
using System.Collections;

public class NPC : MonoBehaviour
{
    public string message = "I wanna be a REAL boy!";
    public GameObject interactPromptUI;
    public TextMeshProUGUI dialogueText;
    private bool playerInRange = false;
    private Coroutine hideCoroutine;

    void Start()
    {
        if (interactPromptUI != null)
        {
            interactPromptUI.SetActive(false);
        }
        if (dialogueText != null)
        {
            dialogueText.text = "";
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
           
            if (dialogueText != null)
            {
                dialogueText.text = message;

                if (hideCoroutine != null)
                {
                    StopCoroutine(hideCoroutine);
                }
                hideCoroutine = StartCoroutine(HideDialogueAfterSeconds(3f));
            }

            if (interactPromptUI != null)
            {
                interactPromptUI.SetActive(false);
            }
        }
    }

    private IEnumerator HideDialogueAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (dialogueText != null)
        {
            dialogueText.text = "";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (interactPromptUI != null)
            {
                interactPromptUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactPromptUI != null)
            { 
                interactPromptUI.SetActive(false); 
            }

            if (dialogueText != null)
            { 
                dialogueText.text = ""; 
            }
        }
    }
}