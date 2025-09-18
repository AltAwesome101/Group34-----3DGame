using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Tooltip("When the player picks up a part, instantiate its preview here (optional).")]
    public Transform holdDisplayPoint;

    [Tooltip("Prefab list indexed by partID for visual preview (optional).")]
    public GameObject[] partPrefabs;

    private int heldPartID = -1;

    private GameObject heldPreviewInstance;

    public bool HasPart() => heldPartID != -1;
    public int GetHeldPartID() => heldPartID;

    public bool AddPart(int partID)
    {
        if (heldPartID != -1) 
        { 
            return false; 
        }
        heldPartID = partID;

        if (holdDisplayPoint != null && partPrefabs != null && partID >= 0 && partID < partPrefabs.Length)
        {
            var prefab = partPrefabs[partID];
            if (prefab != null)
            {
                heldPreviewInstance = Instantiate(prefab, holdDisplayPoint.position, holdDisplayPoint.rotation, holdDisplayPoint);
     
                foreach (var c in heldPreviewInstance.GetComponentsInChildren<Collider>()) c.enabled = false;
            }
        }

        return true;
    }

    public int RemoveHeldPart()
    {
        int tmp = heldPartID;
        heldPartID = -1;
        if (heldPreviewInstance != null) Destroy(heldPreviewInstance);
        heldPreviewInstance = null;
        return tmp;
    }

    public void Clear()
    {
        RemoveHeldPart();
    }
}
