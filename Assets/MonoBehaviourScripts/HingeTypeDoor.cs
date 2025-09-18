using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider), typeof(Rigidbody), typeof(HingeJoint))]
public class UnlockableDoorH : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI promptText;

    [Header("Door Settings")]
    public float openAngle = 90f;    
    public float motorForce = 100f;    
    public float motorSpeed = 120f;    

    private bool isPlayerNearby;
    private bool isOpen;
    private InventoryManager inventory;

    private HingeJoint hinge;

    private void Awake()
    {
        hinge = GetComponent<HingeJoint>();
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.mass = 1f;
        JointLimits limits = hinge.limits;
        limits.min = 0f;             
        limits.max = openAngle;      
        hinge.limits = limits;
        hinge.useLimits = true;
        hinge.useMotor = false;
    }

    private void Start()
    {
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
                OpenDoor();
            }
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

    private void OpenDoor()
    {
        isOpen = true;
        JointMotor motor = hinge.motor;
        motor.force = motorForce;
        motor.targetVelocity = motorSpeed; 
        motor.freeSpin = false;
        hinge.motor = motor;
        hinge.useMotor = true;
    }
}
