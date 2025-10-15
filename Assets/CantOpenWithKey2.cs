using UnityEngine;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class CantOpenWithKey2 : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI interactionText;

    private Transform player;

    private bool isInRange = false;

    private bool hasInteracted = false;


    void Start()
    {

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;


        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
    }

    void Update()
    {

        if (isInRange && !hasInteracted && Input.GetKeyDown(KeyCode.E))
        {
            hasInteracted = true;
            if (interactionText != null)
            {
                interactionText.text = "Door Runs With Alternative Power Source!";
                CancelInvoke();
                Invoke(nameof(HideInteractionText), 3f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
            if (!hasInteracted && interactionText != null)
            {
                interactionText.text = "Door Runs with an Alternative Power Source";
                interactionText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
            if (interactionText != null)
                interactionText.gameObject.SetActive(false);
        }
    }

    private void HideInteractionText()
    {
        if (interactionText != null)
            interactionText.gameObject.SetActive(false);
    }
}
