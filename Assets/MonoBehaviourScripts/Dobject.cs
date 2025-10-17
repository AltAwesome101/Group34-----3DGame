using UnityEngine;

public class Destroyable : MonoBehaviour
{
    [Header("Destruction Settings")]
    public int maxHits = 3;
    private int currentHits = 0;

    [Header("Effects")]
    public ParticleSystem destructionEffect; 
    public AudioClip breakSound;             
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void RegisterHit()
    {
        currentHits++;

        if (currentHits >= maxHits)
        {
            BreakObject();
        }
    }

    void BreakObject()
    {
        if (destructionEffect != null)
        {
            ParticleSystem effect = Instantiate(destructionEffect, transform.position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, effect.main.duration + effect.main.startLifetime.constantMax);
        }

       
        if (breakSound != null)
            audioSource.PlayOneShot(breakSound);

        Destroy(gameObject);
    }
}
