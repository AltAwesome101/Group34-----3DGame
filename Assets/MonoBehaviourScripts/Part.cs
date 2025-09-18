using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(Collider))]
public class PartItem : MonoBehaviour
{
    public int partID;

    [Header("UI / Feedback")]
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private GameObject pickupVFXPrefab;

    private PlayerInputActions controls;

    private PlayerInventory nearbyInventory;

    private AudioSource audioSource;

    private bool playerNearby;

    private void Awake()
    {
        controls = new PlayerInputActions();
        audioSource = GetComponent<AudioSource>();
        HidePrompt();
    }

    private void OnEnable()
    {
        controls.Player.Interact.performed += OnInteract;
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Interact.performed -= OnInteract;
        controls.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        nearbyInventory = other.GetComponent<PlayerInventory>();
        if (nearbyInventory == null) return;

        playerNearby = true;
        ShowPrompt("Press E to pick up part");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = false;
        nearbyInventory = null;
        HidePrompt();
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!playerNearby || nearbyInventory == null) return;

        if (!nearbyInventory.AddPart(partID))
        {
            ShowPrompt("You’re already holding a part, Install it on the Generator then come back");
            return;
        }

        if (audioSource && pickupSound) audioSource.PlayOneShot(pickupSound);
        if (pickupVFXPrefab) Instantiate(pickupVFXPrefab, transform.position, Quaternion.identity);

        HidePrompt();
        gameObject.SetActive(false);
    }

    private void ShowPrompt(string message)
    {
        if (promptText == null) return;
        promptText.text = message;
        promptText.enabled = true;
    }

    private void HidePrompt()
    {
        if (promptText == null) return;
        promptText.enabled = false;
    }
}
