using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager inventory = FindObjectOfType<InventoryManager>();
            if (inventory != null)
            {
                inventory.AddAmmo(ammoAmount);
            }
            Destroy(gameObject);
        }
    }
}

