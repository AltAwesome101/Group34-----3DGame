using UnityEngine;

public class UIBobbing : MonoBehaviour
{
    [Header("Bobbing Settings")]
    public float bobAmplitude = 10f;  
    public float bobFrequency = 1f;   

    private RectTransform rectTransform;
    private Vector3 startPos;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        float yOffset = Mathf.Sin(Time.time * bobFrequency * 2 * Mathf.PI) * bobAmplitude;
        rectTransform.anchoredPosition = startPos + new Vector3(0f, yOffset, 0f);
    }
}

