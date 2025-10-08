//Title: Shooting Mechanics In Unity
//Author: Kieran Coughlan
//Date: 12-04-2016
//Code Version: New-input System
//Availability:https://coderdojoathenry.org/2016/04/12/shooting-mechanics-in-unity/

using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ShootingScript : MonoBehaviour
{
    [Header("Bullet Settings")]
    [Tooltip("Prefab of the bullet to spawn.")]
    public GameObject bulletPrefab;

    [Tooltip("Where bullets will be fired from.")]
    public Transform firePoint;

    [Tooltip("How many bullets to keep in the pool.")]
    public int poolSize = 400;

    [Tooltip("Minimum time (seconds) between shots.")]
    public float fireInterval = 0.3f;
    public enum GunType { Standard, DualShot, Shotgun }

    [Header("Gun Modes")]
    [Tooltip("Which gun is currently active.")]
    public GunType currentGun = GunType.Standard;

    [Tooltip("Horizontal offset for dual shot bullets.")]
    public float dualShotOffset = 0.2f;

    [Tooltip("Total spread angle for shotgun pellets.")]
    public float shotgunSpreadAngle = 30f;


    private InventoryManager inventory;

    private PlayerInputActions input;

    private float lastShotTime;

    private List<GameObject> pool;

    private int poolIndex;

    private void Awake()
    {
        
        input = new PlayerInputActions();
        input.Player.Shoot.performed += _ => TryFire();
        pool = new List<GameObject>(poolSize);
        for (int i = 0; i < poolSize; i++)
        {
            var b = Instantiate(bulletPrefab);
            b.SetActive(false);
            pool.Add(b);
        }
    }

    private void Start()
    {
        inventory = FindObjectOfType<InventoryManager>();
    }

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();
    private void TryFire()
    {
        if (Time.timeScale == 0f || Time.time - lastShotTime < fireInterval)
            return;

        if (inventory == null || inventory.ammo <= 0)
            return;

        switch (currentGun)
        {
            case GunType.Standard:
                SpawnBullet(firePoint.position, firePoint.forward);
                inventory.UseAmmo(1);
                break;

            case GunType.DualShot:
                SpawnBullet(firePoint.position + firePoint.right * dualShotOffset, firePoint.forward);
                SpawnBullet(firePoint.position - firePoint.right * dualShotOffset, firePoint.forward);
                inventory.UseAmmo(2);
                break;

            case GunType.Shotgun:
                float step = shotgunSpreadAngle / 4f;
                float angle = -shotgunSpreadAngle / 2f;
                for (int i = 0; i < 5; i++)
                {
                    Vector3 dir = Quaternion.AngleAxis(angle, firePoint.up) * firePoint.forward;
                    SpawnBullet(firePoint.position, dir);
                    angle += step;
                }
                inventory.UseAmmo(5);
                break;
        }

        lastShotTime = Time.time;
    }

    private void SpawnBullet(Vector3 pos, Vector3 dir)
    {
       
        GameObject bullet = pool[poolIndex];
        poolIndex = (poolIndex + 1) % pool.Count;
        bullet.transform.SetPositionAndRotation(pos, Quaternion.LookRotation(dir));
        bullet.SetActive(false);
        bullet.SetActive(true);

    }
}
