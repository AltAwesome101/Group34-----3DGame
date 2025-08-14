using UnityEngine;

public class BulletContact : MonoBehaviour
{
    public GameObject particle;

    void OnCollisionEnter(Collision other)
    {
        ContactPoint contact = other.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;

        Instantiate(particle, pos, rot);

        // Check if the object hit has a Destroyable script
        Destroyable destroyable = other.gameObject.GetComponent<Destroyable>();
        if (destroyable != null)
        {
            destroyable.RegisterHit();
        }

        // Destroy the bullet
        Destroy(gameObject);
    }
}