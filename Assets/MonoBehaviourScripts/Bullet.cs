using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Bullet : MonoBehaviour
{
    public float speed = 30f;

    public float lifeTime = 5f;

    public GameObject impactParticle;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
       
        transform.position += transform.forward * speed * Time.deltaTime;
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f)
        {
            gameObject.SetActive(false); 
        }
    }

    void OnCollisionEnter(Collision other)
    {
       
        if (impactParticle != null && other.contactCount > 0)
        {
            ContactPoint contact = other.contacts[0];
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Instantiate(impactParticle, contact.point, rot);
        }

       
        Destroyable dest = other.gameObject.GetComponent<Destroyable>();

        if (dest != null) dest.RegisterHit();

        Destroy(gameObject);
    }
}
