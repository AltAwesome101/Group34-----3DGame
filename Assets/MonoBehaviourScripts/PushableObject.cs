using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class PushableObject : MonoBehaviour
{
    [Header("Push Settings")]
    [Tooltip("Force applied when the player pushes.")]
    public float pushForce = 6f;

    [Tooltip("Cooldown between pushes (seconds).")]
    public float pushCooldown = 0.5f;

    [Header("Optional Audio")]
    public AudioClip pushSound;

    private AudioSource audioSource;
    private Rigidbody rb;
    private float nextPushTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationZ; 
        rb.linearDamping = 3f;

        audioSource = GetComponent<AudioSource>();
        if (!audioSource)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
    }

    
    public void Push(Vector3 direction)
    {
        if (Time.time < nextPushTime) return;

        rb.AddForce(direction.normalized * pushForce, ForceMode.Impulse);
        nextPushTime = Time.time + pushCooldown;

        if (pushSound != null)
            audioSource.PlayOneShot(pushSound);
    }
}
