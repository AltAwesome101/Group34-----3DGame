using UnityEngine;

public class EndEffect : MonoBehaviour
{
    private float timer;
    public float maxTime = 1.0f;
    void Start()
    {
        
    }

    void Update()
    {
        timer += Time.deltaTime;

        if(timer > maxTime) 
        {
            Destroy(gameObject);
        }
    }
}
