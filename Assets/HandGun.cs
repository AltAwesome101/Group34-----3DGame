using UnityEngine;

public class SpineFollowCamera_ZAxis_Inverted : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform; // Assign your player camera

    [Header("Settings")]
    public float rotationSpeed = 10f;
    public Vector3 rotationOffset; // Adjust to align the spine rotation correctly

    private void LateUpdate()
    {
        if (!cameraTransform) return;

        // Get the camera pitch (up/down rotation)
        float pitch = cameraTransform.localEulerAngles.x;

        // Normalize the angle (-180 to 180)
        if (pitch > 180f) pitch -= 360f;

        // Invert pitch so looking up bends backward
        pitch = -pitch;

        // Rotate only around local Z axis using inverted pitch
        Quaternion targetRotation = Quaternion.Euler(rotationOffset.x, rotationOffset.y, pitch + rotationOffset.z);

        // Smooth rotation for natural movement
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
