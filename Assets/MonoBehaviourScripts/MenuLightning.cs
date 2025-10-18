using UnityEngine;
using System.Collections;

public class MenuLightning : MonoBehaviour
{
    [Header("Lightning")]
    public Light lightToFlash;         
    public AudioSource thunderSource; 
    public float minDelay = 8f;
    public float maxDelay = 22f;

    [Header("Pattern")]
    public int miniFlashes = 2;
    public float shortFlashMin = 0.03f;
    public float shortFlashMax = 0.12f;
    public float longFlash = 0.25f;

    private float originalIntensity = 1f;

    void Start()
    {
        if (lightToFlash != null) originalIntensity = lightToFlash.intensity;
        if (lightToFlash != null) lightToFlash.enabled = false;
        StartCoroutine(LightningRoutine());
    }

    IEnumerator LightningRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));

            for (int i = 0; i < miniFlashes; i++)
            {
                yield return StartCoroutine(Flash(Random.Range(shortFlashMin, shortFlashMax)));
                yield return new WaitForSeconds(Random.Range(0.04f, 0.18f));
            }
            yield return StartCoroutine(Flash(longFlash));

            if (thunderSource != null) 
            { 
                thunderSource.Play(); 
            }
            yield return new WaitForSeconds(0.4f);
        }
    }

    private IEnumerator Flash(float duration)
    {
        if (lightToFlash != null)
        {
            lightToFlash.enabled = true;
            lightToFlash.intensity = originalIntensity * 2.5f;
        }
        yield return new WaitForSeconds(duration);
        if (lightToFlash != null)
        {
            lightToFlash.enabled = false;
            lightToFlash.intensity = originalIntensity;
        }
    }
}
