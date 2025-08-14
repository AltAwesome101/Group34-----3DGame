using UnityEngine;

public class Destroyable : MonoBehaviour
{
    public int maxHits = 3;
    private int currentHits = 0;

    public void RegisterHit()
    {
        currentHits++;

        if (currentHits >= maxHits)
        {
            Destroy(gameObject);
        }
    }
}