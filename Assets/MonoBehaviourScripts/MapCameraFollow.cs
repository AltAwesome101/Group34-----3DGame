using UnityEngine;

public class MapCameraFollow : MonoBehaviour
{
    public Transform player;     
    public float height = 100f; 
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (player == null) return;

       
        Vector3 targetPosition = new Vector3(player.position.x, height, player.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);

    
        transform.rotation = Quaternion.Euler(90f, player.eulerAngles.y, 0f);
    }
}
