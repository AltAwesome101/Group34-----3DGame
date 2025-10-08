using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 10;
    public AudioClip pickupSound; 
    private AudioSource audioSource;

    private void Awake()
    {
       
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
            
            InventoryManager inventory = Object.FindFirstObjectByType<InventoryManager>();
            if (inventory != null)
            {
                inventory.AddAmmo(ammoAmount);
            }

            
            if (pickupSound != null)
            {
                audioSource.PlayOneShot(pickupSound);
            }

          
            Destroy(gameObject, pickupSound != null ? pickupSound.length : 0f);
        }
    }
}
