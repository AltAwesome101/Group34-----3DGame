using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class EmissionPulse : MonoBehaviour
{
    [ColorUsage(true, true)]
    public Color emissionColor = Color.cyan;

    public float pulseSpeed = 2f;

    public float minIntensity = 0.5f;

    public float maxIntensity = 3f;

    private Material mat;

    private float t;

    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        mat = rend.material; 
        mat.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        t = Mathf.PingPong(Time.time * pulseSpeed, 1f);
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
        mat.SetColor("_EmissionColor", emissionColor * intensity);
    }
}
