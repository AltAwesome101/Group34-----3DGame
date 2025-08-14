using UnityEngine;

public class EnermyHealth : MonoBehaviour
{
    private int hitNumber;

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("bullet"))
        {
            hitNumber++;
            CheckIfDead();
        }
    }
    public void TakeDamage(int amount)
    {
        hitNumber += amount;
        CheckIfDead();
    }

    private void CheckIfDead()
    {
        if (hitNumber >= 2)
        {
            Destroy(gameObject);
        }
    }
}