using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent), typeof(AudioSource))]
public class EnermyVariant2 : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Detection Settings")]
    public float detectionRadius = 15f;
    public float stopChasingRadius = 20f;

    [Header("Patrol Settings")]
    public float patrolRadius = 10f;
    public float patrolWaitTime = 3f;

    [Header("Movement Settings")]
    public float swayDistance = 2f;
    public float swaySpeed = 2f;
    public float dodgeChance = 0.3f;
    public float pathUpdateRate = 0.25f;
    public float hoverAmplitude = 0.5f;
    public float hoverFrequency = 2f;

    [Header("Audio")]
    public AudioClip aggroSound;

    private NavMeshAgent agent;
    private AudioSource audioSource;

    private bool isChasing = false;
    private bool isPatrolling = false;
    private bool hasPlayedAggroSound = false;

    private float patrolTimer;
    private float hoverOffset;
    private Vector3 startPosition;
    private Vector3 patrolTarget;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        startPosition = transform.position;
        hoverOffset = Random.Range(0f, Mathf.PI * 2);

        PickNewPatrolPoint();
        InvokeRepeating(nameof(CheckDistance), 0f, pathUpdateRate);
    }

    private void Update()
    {
        HoverEffect();

        if (isChasing)
        {
            agent.SetDestination(player.position);

            if (Random.value < dodgeChance * Time.deltaTime)
                StartCoroutine(SwayDodge());
        }
        else
        {
            HandlePatrol();
        }
    }

    private void HoverEffect()
    {
        Vector3 pos = transform.position;
        pos.y = startPosition.y + Mathf.Sin(Time.time * hoverFrequency + hoverOffset) * hoverAmplitude;
        transform.position = pos;
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

    private IEnumerator SwayDodge()
    {
        Vector3 sideDir = transform.right * (Random.value > 0.5f ? 1 : -1);
        Vector3 targetPos = transform.position + sideDir * swayDistance;

        float t = 0f;
        Vector3 startPos = transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime * swaySpeed;
            transform.position = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0, 1, t));
            yield return null;
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
