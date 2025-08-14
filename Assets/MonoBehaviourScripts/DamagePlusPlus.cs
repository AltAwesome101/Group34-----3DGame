using UnityEngine;

public class DamagePlusPlus : MonoBehaviour
{
    private void OnCollisionStay(Collision other)
    {
        if (other.transform.CompareTag("Player")) 
        {
            other.transform.SendMessage("ApplyDamage", 1);
        }
    }
}


