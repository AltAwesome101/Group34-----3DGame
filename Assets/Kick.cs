using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class MeleeSwing : MonoBehaviour
{
    [Header("References")]
    public PlayerInputActions controls;

    [Header("Melee Settings")]
    [Tooltip("Total time for forward + return swing.")]
    public float swingDuration = 1f;

    [Tooltip("Start rotation on X-axis.")]
    public float startXRotation = 90f;

    [Tooltip("End rotation on X-axis.")]
    public float endXRotation = 0f;

    [Header("Constant Rotation Axes")]
    [Tooltip("Constant Y-axis rotation during swing.")]
    public float constantY = 0f;

    [Tooltip("Constant Z-axis rotation during swing.")]
    public float constantZ = 0f;

    private bool isSwinging = false;
    private Quaternion startRotation;
    private Quaternion endRotation;
    private float halfDuration;

    private void Awake()
    {
        if (controls == null)
            controls = new PlayerInputActions();

        startRotation = Quaternion.Euler(startXRotation, constantY, constantZ);
        endRotation = Quaternion.Euler(endXRotation, constantY, constantZ);
        halfDuration = swingDuration / 2f;

        transform.localRotation = startRotation;
    }

    private void OnEnable()
    {
        controls.Player.Melee.performed += OnMelee;
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Melee.performed -= OnMelee;
        controls.Disable();
    }

    private void OnMelee(InputAction.CallbackContext ctx)
    {
        if (!isSwinging)
            StartCoroutine(SwingRoutine());
    }

    private IEnumerator SwingRoutine()
    {
        isSwinging = true;

        float elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            transform.localRotation = Quaternion.Slerp(
                Quaternion.Euler(startXRotation, constantY, constantZ),
                Quaternion.Euler(endXRotation, constantY, constantZ),
                t
            );
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            transform.localRotation = Quaternion.Slerp(
                Quaternion.Euler(endXRotation, constantY, constantZ),
                Quaternion.Euler(startXRotation, constantY, constantZ),
                t
            );
            yield return null;
        }

        isSwinging = false;
    }
}
