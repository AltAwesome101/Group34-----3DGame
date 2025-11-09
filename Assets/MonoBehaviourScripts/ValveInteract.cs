//Title: Controlling Zones
//Author: Walter Davis
//Date: 09-02-2020
//Code Version: New-input System
//Availability: https://www.perforce.com/blog/vcs/game-asset-recycling

using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class ValveInteract_WithShaft : MonoBehaviour
{
    [Header("Gas / Controlled Zones")]
    public GasZone[] controlledZones;

    [Header("Handle / Shaft")]
    [Tooltip("The visual handle (wheel) that should rotate.")]
    public Transform handleTransform;
    [Tooltip("Optional: place the shaft Transform at the shaft center and orient its local 'up' along the shaft axis. " +
             "If assigned, the handle will RotateAround this shaft (recommended for wheels on a pipe).")]
    public Transform shaftTransform;

    [Header("Rotation")]
    [Tooltip("Degrees to rotate when closing (positive is direction applied).")]
    public float handleCloseAngle = 360f;
    [Tooltip("How long the rotation animation takes (seconds).")]
    public float rotateDuration = 1f;
    [Tooltip("If true, rotate using the shaftTransform (RotateAround). If false, use handle's local rotation.")]
    public bool useShaftRotate = true;
    [Tooltip("If true, invert rotation direction.")]
    public bool invertDirection = false;

    [Header("UI & Audio")]
    public TextMeshProUGUI promptText;
    public AudioClip closeSound;

    // Internals
    private AudioSource audioSource;
    private bool playerNearby = false;
    private bool isClosed = false;
    private Quaternion localStartRotation;
    private Quaternion localTargetRotation;
    private PlayerInputActions controls;

    private void Awake()
    {
        controls = new PlayerInputActions();
        controls.Player.Interact.performed += OnInteract;
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (handleTransform == null)
            handleTransform = transform; 

        localStartRotation = handleTransform.localRotation;
        localTargetRotation = localStartRotation * Quaternion.AngleAxis(handleCloseAngle * (invertDirection ? -1f : 1f), Vector3.up);

        if (promptText != null) promptText.gameObject.SetActive(false);
    }

    private void OnValidate()
    {
       
        if (handleTransform != null)
        {
            localStartRotation = handleTransform.localRotation;
            localTargetRotation = localStartRotation * Quaternion.AngleAxis(handleCloseAngle * (invertDirection ? -1f : 1f), Vector3.up);
        }
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (playerNearby && !isClosed)
            TryClose();
    }

    private void TryClose()
    {
        CloseValve();
    }

    public void CloseValve()
    {
        if (isClosed) return;
        isClosed = true;

        // mission and gas
        MissionManager.Instance?.CompleteMission("Find the Gas Valve");
        if (controlledZones != null)
        {
            foreach (var gz in controlledZones) gz?.CloseValve();
        }

        if (audioSource != null && closeSound != null) audioSource.PlayOneShot(closeSound);

        if (useShaftRotate && shaftTransform != null)
        {
            StartCoroutine(RotateAroundShaftCoroutine(handleCloseAngle * (invertDirection ? -1f : 1f), rotateDuration));
        }
        else
        {
            StartCoroutine(RotateLocalCoroutine(handleCloseAngle * (invertDirection ? -1f : 1f), rotateDuration));
        }

        if (promptText != null)
        {
            promptText.text = "Valve closed";
            StartCoroutine(HidePromptAfter(2f));
        }
    }

    private IEnumerator RotateLocalCoroutine(float totalAngle, float duration)
    {
        if (handleTransform == null) yield break;

        Quaternion from = handleTransform.localRotation;
        Quaternion to = from * Quaternion.AngleAxis(totalAngle, Vector3.up); // uses local up
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            handleTransform.localRotation = Quaternion.Slerp(from, to, Mathf.Clamp01(t / duration));
            yield return null;
        }
        handleTransform.localRotation = to;
    }

    private IEnumerator RotateAroundShaftCoroutine(float totalAngleDegrees, float duration)
    {
        if (handleTransform == null || shaftTransform == null) yield break;

        Vector3 axisWorld = shaftTransform.TransformDirection(Vector3.up).normalized;

        float elapsed = 0f;
        float accumulated = 0f;
        float sign = Mathf.Sign(totalAngleDegrees);
        float absTotal = Mathf.Abs(totalAngleDegrees);

        while (elapsed < duration)
        {
            float prevElapsed = elapsed;
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float easedT = t * t * (3f - 2f * t);
            float targetAccum = absTotal * easedT;
            float delta = targetAccum - accumulated;
            accumulated = targetAccum;
            handleTransform.RotateAround(shaftTransform.position, axisWorld, delta * sign);

            yield return null;
        }

        float remaining = sign * (totalAngleDegrees) - sign * accumulated;
        if (Mathf.Abs(remaining) > 0.0001f)
        {
            handleTransform.RotateAround(shaftTransform.position, axisWorld, remaining);
        }
    }

    private IEnumerator HidePromptAfter(float s)
    {
        yield return new WaitForSeconds(s);
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = true;
        if (!isClosed && promptText != null)
        {
            promptText.text = "Press E to close valve";
            promptText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerNearby = false;
        if (promptText != null)
            promptText.gameObject.SetActive(false);
    }
}
