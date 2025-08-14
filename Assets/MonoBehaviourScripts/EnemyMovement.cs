using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    public Transform goal;
    private NavMeshAgent agent;
    public float kick = 2.0f;
    public float knockbackTime = 1;
    private bool hit;
    private ContactPoint contact;
    private float timer;
    void Start()
    {
        goal = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        timer = knockbackTime;
    }

    void Update()
    {
        agent.SetDestination(goal.position);
        if (hit)
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<NavMeshAgent>().isStopped = true;
            gameObject.GetComponent<Rigidbody>().AddForceAtPosition(Camera.main.transform.forward * kick, contact.point, ForceMode.Impulse);
            hit = false;
            timer = 0;
        }
        else 
        {
            timer += Time.deltaTime;

            if (knockbackTime < timer) 
            {
                gameObject.GetComponent<Rigidbody>().isKinematic = true;
                gameObject.GetComponent<NavMeshAgent>().isStopped = false;
                agent.SetDestination(goal.position);
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("bullet")) 
        {
            contact = other.contacts[0];
            hit = true;
        }
    }
}
