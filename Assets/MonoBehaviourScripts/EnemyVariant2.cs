using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent), typeof(AudioSource))]
public class EnemyVariant2 : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Detection Settings")]
    public float detectionRadius = 15f;
    public float stopChasingRadius = 22f;
    public float attackRange = 2.5f;
    public float attackCooldown = 2f;

    [Header("Patrol Settings")]
    public float patrolRadius = 10f;
    public float patrolWaitTime = 3f;

    [Header("Combat Movement")]
    public float swayDistance = 2f;
    public float swaySpeed = 5f;
    public float dodgeChance = 0.2f;
    public float reactionDodgeChance = 0.7f;

    [Header("Audio")]
    public AudioClip aggroSound;
    public AudioClip attackSound;
    public AudioClip hurtSound;

    private NavMeshAgent agent;
    private AudioSource audioSource;
    private bool isChasing = false;
    private bool isPatrolling = false;
    private bool isAttacking = false;
    private bool hasPlayedAggroSound = false;

    private float patrolTimer;
    private float lastAttackTime;
    private Vector3 patrolTarget;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        PickNewPatrolPoint();
        InvokeRepeating(nameof(CheckDistance), 0f, 0.25f);
    }

    private void Update()
    {
        if (player == null) return;

        if (isChasing)
        {
            // Face player
            Vector3 lookPos = player.position - transform.position;
            lookPos.y = 0;
            if (lookPos.sqrMagnitude > 0.1f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos), 5f * Time.deltaTime);

            float distance = Vector3.Distance(transform.position, player.position);
            agent.isStopped = false;

            if (distance <= attackRange)
            {
                TryAttack();
            }
            else
            {
                agent.SetDestination(player.position);

                // Occasional dodge when moving
                if (Random.value < dodgeChance * Time.deltaTime)
                    StartCoroutine(SwayDodge());
            }
        }
        else
        {
            HandlePatrol();
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
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius + transform.position;

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
            Vector3 newPos = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0, 1, t));
            agent.Warp(newPos); // Safe way to reposition without breaking navmesh path
            yield return null;
        }
    }

    private void TryAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown || isAttacking) return;

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        agent.isStopped = true;

        // Simple melee lunge
        if (attackSound) audioSource.PlayOneShot(attackSound);
        Vector3 attackDir = (player.position - transform.position).normalized;
        Vector3 lungeTarget = transform.position + attackDir * 1.5f;

        float t = 0f;
        Vector3 startPos = transform.position;
        while (t < 1f)
        {
            t += Time.deltaTime * 6f;
            transform.position = Vector3.Lerp(startPos, lungeTarget, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        // Damage logic here (you can call player.TakeDamage(dmg))
        lastAttackTime = Time.time;
        yield return new WaitForSeconds(0.3f);

        isAttacking = false;
        agent.isStopped = false;
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

    // Reacts to being hit
    public void OnHit()
    {
        if (hurtSound) audioSource.PlayOneShot(hurtSound);

        // Random dodge reaction
        if (Random.value < reactionDodgeChance)
            StartCoroutine(SwayDodge());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopChasingRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
