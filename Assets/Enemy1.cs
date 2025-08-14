using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    public float kickForce = 2.0f;
    public float knockbackTime = 1.0f;

    [Header("Health")]
    public int maxHits = 3;
    private int currentHits = 0;
    private bool isDead = false;
    private bool lastHitWasMelee = false;

    [Header("Player Damage")]
    public int playerDamage = 1;

    private Transform player;
    private NavMeshAgent agent;
    private Rigidbody rb;
    private ContactPoint contact;
    private bool gotHit = false;
    private float knockbackTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        knockbackTimer = knockbackTime;
    }

    void Update()
    {
        if (isDead || player == null) return;

        if (!gotHit && rb.isKinematic)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            knockbackTimer += Time.deltaTime;
            if (knockbackTimer >= knockbackTime)
            {
                rb.isKinematic = true;
                agent.isStopped = false;
                gotHit = false;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (isDead) return;

        // Bullet hit
        if (other.transform.CompareTag("bullet"))
        {
            contact = other.contacts[0];
            ApplyKnockback();
            TakeDamage(1, false);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        // Player contact damage
        if (!isDead && other.transform.CompareTag("Player"))
        {
            other.transform.SendMessage("ApplyDamage", playerDamage, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void TakeDamage(int amount, bool isMelee)
    {
        if (isDead) return;

        currentHits += amount;
        lastHitWasMelee = isMelee;

        if (currentHits >= maxHits)
        {
            isDead = true;

            if (lastHitWasMelee)
            {
                FindObjectOfType<NPC>()?.RegisterMeleeKill();
            }

            gameObject.SetActive(false);
        }
    }

    private void ApplyKnockback()
    {
        gotHit = true;
        rb.isKinematic = false;
        agent.isStopped = true;
        rb.AddForceAtPosition(Camera.main.transform.forward * kickForce, contact.point, ForceMode.Impulse);
        knockbackTimer = 0f;
    }
}
