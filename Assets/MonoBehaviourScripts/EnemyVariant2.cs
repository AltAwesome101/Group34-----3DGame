using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]

[RequireComponent(typeof(Rigidbody))]
public class EnermyVariant2 : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Movement Settings")]
    public float jumpForce = 6f;
    public float dodgeForce = 4f;
    public float jumpInterval = 3f;       // Time between jumps
    public float dodgeChance = 0.4f;      // 30% chance to dodge
    public float detectionRadius = 15f;

    [Header("Audio")]
    public AudioClip jumpSound;
    private AudioSource audioSource;

    private NavMeshAgent agent;

    private Rigidbody rb;

    private bool isGrounded = true;

    private bool canJump = true;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        agent.updatePosition = true;
        agent.updateRotation = true;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        StartCoroutine(JumpRoutine());
    }

    void Update()
    {
        if (!player) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= detectionRadius)
        {
            if (isGrounded && !agent.isStopped)
                agent.SetDestination(player.position);
        }
    }

    IEnumerator JumpRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(jumpInterval);

            if (!isGrounded || player == null) continue;

            float random = Random.value;

            if (random < dodgeChance)
                StartCoroutine(Dodge());
            else
                StartCoroutine(JumpTowardPlayer());
        }
    }

    IEnumerator JumpTowardPlayer()
    {
        if (!isGrounded) yield break;

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0.4f;

        PerformJump(direction, jumpForce);
        yield return new WaitForSeconds(1f);
    }

    IEnumerator Dodge()
    {
        if (!isGrounded) yield break;

        Vector3 sideDir = transform.right * (Random.value > 0.5f ? 1 : -1);
        sideDir.y = 0.3f;

        PerformJump(sideDir, dodgeForce);
        yield return new WaitForSeconds(0.7f);
    }

    private void PerformJump(Vector3 direction, float force)
    {
        if (audioSource && jumpSound)
            audioSource.PlayOneShot(jumpSound);

        agent.isStopped = true;
        rb.isKinematic = false;
        rb.AddForce(direction * force, ForceMode.Impulse);
        isGrounded = false;

        Invoke(nameof(EnableNavMesh), 1f); 
    }

    private void EnableNavMesh()
    {
        rb.isKinematic = true;
        agent.isStopped = false;
        isGrounded = true;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }
}
