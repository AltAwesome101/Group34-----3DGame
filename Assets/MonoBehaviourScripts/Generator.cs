using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(AudioSource))]
public class GeneratorInteract : MonoBehaviour
{
    [Header("Parts")]
    public List<int> requiredPartIDs = new List<int>();
    public GameObject[] partPrefabs;
    public Transform[] placementPoints;

    [Header("Outputs")]
    public Light[] lightsToEnable;
    public GameObject doorToUnlock;

    [Header("UI & Audio")]
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private AudioClip placeSound;
    [SerializeField] private AudioClip completeSound;
    [SerializeField] private AudioClip holdLoopSound;  

    [Header("Hold Interaction")]
    [SerializeField] private GeneratorHoldUI holdUI;
    [SerializeField] private float holdTime = 2f;

    private GeneratorPartsUI partsUI;

    private List<int> remainingParts;

    private int placedCount;

    private bool playerNearby;

    private PlayerInventory nearbyInventory;

    private AudioSource audioSource;

    private PlayerInputActions controls;

    private bool isHolding;

    private float holdProgress;

    private void Awake()
    {
        controls = new PlayerInputActions();
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; 
        audioSource.loop = false;

        remainingParts = new List<int>(requiredPartIDs);
        HidePrompt();

        if (partsUI == null)
            partsUI = FindFirstObjectByType<GeneratorPartsUI>();
    }

    private void OnEnable()
    {
        controls.Player.Interact.started += StartHold;
        controls.Player.Interact.canceled += StopHold;
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Interact.started -= StartHold;
        controls.Player.Interact.canceled -= StopHold;
        controls.Disable();
    }

    private void Start()
    {
        UpdateProgressUI();

        if (lightsToEnable != null)
            foreach (var l in lightsToEnable) if (l) l.enabled = false;

        if (holdUI) holdUI.Show(false);
    }

    private void Update()
    {
        if (!playerNearby || nearbyInventory == null) return;

        if (isHolding)
        {
            holdProgress += Time.deltaTime;

            if (holdUI)
            {
                holdUI.Show(true);
                holdUI.SetProgress(holdProgress / holdTime);
            }

            if (holdProgress >= holdTime)
            {
                TryInsert();
                StopHold();
            }
        }
    }

    private void StartHold(InputAction.CallbackContext ctx)
    {
        if (playerNearby)
        {
            isHolding = true;
            holdProgress = 0;

          
            if (holdLoopSound != null)
            {
                audioSource.clip = holdLoopSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
    }

    private void StopHold(InputAction.CallbackContext ctx) => StopHold();

    private void StopHold()
    {
        isHolding = false;
        holdProgress = 0;

        if (holdUI) holdUI.Show(false);

     
        if (audioSource.isPlaying && audioSource.loop)
        {
            audioSource.Stop();
            audioSource.loop = false;
            audioSource.clip = null;
        }
    }

    private void TryInsert()
    {
        if (!nearbyInventory.HasPart())
        {
            ShowPrompt("You are not holding a part", 1.5f);
            return;
        }

        int held = nearbyInventory.GetHeldPartID();
        int idx = remainingParts.IndexOf(held);

        if (idx == -1)
        {
            ShowPrompt("That part doesn't fit here", 1.5f);
            return;
        }

        PlacePart(held, idx);
    }

    private void PlacePart(int partID, int remainingIndex)
    {
        remainingParts.RemoveAt(remainingIndex);

        if (placementPoints != null && placedCount < placementPoints.Length)
        {
            GameObject prefab = (partPrefabs != null && partID < partPrefabs.Length)
                ? partPrefabs[partID] : null;

            if (prefab)
                Instantiate(prefab,
                            placementPoints[placedCount].position,
                            placementPoints[placedCount].rotation,
                            placementPoints[placedCount]);

            placedCount++;
        }

        nearbyInventory.RemoveHeldPart();

        if (audioSource && placeSound)
            audioSource.PlayOneShot(placeSound);

        if (partsUI != null)
            partsUI.HidePart(partID);

        UpdateProgressUI();
        UpdatePrompt();

        if (remainingParts.Count == 0) OnGeneratorComplete();
    }

    private void UpdatePrompt()
    {
        if (!playerNearby || nearbyInventory == null)
        {
            HidePrompt();
            return;
        }

        if (nearbyInventory.HasPart())
        {
            int held = nearbyInventory.GetHeldPartID();
            if (remainingParts.Contains(held))
                ShowPrompt("Hold [E] to insert part");
            else
                ShowPrompt("This part doesn't fit");
        }
        else
        {
            ShowPrompt("Find parts to power the generator");
        }
    }

    private void ShowPrompt(string msg, float hideAfter = 0f)
    {
        if (!promptText) return;

        promptText.text = msg;
        promptText.enabled = true;

        if (hideAfter > 0)
            StartCoroutine(HideAfter(hideAfter));
    }

    private void HidePrompt()
    {
        if (promptText) promptText.enabled = false;
    }

    private IEnumerator HideAfter(float sec)
    {
        yield return new WaitForSeconds(sec);
        UpdatePrompt();
    }

    private void UpdateProgressUI()
    {
        if (!progressText) return;
        int total = requiredPartIDs.Count;
        int placed = total - remainingParts.Count;
        progressText.text = $"Generator: {placed}/{total} parts placed";
        progressText.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        nearbyInventory = other.GetComponent<PlayerInventory>();
        if (!nearbyInventory) return;

        playerNearby = true;
        UpdatePrompt();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerNearby = false;
        nearbyInventory = null;
        StopHold();
        HidePrompt();
    }

    private void OnGeneratorComplete()
    {
     
        if (audioSource.isPlaying && audioSource.loop)
        {
            audioSource.Stop();
            audioSource.loop = false;
            audioSource.clip = null;
        }

    
        if (audioSource && completeSound)
            audioSource.PlayOneShot(completeSound);

        if (lightsToEnable != null)
            foreach (var l in lightsToEnable) if (l) l.enabled = true;

        if (doorToUnlock)
        {
            var doorComp = doorToUnlock.GetComponent<DoorUnlock>();
            if (doorComp) doorComp.Unlock();
            else doorToUnlock.SetActive(false);
        }

        MissionManager.Instance.CompleteMission("Repair the generator");

        ShowPrompt("Generator powered!", 3f);
    }
}
