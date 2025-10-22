using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GeneratorPartsUI : MonoBehaviour
{
    [Header("Part Icons (Match part IDs)")]
    public List<Image> partIcons = new List<Image>();

    private void Start()
    {
        foreach (var icon in partIcons)
        {
            if (icon) icon.enabled = false;
        }
    }

    public void ShowPart(int partID)
    {
        if (partID >= 0 && partID < partIcons.Count && partIcons[partID])
        {
            partIcons[partID].enabled = true;
        }
    }

    public void HidePart(int partID)
    {
        if (partID >= 0 && partID < partIcons.Count && partIcons[partID])
        {
            partIcons[partID].enabled = false;
        }
    }
}
