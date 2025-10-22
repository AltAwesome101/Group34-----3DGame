using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]
public class PressurePlate : MonoBehaviour
{
    public event Action<PressurePlate, bool> OnPlateStateChanged;

    [Header("Plate Visuals")]
    public Renderer plateRenderer;      
    public Material idleMaterial;
    public Material pressedMaterial;

    [Header("Feedback")]
    public ParticleSystem pressVFX;         
    public AudioClip pressSFX;
    public AudioClip releaseSFX;
    [Range(0f, 1f)] public float audioVolume = 1f;

    bool isPressed = false;
    AudioSource audioSource;

    public bool IsPressed => isPressed;

    private void Awake()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true; 

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;
    }

    private void Start()
    {
        UpdateVisual();
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (IsRelevantCollider(other))
        {
            if (!isPressed)
            {
                isPressed = true;
                OnPlateStateChanged?.Invoke(this, true);
                PlayPressFeedback();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsRelevantCollider(other))
        {
           
            if (!HasAnyRelevantOverlap())
            {
                if (isPressed)
                {
                    isPressed = false;
                    OnPlateStateChanged?.Invoke(this, false);
                    PlayReleaseFeedback();
                }
            }
        }
    }

    bool IsRelevantCollider(Collider c)
    {
        if (c.CompareTag("Player")) return true;
        if (c.attachedRigidbody != null) return true; 
        return false;
    }

    bool HasAnyRelevantOverlap()
    {
        Collider[] overlaps = Physics.OverlapBox(transform.position + GetComponent<Collider>().bounds.center - transform.position,
                                                 GetComponent<Collider>().bounds.extents,
                                                 transform.rotation,
                                                 ~0,
                                                 QueryTriggerInteraction.Collide);

        foreach (var col in overlaps)
        {
            if (IsRelevantCollider(col)) return true;
        }
        return false;
    }

    void PlayPressFeedback()
    {
        UpdateVisual();
        if (pressVFX != null) pressVFX.Play();
        if (pressSFX != null && audioSource != null) audioSource.PlayOneShot(pressSFX, audioVolume);
    }

    void PlayReleaseFeedback()
    {
        UpdateVisual();
        if (releaseSFX != null && audioSource != null) audioSource.PlayOneShot(releaseSFX, audioVolume);
    }

    void UpdateVisual()
    {
        if (plateRenderer != null)
        {
            plateRenderer.material = isPressed && pressedMaterial ? pressedMaterial : idleMaterial;
        }
    }
}
