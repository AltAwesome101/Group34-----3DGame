
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class Enemy
{
    public GameObject go;
    public bool active;

    public Enemy(GameObject newGo, bool newBool)
    {
        go = newGo;
        active = newBool;
    }
}

public class EnemySpawner : MonoBehaviour
{
    public GameObject spawn; // Enemy prefab
    public int amount = 1;   // How many to spawn
    public float delaySpawn = 1f; // Delay between spawns
    public bool spawnsDead;

    private int getAmount;
    private float timer;
    private int spawned;

    public List<Enemy> enemies = new List<Enemy>();

    private void Start()
    {
        ResetRound(); // Will create initial pool
    }

    public void ResetRound()
    {
        spawnsDead = false;
        getAmount = amount;
        spawned = 0;
        timer = 0;

        // Always ensure pool matches amount
        while (enemies.Count < getAmount)
        {
            GameObject instance = Instantiate(spawn, transform.position, Quaternion.identity);
            instance.transform.parent = null;
            instance.SetActive(false);
            enemies.Add(new Enemy(instance, false));
        }

        // Disable all enemies before starting
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].go.SetActive(false);
            enemies[i].active = false;
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        // Spawn enemies over time
        if (timer >= delaySpawn && spawned < getAmount)
        {
            timer = 0f;

            enemies[spawned].go.transform.position = transform.position;
            enemies[spawned].go.SetActive(true);
            enemies[spawned].active = true;

            StartCoroutine(SetKinematic(spawned));
            spawned++;
        }

        // Check if all spawned enemies are dead
        int deadCount = 0;
        for (int i = 0; i < Mathf.Min(getAmount, enemies.Count); i++)
        {
            if (!enemies[i].go.activeSelf && enemies[i].active)
            {
                enemies[i].active = false;
                deadCount++;
            }
        }

        if (deadCount == getAmount && spawned == getAmount)
        {
            spawnsDead = true;
        }
    }

    private IEnumerator SetKinematic(int id)
    {
        yield return null; // Wait 1 frame to ensure physics is set up

        Rigidbody rb = enemies[id].go.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (spawn != null && spawn.GetComponent<MeshFilter>() != null)
        {
            Gizmos.DrawWireMesh(
                spawn.GetComponent<MeshFilter>().sharedMesh,
                transform.position,
                transform.rotation,
                Vector3.one
            );
        }
    }
}