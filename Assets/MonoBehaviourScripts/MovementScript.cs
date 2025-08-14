using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public float speed = 4f;
    public float sprintMultiplier = 2f;
    public float jump = 4f;
    public float crouchSpeedMultiplier = 0.5f;
    public Vector3 crouchScale = new Vector3(1f, 0.5f, 1f);
    public Vector3 normalScale = new Vector3(1f, 1f, 1f);

    private bool isCrouching = false;

    Rigidbody rb;
    BoxCollider col;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        col = GetComponent<BoxCollider>();
        transform.localScale = normalScale;
    }

    void Update()
    {
        float currentSpeed = speed;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= sprintMultiplier;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = true;
            transform.localScale = crouchScale;
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            isCrouching = false;
            transform.localScale = normalScale;
        }

        if (isCrouching)
        {
            currentSpeed *= crouchSpeedMultiplier;
        }

        float Horizontal = Input.GetAxis("Horizontal") * currentSpeed;
        float Vertical = Input.GetAxis("Vertical") * currentSpeed;
        Horizontal *= Time.deltaTime;
        Vertical *= Time.deltaTime;

        transform.Translate(Horizontal, 0, Vertical);

        if (isGrounded() && Input.GetButtonDown("Jump") && !isCrouching)
        {
            rb.AddForce(Vector3.up * jump, ForceMode.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private bool isGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, col.bounds.extents.y + 0.1f);
    }
}