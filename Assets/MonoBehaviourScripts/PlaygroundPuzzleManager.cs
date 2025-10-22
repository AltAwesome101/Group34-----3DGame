using UnityEngine;
using System.Linq;
using TMPro;

public class PlaygroundPuzzleManager : MonoBehaviour
{
    [Header("Plates")]
    public PressurePlate[] plates;

    [Header("Door")]
    public DoorUnlockSimple doorToUnlock;

    [Header("Feedback")]
    public AudioClip puzzleSolvedSFX;
    public ParticleSystem solvedVFX;
    public float solvedSFXVolume = 1f;

    [Header("UI Hint")]
    public TextMeshProUGUI hintText;      
    public string solvedMessage = "All plates activated!";
    public float messageDuration = 3f;

    private AudioSource audioSource;
    private bool solved = false;
    private float messageTimer = 0f;

    private void Awake()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        if (plates == null || plates.Length == 0)
            Debug.LogWarning("PlaygroundPuzzleManager: No plates assigned.");

        foreach (var p in plates)
        {
            p.OnPlateStateChanged += OnPlateChanged;
        }

        if (hintText != null)
            hintText.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        foreach (var p in plates)
            p.OnPlateStateChanged -= OnPlateChanged;
    }

    private void Update()
    {
       
        if (messageTimer > 0)
        {
            messageTimer -= Time.deltaTime;
            if (messageTimer <= 0 && hintText != null)
                hintText.gameObject.SetActive(false);
        }
    }

    private void OnPlateChanged(PressurePlate plate, bool pressed)
    {
        if (solved) return;

        if (AllPressed())
        {
            solved = true;
            OnPuzzleSolved();
        }
    }

    private bool AllPressed()
    {
        return plates != null && plates.All(p => p != null && p.IsPressed);
    }

    private void OnPuzzleSolved()
    {
        Debug.Log("Playground puzzle solved!");

        
        if (puzzleSolvedSFX != null)
            audioSource.PlayOneShot(puzzleSolvedSFX, solvedSFXVolume);

        
        if (solvedVFX != null)
            solvedVFX.Play();

       
        if (doorToUnlock != null)
            doorToUnlock.UnlockDoor();

        
        if (hintText != null)
        {
            hintText.text = solvedMessage;
            hintText.gameObject.SetActive(true);
            messageTimer = messageDuration;
        }
    }
}
