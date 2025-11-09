//Title: Setting up an Inventory system with scriptable objects in unity
//Author: Jared Amlin
//Date: 04-03-2021
//Code Version: New-input System
//Availability: https://jaredamlin.medium.com/setting-up-an-inventory-system-with-scriptable-objects-in-unity-176599ca49bb

using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(Collider))]
public class PartItem : MonoBehaviour
{
    public int partID;

    [Header("UI / Feedback")]
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private GameObject pickupVFXPrefab;
    [Range(0f, 1f)] public float pickupVolume = 1f;

    private GeneratorPartsUI partsUI;

    private PlayerInputActions controls;

    private PlayerInventory nearbyInventory;

    private bool playerNearby;

    private void Awake()
    {
        controls = new PlayerInputActions();
        HidePrompt();

        if (partsUI == null)
        {
            partsUI = FindFirstObjectByType<GeneratorPartsUI>();
        }
    }

    private void OnEnable()
    {
        controls.Player.Interact.performed += OnInteract;
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Interact.performed -= OnInteract;
        controls.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        nearbyInventory = other.GetComponent<PlayerInventory>();
        if (nearbyInventory == null)
        {
            nearbyInventory = FindFirstObjectByType<PlayerInventory>();
        }

        playerNearby = true;
        ShowPrompt("Press E to pick up part");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = false;
        nearbyInventory = null;
        HidePrompt();
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!playerNearby || nearbyInventory == null)
        {
            return;
        }

        if (!nearbyInventory.AddPart(partID))
        {
            ShowPrompt("You're already holding a part! Install it on the Generator first.");
            return;
        }

        if (partsUI != null)
        {
            partsUI.ShowPart(partID);
        }

        
        if (pickupSound)
        {
            GameObject tempAudio = new GameObject("Temp2DSound");
            AudioSource aSrc = tempAudio.AddComponent<AudioSource>();
            aSrc.clip = pickupSound;
            aSrc.volume = pickupVolume;
            aSrc.spatialBlend = 0f; 
            aSrc.Play();
            Destroy(tempAudio, pickupSound.length);
        }

      
        if (pickupVFXPrefab)
        {
            var fx = Instantiate(pickupVFXPrefab, transform.position, Quaternion.identity);
            Destroy(fx, 3f); 
        }

        HidePrompt();
        gameObject.SetActive(false);
    }

    private void ShowPrompt(string message)
    {
        if (!promptText) return;
        promptText.text = message;
        promptText.enabled = true;
    }

    private void HidePrompt()
    {
        if (!promptText) return;
        promptText.enabled = false;
    }
}
