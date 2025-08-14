using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        InventoryManager inventory = other.GetComponent<InventoryManager>();
        if (inventory == null)
        {
            inventory = FindObjectOfType<InventoryManager>();
        }

        if (inventory != null)
        {
            inventory.AddKey();
        }

        Destroy(gameObject);
    }
}