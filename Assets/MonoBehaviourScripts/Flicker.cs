using UnityEngine;

public class ScaryLightFlicker : MonoBehaviour
{
    [Header("Flicker Settings")]
    public Light pointLight;          
    public float minIntensity = 0f;   
    public float maxIntensity = 2f;   
    public float flickerSpeed = 0.1f; 

    [Header("Color Change Settings")]
    public int flickersBeforeRed = 5; 
    public Color scaryColor = Color.red; 

    private int flickerCount = 0;
    private bool isScary = false;

    void Start()
    {
        if (pointLight == null)
            pointLight = GetComponent<Light>();

        if (pointLight == null)
            Debug.LogError("No Light component found!");

        
        InvokeRepeating(nameof(FlickerLight), 0f, flickerSpeed);
    }

    void FlickerLight()
    {
        if (isScary) return;

        
        pointLight.intensity = Random.Range(minIntensity, maxIntensity);
        flickerCount++;

        
        if (flickerCount >= flickersBeforeRed)
        {
            pointLight.color = scaryColor;
            pointLight.intensity = maxIntensity; 
            isScary = true;
        }
    }
}
    

