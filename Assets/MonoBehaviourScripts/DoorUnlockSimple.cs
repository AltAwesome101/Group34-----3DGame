using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider))]
public class DoorUnlockSimple : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI promptText;

    [Header("Door Settings")]
    public float openAngle = 90f;
    public float openSpeed = 90f;

    [Header("Artificial Hinge")]
    public Vector3 hingeOffset = new Vector3(-2.0f, 0f, -0.4f);

    [Header("Puzzle Link")]
    public bool isPuzzleCompleted = false; 

    private bool isPlayerNearby;
    private bool isOpen;
    private float currentAngle = 0f;
    private Vector3 hingePoint;

    private void Start()
    {
        hingePoint = transform.position + transform.TransformVector(hingeOffset);
        if (promptText) promptText.text = "";
    }

    private void Update()
    {
        
        if (isPuzzleCompleted && !isOpen)
        {
            UnlockDoor();
        }

        
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

        if (!isOpen && promptText != null)
            promptText.text = "Complete the playground puzzle to unlock";
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        isPlayerNearby = false;
        if (promptText != null) promptText.text = "";
    }

    public void UnlockDoor()
    {
        if (isOpen) return;
        isOpen = true;
        if (promptText != null) promptText.text = "";
    }
}
