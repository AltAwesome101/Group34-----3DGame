using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
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

    private bool isCrouching = false;
    private bool isSprinting = false;
    private Vector2 moveInput;

    private Rigidbody rb;
    private CapsuleCollider col;
    private PlayerInputActions inputActions;

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

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        transform.localScale = normalScale;
    }

    void Update()
    {
        float currentSpeed = speed;

        if (isSprinting)
            currentSpeed *= sprintMultiplier;

        if (isCrouching)
            currentSpeed *= crouchSpeedMultiplier;

        Vector3 direction = new Vector3(moveInput.x, 0, moveInput.y);
        transform.Translate(direction * currentSpeed * Time.deltaTime, Space.Self);
    }

    private void TryJump()
    {
        if (isGrounded() && !isCrouching)
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

    private bool isGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, col.bounds.extents.y + 0.1f);
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}
