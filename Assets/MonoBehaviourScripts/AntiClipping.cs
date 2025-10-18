using UnityEngine;

public class FPCameraWallAvoid : MonoBehaviour
{
    [Header("Settings")]
    public float checkDistance = 0.3f;     
    public float pushBackDistance = 0.05f; 
    public float smoothSpeed = 10f;        
    public LayerMask wallMask = ~0;       

    private Vector3 defaultLocalPos;       
    private Transform player;

    void Start()
    {
        player = transform.parent;
        defaultLocalPos = transform.localPosition; 
    }

    void LateUpdate()
    {
        if (!player) return;

        
        Vector3 worldOrigin = player.position + player.TransformVector(defaultLocalPos);
        Vector3 worldDir = transform.forward;

        if (Physics.Raycast(worldOrigin, worldDir, out RaycastHit hit, checkDistance, wallMask))
        {
            
            float dist = Mathf.Clamp(hit.distance - pushBackDistance, 0f, checkDistance);
            Vector3 targetLocalPos = defaultLocalPos;
            targetLocalPos.z = -dist; 
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocalPos, Time.deltaTime * smoothSpeed);
        }
        else
        {
            
            transform.localPosition = Vector3.Lerp(transform.localPosition, defaultLocalPos, Time.deltaTime * smoothSpeed);
        }
    }
}
