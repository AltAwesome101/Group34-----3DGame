using UnityEngine;

public class FixedGunPosition : MonoBehaviour
{
    public Vector3 fixedLocalPosition = new Vector3(0.4f, -0.3f, 0.8f);
    public Vector3 fixedLocalEulerAngles = new Vector3(0, 0, 0);

    void LateUpdate()
    {
        // Lock the gun's position and rotation relative to the camera
        transform.localPosition = fixedLocalPosition;
        transform.localEulerAngles = fixedLocalEulerAngles;
    }
}