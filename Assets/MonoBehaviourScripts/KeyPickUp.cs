using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager inventory = FindObjectOfType<InventoryManager>();
            if (inventory != null)
            {
                inventory.AddKey();
            }
            Destroy(gameObject);
        }
    }
}
