using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DamagePlayer playerHealth = other.GetComponent<DamagePlayer>();
            if (playerHealth != null && playerHealth.health < playerHealth.maxHealth)
            {
                playerHealth.AddHealth(healAmount);
                Destroy(gameObject);
            }
        }
    }
}
