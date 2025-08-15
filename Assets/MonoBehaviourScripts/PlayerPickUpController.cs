//Title: PickingUP Objects in Unity
//Author: Hayes A.
//Date: 14-08-2025
//Code Version: New-input System
//Availability: Module Tab-class Slides

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickupController : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;
    public Transform holdPoint;

    [Header("Settings")]
    public float pickupRange = 3f;
    public float throwForce = 10f;

    private PickableObject heldObject;

    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.PickUpObject.performed += OnPickUpObject;
        inputActions.Player.ThrowObject.performed += OnThrowObject;
    }

    private void OnDisable()
    {
        inputActions.Player.PickUpObject.performed -= OnPickUpObject;
        inputActions.Player.ThrowObject.performed -= OnThrowObject;
        inputActions.Disable();
    }

    private void Update()
    {
        if (heldObject != null && heldObject.transform == null)
        {
            heldObject = null;
            return;
        }
        if (heldObject != null)
        {
            heldObject.MoveToHoldPoint(holdPoint.position);
        }
    }

    private void OnPickUpObject(InputAction.CallbackContext context)
    {
        if (!context.performed)
        { 
            return; 
        }

        if (heldObject != null)
        {
            heldObject.Drop();
            heldObject = null;
            return;
        }

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            PickableObject pickable = hit.collider.GetComponent<PickableObject>();
            if (pickable != null)
            {
                pickable.Pickup(holdPoint);
                heldObject = pickable;
            }
        }
    }

    private void OnThrowObject(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (heldObject != null && heldObject.transform != null)
        {
            Vector3 throwDirection = cameraTransform.forward * throwForce;
            heldObject.Throw(throwDirection);
        }
        heldObject = null;
    }
}