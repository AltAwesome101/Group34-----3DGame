using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    [Tooltip("Optional: leave empty to use Camera.main")]
    public Camera targetCamera;

    private void LateUpdate()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera != null)
        {
            transform.LookAt(
                transform.position + targetCamera.transform.rotation * Vector3.forward,
                targetCamera.transform.rotation * Vector3.up
            );
        }
    }
}
