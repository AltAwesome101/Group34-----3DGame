//Title: Pressure Puzzle
//Author: Lain Couper
//Date: 24-10-2014
//Code Version: New-input System
//Availability: https://www.reddit.com/r/Unity3D/comments/tgze94/puzzle_hub_all_made_in_unitys_ui_system/

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

    private bool isPressed = false;

    private AudioSource audioSource;

    private int pushableLayer;

    public bool IsPressed => isPressed;

    private void Awake()
    {
      
        pushableLayer = LayerMask.NameToLayer("Pushable");

       
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

    private bool IsRelevantCollider(Collider c)
    {
        return c.gameObject.layer == pushableLayer;
    }

    private bool HasAnyRelevantOverlap()
    {
        Collider plateCollider = GetComponent<Collider>();
        Collider[] overlaps = Physics.OverlapBox(
            plateCollider.bounds.center,
            plateCollider.bounds.extents,
            transform.rotation,
            1 << pushableLayer, 
            QueryTriggerInteraction.Collide
        );

        return overlaps.Length > 0;
    }

    private void PlayPressFeedback()
    {
        UpdateVisual();
        if (pressVFX != null) pressVFX.Play();
        if (pressSFX != null && audioSource != null)
            audioSource.PlayOneShot(pressSFX, audioVolume);
    }

    private void PlayReleaseFeedback()
    {
        UpdateVisual();
        if (releaseSFX != null && audioSource != null)
            audioSource.PlayOneShot(releaseSFX, audioVolume);
    }

    private void UpdateVisual()
    {
        if (plateRenderer != null)
        {
            plateRenderer.material = isPressed && pressedMaterial ? pressedMaterial : idleMaterial;
        }
    }
}
