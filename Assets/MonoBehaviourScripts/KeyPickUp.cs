using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KeyPickup : MonoBehaviour
{
    [Header("Audio")]
    [Tooltip("Sound to play when the key is picked up")]
    public AudioClip pickupSound;

    private AudioSource audioSource;
    private bool isPickedUp = false;   // prevents multiple pickups if player stays in trigger

    private void Awake()
    {
        // Re-use an AudioSource on the object or add one if missing
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f; // make it 3D so sound comes from world position
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPickedUp) return;
        if (!other.CompareTag("Player")) return;

        InventoryManager inventory = other.GetComponent<InventoryManager>();
        if (inventory == null)
        {
            inventory = FindObjectOfType<InventoryManager>();
        }

        if (inventory != null)
        {
            inventory.AddKey();
            PlayPickupSound();
            isPickedUp = true;
            Destroy(gameObject, pickupSound != null ? pickupSound.length : 0f); // wait for sound
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
