using UnityEngine;

public class SpineFollowCamera_ZAxis_Inverted : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform; 

    [Header("Settings")]
    public float rotationSpeed = 10f;
    public Vector3 rotationOffset; 

    private void LateUpdate()
    {
        if (!cameraTransform) return;

        
        float pitch = cameraTransform.localEulerAngles.x;

      
        if (pitch > 180f) pitch -= 360f;
        pitch = -pitch;

        Quaternion targetRotation = Quaternion.Euler(rotationOffset.x, rotationOffset.y, pitch + rotationOffset.z);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
