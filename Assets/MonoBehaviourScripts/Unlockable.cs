using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class UnlockableDoor : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI promptText;

    [Header("Door Settings")]
    public float openAngle = 90f;    // total rotation in degrees
    public float openSpeed = 90f;    // degrees per second

    [Header("Artificial Hinge")]
    public Vector3 hingeOffset = new Vector3(-2.0f, 0f, -0.4f);
    // Relative to door center; left edge = -0.5 if width = 1

    private bool isPlayerNearby;
    private bool isOpen;
    private InventoryManager inventory;

    private float currentAngle = 0f;
    private Vector3 hingePoint;

    private void Start()
    {
        // Calculate world position of hinge
        hingePoint = transform.position + transform.TransformVector(hingeOffset);

        if (promptText) promptText.text = "";
    }

    private void Update()
    {
        if (isPlayerNearby && !isOpen && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (inventory != null && inventory.keys > 0)
            {
                inventory.UseKey();
                promptText.text = "";
                isOpen = true;
            }
        }

        // Animate door rotation around hinge
        if (isOpen && currentAngle < openAngle)
        {
            float delta = openSpeed * Time.deltaTime;
            if (currentAngle + delta > openAngle)
                delta = openAngle - currentAngle;

            transform.RotateAround(hingePoint, Vector3.up, delta);
            currentAngle += delta;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerNearby = true;
        inventory = other.GetComponent<InventoryManager>() ??
                    FindObjectOfType<InventoryManager>();

        if (!isOpen && promptText != null)
            promptText.text = "Press [E] to unlock";
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerNearby = false;
        if (promptText != null) promptText.text = "";
    }
}
