using UnityEngine;

public class WorldBobbingAndLook : MonoBehaviour
{
    [Header("Bobbing Settings")]
    [Tooltip("How far the object moves up and down.")]
    public float bobAmplitude = 0.25f;

    [Tooltip("How fast the object bobs up and down.")]
    public float bobFrequency = 1f;

    [Header("Look Settings")]
    [Tooltip("The target to look at, usually the player or camera.")]
    public Transform lookTarget;

    [Tooltip("Rotation offset if the model isn't facing correctly.")]
    public Vector3 rotationOffset = Vector3.zero;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;

        
        if (lookTarget == null && Camera.main != null)
        {
            lookTarget = Camera.main.transform;
        }
    }

    private void Update()
    {
        
        float newY = startPos.y + Mathf.Sin(Time.time * bobFrequency * 2f * Mathf.PI) * bobAmplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);

        
        if (lookTarget != null)
        {
            Vector3 direction = lookTarget.position - transform.position;
            direction.y = 0f; 
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = lookRotation * Quaternion.Euler(rotationOffset);
            }
        }
    }
}
