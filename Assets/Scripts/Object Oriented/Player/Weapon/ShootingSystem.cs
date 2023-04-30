using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Essentials;
using UnityEngine;

/// <summary>
/// Fires bullets from the player's weapon to the mouse position.
/// </summary>
public class ShootingSystem : MonoBehaviour
{
    #region Configurable Parameters
    [Header("Shooting Parameters"), Tooltip("Parameters that govern the shooting of the weapon.")]
    [SerializeField] float fireRate;

    [Header("Bullet Fields")]
    [SerializeField] float bulletLifetime;
    [SerializeField] float bulletForce;

    [Header("Read-only Fields"), SerializeField, ReadOnly]
    float timeSinceLastShot;
    #endregion


    #region Cached References
    [Header("Cached References")]
    [SerializeField] GameObject bulletPrefab;
    Camera cam;
    MeleeSystem meleeSys;
    ObjectPool bulletPool;
    #endregion

    // Delegate for when the player shoots.
    public delegate void OnShoot();
    public event OnShoot onShoot;

    void Start()
    {
        // Cache references.
        cam = Camera.main;
        meleeSys = GetComponent<MeleeSystem>();

        // Create the bullet pool and set it up
        bulletPool = ObjectPoolManager.CreateNewPool(bulletPrefab, 7);
        bulletPool.gameObject.name = "Bullet Pool";

        // Subscribe to the onShoot event.
        onShoot += Shooting;

        // Assertions:
        // Return bullet to pool after bulletLifetime seconds. Also asserting that the bulletLifetime is greater than 0.
        Debug.Assert(bulletLifetime > 0, "Bullet lifetime is less than or equal to 0. " +
                                         "This will cause the bullet to never be returned to the pool.");
    }

    // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
    // Increments the time since the last shot, and while it is larger than the fireRate, the player can shoot.
    void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        if (Input.GetMouseButton(0) && timeSinceLastShot > fireRate && bulletPool != null)
            // Raise the onShoot event.
            onShoot?.Invoke();
    }

    void Shooting() //TODO: THERE IS KNOCKBACK? // figured it out, the bullet is hitting the player and pushing them back.
    {
        // Enter combat.
        CombatManager.Instance.EnterCombat();

        // Initialize the bullet before method call to to avoid closure allocation.
        GameObject pooledBullet = bulletPool.GetPooledObject();
        Debug.Log(pooledBullet);

        // If the pooled bullet is null, return.
        try
        {
            if (pooledBullet == null) return;
        } catch (Exception e)
        {
            Debug.Log("Pooled bullet is null! \n" + e);
            throw;
        }

        // Activate the bullet, and set its position and rotation.
        pooledBullet.GetComponent<TrailRenderer>().Clear(); //TODO: Probably a bad idea calling GetComponent every time, but it works for now.
        pooledBullet = bulletPool.GetPooledObject(true);

        // Set the position to the shooting position plus an offset relative to the mouse position.
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        // Set the bullet's position and rotation.
        pooledBullet.transform.position = meleeSys.HitPoint.transform.position + (mousePos - transform.position).normalized * 0.5f;
        pooledBullet.transform.rotation = transform.rotation;

        // Add force to the bullet.
        var bulletRB = pooledBullet.GetComponent<Rigidbody2D>();
        bulletRB.AddForce(transform.up * bulletForce, ForceMode2D.Impulse);

        PostShooting();

        void PostShooting()
        { // Return the bullet to the pool after bulletLifetime seconds.
            Task delayTask = UsefulMethods.DelayedTaskAsync(() => pooledBullet.SetActive(false), bulletLifetime).AsTask();
            delayTask.ContinueWith(_ => Debug.Log("Bullet returned to pool!"));

            // Reset the time since the last shot.
            timeSinceLastShot = 0;
        }
    }
}