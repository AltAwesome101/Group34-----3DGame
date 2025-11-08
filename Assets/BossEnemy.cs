using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class BossEnemy : MonoBehaviour
{
    [Header("Boss Settings")]
    public int maxHits = 20;
    public AudioClip screamClip;
    public GameObject[] minionPrefabs;
    public Transform[] spawnPoints;
    public GameObject entryDoor; 

    [Header("Chase Settings")]
    public float detectionRadius = 25f;
    public float stopChasingRadius = 35f;

    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip deathSound;
    public AudioClip aggroSound;

    [Header("Movement")]
    public float patrolRadius = 10f;
    public float patrolWaitTime = 3f;
    public float hoverAmplitude = 0.5f;
    public float hoverFrequency = 2f;

    private int hitNumber = 0;
    private bool isDead = false;
    private bool isChasing = false;
    private bool hasSpawnedMinions = false;
    private float hoverOffset;
    private Vector3 startPosition;

    private NavMeshAgent agent;
    private Transform player;
    private AudioSource audioSource;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        startPosition = transform.position;
        hoverOffset = Random.Range(0f, Mathf.PI * 2);

        InvokeRepeating(nameof(CheckDistance), 0f, 0.3f);
    }

    private void Update()
    {
        HoverEffect();

        if (isChasing && !isDead)
            agent.SetDestination(player.position);
    }

    private void HoverEffect()
    {
        Vector3 pos = transform.position;
        pos.y = startPosition.y + Mathf.Sin(Time.time * hoverFrequency + hoverOffset) * hoverAmplitude;
        transform.position = pos;
    }

    private void CheckDistance()
    {
        if (isDead || player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (!isChasing && dist <= detectionRadius)
        {
            isChasing = true;
            if (aggroSound) audioSource.PlayOneShot(aggroSound);
        }
        else if (isChasing && dist > stopChasingRadius)
        {
            isChasing = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (isDead) return;

        if (other.transform.CompareTag("bullet"))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        hitNumber += amount;
        if (hitSound) audioSource.PlayOneShot(hitSound);

     
        if (!hasSpawnedMinions && hitNumber >= 10)
        {
            hasSpawnedMinions = true;
            SoundPlayer2D.Play2D(screamClip, 1f);
            SpawnMinions();
        }

      
        if (hitNumber >= maxHits)
        {
            Die();
        }
    }

    private void SpawnMinions()
    {
        foreach (Transform spawn in spawnPoints)
        {
            if (minionPrefabs.Length > 0)
            {
                GameObject minion = Instantiate(
                    minionPrefabs[Random.Range(0, minionPrefabs.Length)],
                    spawn.position,
                    spawn.rotation
                );
            }
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        if (deathSound) audioSource.PlayOneShot(deathSound);

        float delay = deathSound ? deathSound.length : 0f;
        Invoke(nameof(OnBossDefeated), delay);
    }

    private void OnBossDefeated()
    {
        if (entryDoor != null)
            Destroy(entryDoor);

        Destroy(gameObject);
    }
}
