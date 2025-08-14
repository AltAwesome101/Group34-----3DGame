using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public float attackRange = 1.5f;
    public float attackRadius = 1f;
    public int damage = 1;
    public LayerMask enemyLayer;
    public Transform attackPoint;
    public float attackCooldown = 0.5f;
    private float lastAttackTime = -100f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }

    void Attack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRadius, enemyLayer);
        foreach (Collider enemy in hitEnemies)
        {
            enemy.GetComponent<EnermyHealth>()?.TakeDamage(damage);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}

