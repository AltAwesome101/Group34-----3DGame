using UnityEngine;

public class ArmFollowCameraPosition : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;   
    public Transform playerBody;        

    [Header("Offsets")]
    public Vector3 positionOffset = new Vector3(0f, -0.3f, 0.3f);
    public Vector3 rotationOffset;

    [Header("Settings")]
    public float followSpeed = 10f;
    public float rotationFollowSpeed = 15f;

    private void LateUpdate()
    {
        if (!cameraTransform || !playerBody) return;

        Vector3 targetPosition = playerBody.position + playerBody.TransformDirection(positionOffset);
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        Quaternion targetRotation = Quaternion.Euler(cameraTransform.eulerAngles) * Quaternion.Euler(rotationOffset);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationFollowSpeed * Time.deltaTime);
    }
}
