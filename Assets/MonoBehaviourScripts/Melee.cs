using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeAttack : MonoBehaviour
{
    public float attackRadius = 1f;

    public int damage = 1;

    public LayerMask enemyLayer;

    public Transform attackPoint;

    public float attackCooldown = 0.5f;

    private float lastAttackTime = -100f;

    public GameObject hitEffectPrefab;

    private PlayerInputActions inputActions;

    void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.Melee.performed += OnMelee;
    }

    void OnDisable()
    {
        inputActions.Player.Melee.performed -= OnMelee;
        inputActions.Player.Disable();
    }

    private void OnMelee(InputAction.CallbackContext ctx)
    {
        PerformAttack();
    }
    private void PerformAttack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
        }
    }
    private void Attack()
    {
        Collider[] hitObjects = Physics.OverlapSphere(attackPoint.position, attackRadius, enemyLayer);
        foreach (Collider obj in hitObjects)
        {
            EnermyHealth enemyHealth = obj.GetComponent<EnermyHealth>();
            
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage, true);
            }

            Destroyable destroyable = obj.GetComponent<Destroyable>();
            if (destroyable != null)
            {
                destroyable.RegisterHit();
            }

            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, obj.transform.position + Vector3.up * 1f, Quaternion.identity);
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}