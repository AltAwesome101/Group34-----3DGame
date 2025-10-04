using UnityEngine;

public class CameraIdleSway : MonoBehaviour
{
    [Header("Sway")]
    public float swayAmount = 0.12f;     
    public float swaySpeed = 0.35f;     
    public float rotAmount = 0.6f;       
    public float rotSpeed = 0.15f;

    private Vector3 startLocalPos;

    private Quaternion startLocalRot;

    void Start()
    {
        startLocalPos = transform.localPosition;
        startLocalRot = transform.localRotation;
    }

    void Update()
    {
        float t = Time.time * swaySpeed;
        float x = Mathf.Sin(t) * swayAmount;
        float y = Mathf.Sin(t * 0.6f) * (swayAmount * 0.4f);

        transform.localPosition = startLocalPos + new Vector3(x, y, 0f);

        float rt = Time.time * rotSpeed;
        float rz = Mathf.Sin(rt) * rotAmount;

        transform.localRotation = startLocalRot * Quaternion.Euler(0f, 0f, rz);
    }
}
