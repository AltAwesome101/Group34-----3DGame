using UnityEngine;
using UnityEngine.InputSystem;

public class LookScript : MonoBehaviour
{
    private GameObject Player;

    [Header("Look Settings")]
    public float minClamp = -45;
    public float maxClamp = 45;
    public float lookSensitivity = 2f;
    public float lookSmoothDamp = 0.1f;

    [Header("Head Bobbing")]
    public float bobFrequency = 8f;
    public float bobAmplitude = 0.05f;

    private Vector2 rotation;
    private Vector2 currentLookRot;
    private Vector2 rotationV = Vector2.zero;

    private float bobTimer = 0f;
    private Vector3 startLocalPos;
    private Rigidbody playerRb;

    private PlayerInputActions inputActions;
    private Vector2 mouseDelta;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.Look.performed += ctx => mouseDelta = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => mouseDelta = Vector2.zero;
    }

    void Start()
    {
        Player = transform.parent.gameObject;
        startLocalPos = transform.localPosition;
        playerRb = Player.GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        ApplyLookRotation();
        HandleHeadBobbing();
    }

    void ApplyLookRotation()
    {
        rotation.y += mouseDelta.y * lookSensitivity;
        rotation.y = Mathf.Clamp(rotation.y, minClamp, maxClamp);

        Player.transform.Rotate(Vector3.up * mouseDelta.x * lookSensitivity);

        currentLookRot.y = Mathf.SmoothDamp(currentLookRot.y, rotation.y, ref rotationV.y, lookSmoothDamp);
        transform.localEulerAngles = new Vector3(-currentLookRot.y, 0, 0);
    }

    void HandleHeadBobbing()
    {
        Vector2 moveInput = inputActions.Player.Movement.ReadValue<Vector2>();
        bool isMoving = moveInput != Vector2.zero;
        bool isGrounded = Physics.Raycast(Player.transform.position, Vector3.down, 1.1f);

        if (isMoving && isGrounded)
        {
            bobTimer += Time.deltaTime * bobFrequency;
            float offsetY = Mathf.Sin(bobTimer) * bobAmplitude;
            transform.localPosition = startLocalPos + new Vector3(0, offsetY, 0);
        }
        else
        {
            bobTimer = 0f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, startLocalPos, Time.deltaTime * 5f);
        }
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
