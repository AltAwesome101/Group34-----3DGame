using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 10;
    public AudioClip pickupSound; // Assign in Inspector
    private AudioSource audioSource;

    private void Awake()
    {
        // Add an AudioSource component if it doesn't exist
        audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Use FindFirstObjectByType instead of the obsolete method
            InventoryManager inventory = Object.FindFirstObjectByType<InventoryManager>();
            if (inventory != null)
            {
                inventory.AddAmmo(ammoAmount);
            }

            // Play pickup sound
            if (pickupSound != null)
            {
                audioSource.PlayOneShot(pickupSound);
            }

            // Destroy the object after the sound plays
            Destroy(gameObject, pickupSound != null ? pickupSound.length : 0f);
        }
    }
}
