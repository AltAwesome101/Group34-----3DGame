using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingScript : MonoBehaviour
{
    public GameObject bullet;
    public Transform firePoint;
    public float fireInterval = 0.3f;

    [Header("Shotgun Settings")]
    public float shotgunSpreadAngle = 30f;

    [Header("DualShot Settings")]
    public float dualShotOffset = 0.2f;

    private InventoryManager inventory;
    private PlayerInputActions inputActions;

    public enum GunType { Standard, DualShot, Shotgun }
    public GunType currentGun = GunType.Standard;
    private float lastShotTime = 0f;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.Shoot.performed += ctx => Fire();
    }

    void Start()
    {
        inventory = FindObjectOfType<InventoryManager>();
    }

    void Fire()
    {
        if (Time.timeScale == 0f || Time.time - lastShotTime < fireInterval)
            return;

        if (inventory != null && inventory.ammo > 0)
        {
            switch (currentGun)
            {
                case GunType.Standard:
                    SpawnBullet(firePoint.forward);
                    inventory.UseAmmo(1);
                    break;

                case GunType.DualShot:
                    SpawnBullet(firePoint.forward, firePoint.position + firePoint.right * dualShotOffset);
                    SpawnBullet(firePoint.forward, firePoint.position - firePoint.right * dualShotOffset);
                    inventory.UseAmmo(2);
                    break;

                case GunType.Shotgun:
                    SpawnBullet(firePoint.forward);
                    float step = shotgunSpreadAngle / 4f;
                    float angle = -shotgunSpreadAngle / 2f;
                    for (int i = 0; i < 4; i++)
                    {
                        Vector3 dir = Quaternion.AngleAxis(angle, firePoint.up) * firePoint.forward;
                        SpawnBullet(dir);
                        angle += step;
                    }
                    inventory.UseAmmo(5);
                    break;
            }

            lastShotTime = Time.time;
        }
    }

    void SpawnBullet(Vector3 direction, Vector3? customPos = null)
    {
        Vector3 spawnPos = customPos ?? firePoint.position;
        var instance = Instantiate(bullet, spawnPos, Quaternion.LookRotation(direction));
        Destroy(instance, 4f);
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();
}
