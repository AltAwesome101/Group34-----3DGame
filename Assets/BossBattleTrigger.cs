using UnityEngine;

public class BossBattleTrigger : MonoBehaviour
{
    [Header("Boss Setup")]
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;
    public GameObject entryDoor;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        StartBossBattle();
    }

    private void StartBossBattle()
    {
        if (bossPrefab && bossSpawnPoint)
        {
            GameObject boss = Instantiate(bossPrefab, bossSpawnPoint.position, bossSpawnPoint.rotation);

            BossEnemy bossScript = boss.GetComponent<BossEnemy>();
            if (bossScript)
                bossScript.entryDoor = entryDoor;
        }
      
    }
}
