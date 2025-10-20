using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public bool isHoldingPart { get; private set; } = false;
    public int currentPartID { get; private set; } = -1;

    
    public bool HasPart() => isHoldingPart;
    public int GetHeldPartID() => currentPartID;
    public void RemoveHeldPart()
    {
        if (!isHoldingPart) return;
        currentPartID = -1;
        isHoldingPart = false;
    }

    
    public bool AddPart(int partID)
    {
        if (isHoldingPart)
            return false;

        currentPartID = partID;
        isHoldingPart = true;
        return true;
    }
}
