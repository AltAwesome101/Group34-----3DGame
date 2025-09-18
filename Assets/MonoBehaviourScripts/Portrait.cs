using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PortraitInteractable : MonoBehaviour
{
    [Header("Portrait Settings")]
    public char letter;                 
    public int slotIndex;                
    public Material highlightMat;        

    private Material originalMat;
    private Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        originalMat = rend.material;
    }

    public void Highlight(bool on)
    {
        if (highlightMat == null) return;
        rend.material = on ? highlightMat : originalMat;
    }
}

