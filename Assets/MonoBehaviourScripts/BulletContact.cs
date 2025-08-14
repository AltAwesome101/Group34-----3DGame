using UnityEngine;

public class BulletContact : MonoBehaviour
{
    void OnCollisionEnter() 
    {
        gameObject.SetActive(false);
    }
}
