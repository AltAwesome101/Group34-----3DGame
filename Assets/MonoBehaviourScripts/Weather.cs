using UnityEngine;
using System.Collections;

public class Weather : MonoBehaviour
{
    [Header("Thunderstorm Settings")]
    public Light[] sceneLights;
    public AudioSource lightningSound;
    public float interval = 180f;
    public float blackoutDuration = 5f;

    [Header("Lightning Flicker Settings")]
    public int flickerCount = 3;
    public float minFlickerTime = 0.05f;
    public float maxFlickerTime = 0.2f;

    [Header("Flashlight Settings")]
    public Light playerFlashlight; 
    public bool autoFlashlight = true;  

    private bool isBlackout = false;

    void Start()
    {
        
        if (playerFlashlight != null)
            playerFlashlight.enabled = false;

        StartCoroutine(ThunderstormRoutine());
    }

    IEnumerator ThunderstormRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(interval);

            
            for (int i = 0; i < flickerCount; i++)
            {
                ToggleLights(false);
                yield return new WaitForSeconds(Random.Range(minFlickerTime, maxFlickerTime));
                ToggleLights(true);
                yield return new WaitForSeconds(Random.Range(minFlickerTime, maxFlickerTime));
            }
            StartCoroutine(LightningStrike());
        }
    }

    IEnumerator LightningStrike()
    {
        if (isBlackout) yield break;

        isBlackout = true;

        if (lightningSound != null)
            lightningSound.Play();
       
        ToggleLights(false);

        if (autoFlashlight && playerFlashlight != null)
            playerFlashlight.enabled = true;

        yield return new WaitForSeconds(blackoutDuration);

        ToggleLights(true);

        if (autoFlashlight && playerFlashlight != null)
            playerFlashlight.enabled = false;

        isBlackout = false;
    }

    void ToggleLights(bool state)
    {
        foreach (Light l in sceneLights)
        {
            if (l != null)
                l.enabled = state;
        }
    }
}
