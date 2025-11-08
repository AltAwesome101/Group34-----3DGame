using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(AudioSource))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRadius = 12f;
    public float stopChasingRadius = 18f;
    public float pathUpdateRate = 0.25f;

    [Header("Patrol Settings")]
    public float patrolRadius = 10f;
    public float patrolWaitTime = 3f;

    [Header("Knockback Settings")]
    public float kick = 2.0f;
    public float knockbackTime = 1;

    [Header("Audio")]
    public AudioClip aggroSound;

    private NavMeshAgent agent;
    private Transform player;
    private AudioSource audioSource;

    private bool isChasing = false;
    private bool hit;
    private bool hasPlayedAggroSound = false;
    private bool isPatrolling = false;

    private ContactPoint contact;
    private float timer;
    private float patrolTimer = 0f;
    private Vector3 patrolTarget;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();

        timer = knockbackTime;

        // Start patrol immediately
        PickNewPatrolPoint();

        InvokeRepeating(nameof(CheckDistance), 0f, pathUpdateRate);
    }

    private void Update()
    {
        if (hit)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;
            agent.isStopped = true;
            rb.AddForceAtPosition(Camera.main.transform.forward * kick, contact.point, ForceMode.Impulse);
            hit = false;
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;

            if (knockbackTime < timer)
            {
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.isKinematic = true;
                agent.isStopped = false;

                if (isChasing)
                {
                    agent.SetDestination(player.position);
                }
                else
                {
                    HandlePatrol();
                }
            }
        }
    }

    private void HandlePatrol()
    {
        if (!isPatrolling || agent.remainingDistance < 0.5f)
        {
            patrolTimer += Time.deltaTime;

            if (patrolTimer >= patrolWaitTime)
            {
                PickNewPatrolPoint();
                patrolTimer = 0f;
            }
        }
    }

    private void PickNewPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
        {
            patrolTarget = hit.position;
            agent.SetDestination(patrolTarget);
            isPatrolling = true;
        }
    }

    private void CheckDistance()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (!isChasing && distance <= detectionRadius)
        {
            isChasing = true;
            isPatrolling = false;

            // Play aggro sound ONCE
            if (!hasPlayedAggroSound && aggroSound != null)
            {
                audioSource.PlayOneShot(aggroSound);
                hasPlayedAggroSound = true;
            }
        }
        else if (isChasing && distance >= stopChasingRadius)
        {
            isChasing = false;
            PickNewPatrolPoint();
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

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }
}
