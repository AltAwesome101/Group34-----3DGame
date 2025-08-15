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

    public void SpawnPickup()
    {
        if (Random.value <= spawnChance)
        {
            GameObject[] pickups = { pickup1, pickup2, pickup3 };
            GameObject[] validPickups = System.Array.FindAll(pickups, p => p != null);
            if (validPickups.Length > 0)
            {
                GameObject selectedPickup = validPickups[Random.Range(0, validPickups.Length)];
                Instantiate(selectedPickup, transform.position + spawnOffset, Quaternion.identity);
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