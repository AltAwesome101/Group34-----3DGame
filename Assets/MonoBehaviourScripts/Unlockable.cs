using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class UnlockableObject : MonoBehaviour
{
    public TextMeshProUGUI promptText;
    private bool isPlayerNearby = false;
    private InventoryManager inventory;

    void Start()
    {
        if (promptText != null)
            promptText.text = "";
    }

    void Update()
    {
        if (isPlayerNearby && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (inventory != null && inventory.keys > 0)
            {
                inventory.UseKey();
                promptText.text = "";
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerNearby = true;
        inventory = other.GetComponent<InventoryManager>();
        if (inventory == null)
            inventory = FindObjectOfType<InventoryManager>();

        if (promptText != null)
            promptText.text = "Press E to unlock";
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerNearby = false;
        if (promptText != null)
            promptText.text = "";
    }
}