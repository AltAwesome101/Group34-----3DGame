using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KeyPickup : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("Sound to play when the key is picked up")]
    public AudioClip pickupSound;

    [Header("Rotation Settings")]
    [Tooltip("How fast the key spins in degrees per second")]
    public float rotationSpeed = 90f;  

    private AudioSource audioSource;
    private bool isPickedUp = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
    }

    private void Update()
    {
        
        if (!isPickedUp)
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPickedUp) return;
        if (!other.CompareTag("Player")) return;

        InventoryManager inventory = other.GetComponent<InventoryManager>();
        if (inventory == null)
        {
            inventory = FindFirstObjectByType<InventoryManager>();
        }

        if (inventory != null)
        {
            inventory.AddKey();
            PlayPickupSound();
            isPickedUp = true;
            Destroy(gameObject, pickupSound != null ? pickupSound.length : 0f);
        }
    }

    private void PlayPickupSound()
    {
        if (pickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }
    }
}
