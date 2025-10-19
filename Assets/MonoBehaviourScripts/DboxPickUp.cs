using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [Header("Pickup Prefabs")]
    public GameObject pickup1;
    public GameObject pickup2;
    public GameObject pickup3;

    [Header("Settings")]
    [Range(0f, 1f)] public float spawnChance = 1f;
    public Vector3 spawnOffset = Vector3.up * 0.5f;

    [Header("Spawn Scale")]
    [Tooltip("Controls how large or small the spawned pickups appear.")]
    public float spawnScale = 1f; 

    public void SpawnPickup()
    {
        if (Random.value <= spawnChance)
        {
            GameObject[] pickups = { pickup1, pickup2, pickup3 };
            GameObject[] validPickups = System.Array.FindAll(pickups, p => p != null);

            if (validPickups.Length > 0)
            {
                GameObject selectedPickup = validPickups[Random.Range(0, validPickups.Length)];
                GameObject instance = Instantiate(selectedPickup, transform.position + spawnOffset, Quaternion.identity);
                instance.transform.localScale = Vector3.one * spawnScale;
            }
        }
    }

    private void OnDestroy()
    {
        if (Application.isPlaying)
        {
            SpawnPickup();
        }
    }
}
