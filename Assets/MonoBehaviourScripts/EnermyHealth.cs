using UnityEngine;

public class EnermyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    private int hitNumber = 0;
    private bool isDead = false;
    private bool lastHitWasMelee = false;

    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip deathSound;
    private AudioSource audioSource;

    private void Awake()
    {
       
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.spatialBlend = 1f; 
        audioSource.playOnAwake = false;
    }

    private void OnEnable()
    {
        hitNumber = 0;
        isDead = false;
    }

    public void TakeDamage(int amount, bool isMelee = false)
    {
        if (isDead) return;

        hitNumber += amount;
        lastHitWasMelee = isMelee;

        
        if (hitSound != null)
            audioSource.PlayOneShot(hitSound);

        if (hitNumber >= 3)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        
        if (deathSound != null)
            audioSource.PlayOneShot(deathSound);

        
        if (lastHitWasMelee)
            FindFirstObjectByType<NPC>()?.RegisterMeleeKill();

        
        float delay = deathSound != null ? deathSound.length : 0f;
        Invoke(nameof(DisableAfterDeath), delay);
    }

    private void DisableAfterDeath()
    {
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("bullet"))
        {
            TakeDamage(1, false);
        }
    }
}
