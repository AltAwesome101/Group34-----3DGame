using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(AudioSource))]
public class MovementScript : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 4f;
    public float sprintMultiplier = 2f;
    public float jump = 4f;
    public float crouchSpeedMultiplier = 0.5f;

    [Header("Crouch Settings")]
    public Vector3 crouchScale = new Vector3(1f, 0.5f, 1f);
    public Vector3 normalScale = new Vector3(1f, 1f, 1f);

    [Header("Collision Audio")]
    public AudioClip collisionSound;
    public float collisionThreshold = 1f; 
    public float capsuleYOffset = 0.2f;   

    private bool isCrouching = false;
    private bool isSprinting = false;

    private Vector2 moveInput;
    private Rigidbody rb;
    private CapsuleCollider col;
    private PlayerInputActions inputActions;
    private AudioSource audioSource;

    private float blockedTime = 0f;
    private bool soundPlayed = false;

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        inputActions.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Movement.canceled += ctx => moveInput = Vector2.zero;
        inputActions.Player.Jump.performed += ctx => TryJump();
        inputActions.Player.Crouch.started += ctx => StartCrouch();
        inputActions.Player.Crouch.canceled += ctx => StopCrouch();
        inputActions.Player.Sprint.started += ctx => isSprinting = true;
        inputActions.Player.Sprint.canceled += ctx => isSprinting = false;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        audioSource = GetComponent<AudioSource>();

        rb.freezeRotation = true; 
        transform.localScale = normalScale;
    }

    private void FixedUpdate()
    {
        float currentSpeed = speed;
        if (isSprinting) currentSpeed *= sprintMultiplier;
        if (isCrouching) currentSpeed *= crouchSpeedMultiplier;

        Vector3 direction = transform.TransformDirection(new Vector3(moveInput.x, 0, moveInput.y));
        Vector3 step = direction * currentSpeed * Time.fixedDeltaTime;

        Vector3 bottom = col.bounds.center - Vector3.up * (col.height / 2 - col.radius) + Vector3.up * capsuleYOffset;
        Vector3 top = col.bounds.center + Vector3.up * (col.height / 2 - col.radius) + Vector3.up * capsuleYOffset;

        float skin = 0.02f;                       
        float radius = col.radius * 0.95f;         

        bool hit = Physics.CapsuleCast(
            top,
            bottom,
            radius,
            step.normalized,
            step.magnitude + skin,
            ~0,
            QueryTriggerInteraction.Ignore);

        if (!hit)
        {
            rb.MovePosition(rb.position + step);
            blockedTime = 0f;
            soundPlayed = false;
        }
        else
        {
            blockedTime += Time.fixedDeltaTime;
            if (!soundPlayed && blockedTime >= collisionThreshold)
            {
                if (collisionSound) audioSource.PlayOneShot(collisionSound);
                soundPlayed = true;
            }
        }
    }


    private void TryJump()
    {
        if (IsGrounded() && !isCrouching)
        {
            rb.AddForce(Vector3.up * jump, ForceMode.Impulse);
        }
    }

    private void StartCrouch()
    {
        isCrouching = true;
        transform.localScale = crouchScale;
    }

    private void StopCrouch()
    {
        isCrouching = false;
        transform.localScale = normalScale;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, col.bounds.extents.y + 0.1f);
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();
}
