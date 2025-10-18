using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider), typeof(AudioSource))]
public class CandleInteractable : MonoBehaviour
{
    [Header("Candle ID (unique)")]
    public int candleID;

    [Header("Visuals")]
    public Light candleLight;              
    public ParticleSystem flameFX;         
    public Renderer targetRenderer;        
    public Material unlitMaterial;
    public Material litMaterial;

    [Header("Audio")]
    public AudioClip lightSound;           
    [Tooltip("Optional world-space prompt (TextMeshPro) to show/hide")]
    public TextMeshProUGUI promptText;

  
    private bool playerNearby = false;

    private bool isLit = false;

    private AudioSource audioSource;

    private PlayerInputActions controls;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource)
        {
            audioSource.playOnAwake = false;
        }
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }

        controls = new PlayerInputActions();

        controls.Player.Interact.performed += OnInteract;

        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
        ApplyLitState(false, playFX: false);
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = true;
        if (!isLit && promptText != null)
        {
            promptText.text = "Press [E] to light";
            promptText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = false;
        if (promptText != null) promptText.gameObject.SetActive(false);
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!playerNearby)
        {
            return;
        }
        if (isLit)
        {
            return;
        }

        var manager = FindObjectOfType<CandlePuzzleManager>();

        if (manager != null)
        {
            manager.TryLight(this);
        }
        else
        {
            ApplyLitState(true, playFX: true);
        }
    }

    public void ApplyLitState(bool lit, bool playFX = true)
    {
        isLit = lit;

        if (flameFX != null)
        {
            if (lit)
            {
                if (playFX) flameFX.Play();
            }
            else
            {
                flameFX.Stop();
                flameFX.Clear();
            }
        }

        if (candleLight != null) candleLight.enabled = lit;

        if (targetRenderer != null)
        {
            if (lit && litMaterial != null) targetRenderer.material = litMaterial;
            else if (!lit && unlitMaterial != null) targetRenderer.material = unlitMaterial;
        }

        if (playFX && lit && lightSound != null && audioSource != null)
            audioSource.PlayOneShot(lightSound);

        if (promptText != null)
            promptText.gameObject.SetActive(!lit);
    }

    public bool IsLit => isLit;
}
