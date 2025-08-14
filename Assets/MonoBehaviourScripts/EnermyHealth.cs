using UnityEngine;

public class EnermyHealth : MonoBehaviour
{
    private int hitNumber = 0;
    private bool isDead = false;
    private bool lastHitWasMelee = false;

    private void OnEnable()
    {
        // Reset enemy state when re-enabled
        hitNumber = 0;
        isDead = false;
    }

    public void TakeDamage(int amount, bool isMelee = false)
    {
        if (isDead) return;

        hitNumber += amount;
        lastHitWasMelee = isMelee;

        if (hitNumber >= 3)
        {
            isDead = true;

            if (lastHitWasMelee)
            {
                FindObjectOfType<NPC>()?.RegisterMeleeKill();
            }

            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("bullet"))
        {
            TakeDamage(1, false);
        }
    }
}
