using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class ValveInteract : MonoBehaviour
{
    [Tooltip("Gas zones controlled by this valve")]
    public GasZone[] controlledZones;

    [Tooltip("Handle transform (rotated on close). If left empty, this object's transform will be used.")]
    public Transform handleTransform;

    public float handleCloseAngle = -90f; 

    public float rotateDuration = 0.5f;

    [Header("UI & Audio")]
    public TextMeshProUGUI promptText;
    public AudioClip closeSound;

    private AudioSource audioSource;

    private bool playerNearby = false;

    private bool isClosed = false;

    private Quaternion openRotation;

    private Quaternion closedRotation;

    private PlayerInputActions controls;

    private void Awake()
    {
        controls = new PlayerInputActions();
        controls.Player.Interact.performed += OnInteract;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (handleTransform == null) handleTransform = transform;
        openRotation = handleTransform.localRotation;
        closedRotation = openRotation * Quaternion.Euler(0f, handleCloseAngle, 0f);

        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (playerNearby && !isClosed)
        {
            TryClose();
        }
    }

    private void TryClose()
    {
        CloseValve();
    }

    public void CloseValve()
    {
        if (isClosed) return;
        isClosed = true;
        MissionManager.Instance.CompleteMission("Find the Gas Valve");

        if (controlledZones != null)
        {
            foreach (var gz in controlledZones)
                if (gz != null) gz.CloseValve();
        }

        if (audioSource != null && closeSound != null)
        {
            audioSource.PlayOneShot(closeSound);
        }

        StartCoroutine(RotateHandle(openRotation, closedRotation, rotateDuration));

        if (promptText != null)
        {
            promptText.text = "Valve closed";
            StartCoroutine(HidePromptAfter(2f));
        }
    }

    private IEnumerator RotateHandle(Quaternion from, Quaternion to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            handleTransform.localRotation = Quaternion.Slerp(from, to, t / duration);
            yield return null;
        }
        handleTransform.localRotation = to;
    }

    private IEnumerator HidePromptAfter(float s)
    {
        yield return new WaitForSeconds(s);
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            if (!isClosed && promptText != null)
            {
                promptText.text = "Press E to close valve";
                promptText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            if (promptText != null)
                promptText.gameObject.SetActive(false);
        }
    }
}
