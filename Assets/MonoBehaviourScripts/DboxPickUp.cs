using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [Header("Pickup Prefabs (Assign in Inspector)")]
    public GameObject pickup1;
    public GameObject pickup2;
    public GameObject pickup3;

    [Header("Settings")]
    [Range(0f, 1f)] public float spawnChance = 1f; // 1 = always spawn, 0.5 = 50% chance
    public Vector3 spawnOffset = Vector3.up * 0.5f; // small offset above cube

    /// <summary>
    /// Call this method when the cube is destroyed
    /// </summary>
    public void SpawnPickup()
    {
        if (Random.value <= spawnChance)
        {
            // Put pickups in an array for random selection
            GameObject[] pickups = { pickup1, pickup2, pickup3 };

            // Filter out any null slots in case some aren’t assigned
            GameObject[] validPickups = System.Array.FindAll(pickups, p => p != null);

            if (validPickups.Length > 0)
            {
                GameObject selectedPickup = validPickups[Random.Range(0, validPickups.Length)];
                Instantiate(selectedPickup, transform.position + spawnOffset, Quaternion.identity);
            }
        }
    }

    /// <summary>
    /// Example: Automatically spawn when destroyed
    /// </summary>
    private void OnDestroy()
    {
        // Avoid running in editor scene cleanup
        if (Application.isPlaying)
            SpawnPickup();
    }
}