using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HealthPickup : MonoBehaviour
{
    [Header("Health Settings")]
    public int healAmount = 10;

    [Header("Rotation Settings")]
    [Tooltip("How fast the health pack rotates (degrees per second)")]
    public float rotationSpeed = 90f;

    [Header("Audio")]
    [Tooltip("Sound to play when picked up")]
    public AudioClip pickupSound;

    private AudioSource audioSource;
    private bool isCollected = false;

    private Transform visualChild; 

    private void Awake()
    {
       
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;

        
        if (transform.childCount > 0)
        {
            visualChild = transform.GetChild(0);
        }
        else
        {
            
            GameObject child = new GameObject("Visuals");
            child.transform.SetParent(transform);
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.identity;
            child.transform.localScale = Vector3.one;
            visualChild = child.transform;
        }
    }

    private void Update()
    {
        
        if (!isCollected && visualChild != null)
        {
            visualChild.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.Self);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected) return;
        if (!other.CompareTag("Player")) return;

        DamagePlayer playerHealth = other.GetComponent<DamagePlayer>();
        if (playerHealth != null && playerHealth.health < playerHealth.maxHealth)
        {
            playerHealth.AddHealth(healAmount);
            PlayPickupSound();
            isCollected = true;

            
            if (visualChild != null) visualChild.gameObject.SetActive(false);

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
