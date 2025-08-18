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
    [Header("Bullet & Fire Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireInterval = 0.3f;
    public int poolSize = 50;

    [Header("Shotgun Settings")]
    public float shotgunSpreadAngle = 30f;

    [Header("DualShot Settings")]
    public float dualShotOffset = 0.2f;

    private InventoryManager inventory;

    private PlayerInputActions inputActions;

    public enum GunType { Standard, DualShot, Shotgun }

    public GunType currentGun = GunType.Standard;

    private float lastShotTime = 0f;

    private Queue<GameObject> bulletPool = new Queue<GameObject>();

    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.Shoot.performed += ctx => Fire();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(bulletPrefab);
            obj.SetActive(false);
            bulletPool.Enqueue(obj);
        }
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
        GameObject bullet = bulletPool.Count > 0 ? bulletPool.Dequeue() : Instantiate(bulletPrefab);
        bullet.transform.SetPositionAndRotation(spawnPos, Quaternion.LookRotation(direction));
        bullet.SetActive(true);
        StartCoroutine(DisableAfterTime(bullet, 4f));
    }

    System.Collections.IEnumerator DisableAfterTime(GameObject bullet, float time)
    {
        yield return new WaitForSeconds(time);
        bullet.SetActive(false);
        bulletPool.Enqueue(bullet);
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();
}
