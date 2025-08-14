using UnityEngine;
using UnityEngine.InputSystem;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Pickup Settings")]
    public Transform holdPoint;
    public Transform cameraTransform;
    public float pickupRange = 3f;

    private Rigidbody rb;
    private NewMonoBehaviourScript heldObject; // Holds the currently picked-up object

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Pickup(Transform holdPoint)
    {
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
    }

    public void Drop()
    {
        rb.useGravity = true;
        transform.SetParent(null);
    }

    public void MoveToHoldPoint(Vector3 targetPosition)
    {
        rb.MovePosition(targetPosition);
    }

    public void OnPickUP(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (heldObject == null)
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
            {
                NewMonoBehaviourScript pickUp = hit.collider.GetComponent<NewMonoBehaviourScript>();
                if (pickUp != null)
                {
                    pickUp.Pickup(holdPoint);
                    heldObject = pickUp;
                }
            }
        }
        else
        {
            heldObject.Drop();
            heldObject = null;
        }
    }

    private void Update()
    {
        if (heldObject != null)
        {
            heldObject.MoveToHoldPoint(holdPoint.position);
        }
    }
}