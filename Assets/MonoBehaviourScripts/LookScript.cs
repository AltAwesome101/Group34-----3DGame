using UnityEngine;

public class LookScript : MonoBehaviour
{
    private GameObject Player;
    private float minClamp = -45;
    private float maxClamp = 45;
    public Vector2 rotation;
    private Vector2 currentLookRot;
    private Vector2 rotationV = new Vector2(0, 0);
    public float lookSensitivity = 2f;
    public float lookSmoothDamp = 0.1f;
    public float bobFrequency = 8f;
    public float bobAmplitude = 0.05f;
    private float bobTimer = 0f;
    private Vector3 startLocalPos;
    private Rigidbody playerRb;

    void Start()
    {
        Player = transform.parent.gameObject;
        startLocalPos = transform.localPosition;
        playerRb = Player.GetComponent<Rigidbody>();
    }

    void Update()
    {
        rotation.y += Input.GetAxis("Mouse Y") * lookSensitivity;
        rotation.y = Mathf.Clamp(rotation.y, minClamp, maxClamp);
        Player.transform.RotateAround(transform.position, Vector3.up, Input.GetAxis("Mouse X") * lookSensitivity);
        currentLookRot.y = Mathf.SmoothDamp(currentLookRot.y, rotation.y, ref rotationV.y, lookSmoothDamp);
        transform.localEulerAngles = new Vector3(-currentLookRot.y, 0, 0);

        HandleHeadBobbing();
    }

    void HandleHeadBobbing()
    {
        // Use input to detect movement
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");
        bool isMoving = (inputX != 0 || inputZ != 0);
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
}