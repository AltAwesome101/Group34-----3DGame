using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PortraitPuzzleManager : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public TextMeshProUGUI promptText;
    public GameObject doorToUnlock;
    public AudioClip successSound;

    [Header("Settings")]
    public float interactDistance = 3f;
    public LayerMask slotLayer;
    public float swapSpeed = 5f;

    private AudioSource audioSource;
    private PlayerInputActions controls;
    private PortraitInteractable selected;
    private bool solved;

    private readonly char[] correctOrder = { 'D', 'A', 'W', 'N' };
    private PortraitInteractable[] allSlots;

    void Awake()
    {
        controls = new PlayerInputActions();
        controls.Player.Interact.performed += OnInteract;
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Start()
    {
    
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; 
        audioSource.volume = 1f;
        audioSource.loop = false;

        if (promptText)
            promptText.gameObject.SetActive(false);

      
        allSlots = Object.FindObjectsByType<PortraitInteractable>(FindObjectsSortMode.None);
        System.Array.Sort(allSlots, (x, y) => x.transform.position.x.CompareTo(y.transform.position.x));
    }

    void Update()
    {
        if (solved) return;
        HandlePrompt();
    }

    void HandlePrompt()
    {
        if (!promptText) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, slotLayer))
        {
            if (hit.collider.TryGetComponent(out PortraitInteractable target))
            {
                promptText.gameObject.SetActive(true);
                promptText.text = selected == null ? "Press E to Select" : "Press E to Swap";
                return;
            }
        }

        promptText.gameObject.SetActive(false);
    }

    void OnInteract(InputAction.CallbackContext ctx)
    {
        if (solved) return;

        if (!Physics.Raycast(playerCamera.transform.position,
                             playerCamera.transform.forward,
                             out RaycastHit hit,
                             interactDistance, slotLayer)) return;

        if (!hit.collider.TryGetComponent(out PortraitInteractable p)) return;

        if (selected == null)
        {
            selected = p;
            p.Highlight(true);
        }
        else
        {
            if (p == selected)
            {
                p.Highlight(false);
                selected = null;
                return;
            }
            selected.Highlight(false);
            p.Highlight(false);
            StartCoroutine(SmoothSwap(selected, p));
            selected = null;
        }
    }

    IEnumerator SmoothSwap(PortraitInteractable a, PortraitInteractable b)
    {
        Vector3 posA = a.transform.position;
        Vector3 posB = b.transform.position;

        char tmpLetter = a.letter;
        a.letter = b.letter;
        b.letter = tmpLetter;

        float t = 0f;
        float distance = Vector3.Distance(posA, posB);
        float duration = distance / swapSpeed;

        while (t < duration)
        {
            t += Time.deltaTime;
            float factor = Mathf.Clamp01(t / duration);
            a.transform.position = Vector3.Lerp(posA, posB, factor);
            b.transform.position = Vector3.Lerp(posB, posA, factor);
            yield return null;
        }

        a.transform.position = posB;
        b.transform.position = posA;

        CheckSolution();
    }

    void CheckSolution()
    {
        foreach (var slot in allSlots)
        {
            if (slot.letter != correctOrder[slot.slotIndex])
                return;
        }

        Solve();
    }

    void Solve()
    {
        solved = true;

        if (successSound && audioSource)
            audioSource.PlayOneShot(successSound);

        if (doorToUnlock)
            doorToUnlock.SetActive(false);

        if (promptText)
        {
            promptText.text = "Door unlocked!";
            MissionManager.Instance.CompleteMission("Investigate your room");
        }

        StartCoroutine(HidePrompt());
    }

    IEnumerator HidePrompt()
    {
        yield return new WaitForSeconds(2f);
        if (promptText)
            promptText.gameObject.SetActive(false);
    }
}
