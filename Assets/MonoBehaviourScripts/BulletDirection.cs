using UnityEngine;

public class BulletDirection : MonoBehaviour
{
    public float speed = 10f;
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}
