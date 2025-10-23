using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("Chase Settings")]
    public float detectionRadius = 12f;
    public float stopChasingRadius = 18f;
    public float pathUpdateRate = 0.25f;

    [Header("Knockback Settings")]
    public float kick = 2.0f;
    public float knockbackTime = 1;

    private NavMeshAgent agent;
    private Transform player;
    private bool isChasing = false;

    private bool hit;
    private ContactPoint contact;
    private float timer;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        timer = knockbackTime;

        InvokeRepeating(nameof(CheckDistance), 0f, pathUpdateRate);
    }

    private void Update()
    {
        
        if (hit)
        {
            GetComponent<Rigidbody>().isKinematic = false;
            agent.isStopped = true;
            GetComponent<Rigidbody>().AddForceAtPosition(Camera.main.transform.forward * kick, contact.point, ForceMode.Impulse);
            hit = false;
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;

            if (knockbackTime < timer)
            {
                GetComponent<Rigidbody>().isKinematic = true;
                agent.isStopped = false;

                if (isChasing)
                    agent.SetDestination(player.position);
                else
                    agent.ResetPath();
            }
        }
    }

    private void CheckDistance()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        
        if (!isChasing && distance <= detectionRadius)
        {
            isChasing = true;
        }

        
        else if (isChasing && distance >= stopChasingRadius)
        {
            isChasing = false;
            agent.ResetPath();
        }

        
        if (isChasing && timer > knockbackTime)
        {
            agent.SetDestination(player.position);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("bullet"))
        {
            contact = other.contacts[0];
            hit = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopChasingRadius);
    }
}
