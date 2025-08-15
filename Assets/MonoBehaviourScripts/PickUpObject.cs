using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PickableObject : MonoBehaviour
{
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Pickup(Transform holdPoint)
    {
        if (rb == null) return;

        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
    }

    public void Drop()
    {
        if (rb == null) return;

        rb.useGravity = true;
        transform.SetParent(null);
    }

    public void Throw(Vector3 force)
    {
        Drop();
        if (rb != null)
        {
            rb.AddForce(force, ForceMode.Impulse);
        }
    }

    public void MoveToHoldPoint(Vector3 targetPosition)
    {
        if (rb != null)
        {
            rb.MovePosition(targetPosition);
        }
    }
}
