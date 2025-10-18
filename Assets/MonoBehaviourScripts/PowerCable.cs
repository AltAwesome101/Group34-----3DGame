using UnityEngine;

[ExecuteAlways]
public class CableConnector : MonoBehaviour
{
    public Transform startPoint;
    
    public Transform endPoint;
    
    public int segments = 20; 
    
    public float cableSlack = 0.5f;

    private LineRenderer line;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (line == null || startPoint == null || endPoint == null)
            return;

      
        line.positionCount = segments;

    
        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)(segments - 1);

       
            Vector3 pos = Vector3.Lerp(startPoint.position, endPoint.position, t);

            float droop = Mathf.Sin(t * Mathf.PI) * cableSlack;
            pos.y -= droop;

            line.SetPosition(i, pos);
        }
    }
}
