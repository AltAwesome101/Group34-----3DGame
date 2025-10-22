using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerPushInteraction : MonoBehaviour
{
    [Header("Push Interaction")]
    public float interactRange = 2.5f;
    public LayerMask pushableLayer;
    public TextMeshProUGUI promptText;

    private Camera cam;
    private PlayerInputActions controls;
    private bool isHoldingPushKey;

    private void Awake()
    {
        cam = Camera.main;
        controls = new PlayerInputActions();
    }

    private void OnEnable()
    {
        controls.Player.Interact.started += OnInteract;
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Interact.started -= OnInteract;
        controls.Disable();
    }

    private void Update()
    {
        ShowPromptIfLookingAtPushable();
    }

    private void ShowPromptIfLookingAtPushable()
    {
        if (!promptText) return;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, interactRange, pushableLayer))
        {
            if (hit.collider.GetComponent<PushableObject>())
            {
                promptText.text = "Press [E] to push";
                promptText.enabled = true;
                return;
            }
        }

        promptText.enabled = false;
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, interactRange, pushableLayer))
        {
            PushableObject pushable = hit.collider.GetComponent<PushableObject>();
            if (pushable != null)
            {
                Vector3 pushDir = cam.transform.forward;
                pushDir.y = 0;
                pushable.Push(pushDir);
            }
        }
    }
}
