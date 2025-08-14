using UnityEngine;

public class ShootingScript : MonoBehaviour
{
    public GameObject bullet;

    private InventoryManager inventory;

    void Start()
    {
        // Cache reference to InventoryManager at start
        inventory = FindObjectOfType<InventoryManager>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (inventory != null && inventory.ammo > 0)
            {
                var clone = Instantiate(bullet, transform.position, transform.rotation);
                Destroy(clone, 4.0f);

                inventory.UseAmmo(1); 
            }
          
        }
    }
}