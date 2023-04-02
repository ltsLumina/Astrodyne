using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Debug;
using static Essentials.UsefulMethods;
using ReadOnly = Essentials.ReadOnlyAttribute;

public class Weapon : MonoBehaviour
{
    #region Serialized Fields
    [Header("Cached References")]
    Camera cam;
    Transform playerPos;
    Player player;
    ObjectPool bulletPool;

    CancellationTokenSource cancellationToken;

    [Header("Shooting Parameters"), Tooltip("Parameters that govern the shooting of the weapon."), ReadOnly]
    [SerializeField] float timeSinceLastShot;
    [SerializeField] float fireRate;

    [Header("Bullet Fields")]
    [SerializeField] float bulletLifetime;
    [SerializeField] float bulletForce;
    #endregion

    void Start()
    {
        cam               = Camera.main;
        playerPos         = FindObjectOfType<Player>().transform;
        player            = FindObjectOfType<Player>();
        bulletPool        = FindObjectOfType<ObjectPool>();
        cancellationToken = new CancellationTokenSource();

        // Assertions:
        // Return bullet to pool after bulletLifetime seconds. Also asserting that the bulletLifetime is greater than 0.
        Assert(bulletLifetime > 0, "Bullet lifetime is less than or equal to 0. This will cause the bullet to never be returned to the pool.");
    }

    void Update()
    {
        if (!player.IsDead) WeaponLogic();
    }


    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// Handles all the logic for the weapon such as the rotation, position, and shooting.
    /// </summary>
    void WeaponLogic()
    {
        HandleRotation();

        void HandleRotation()
        {
            // Converts the mousePos from Screen values to World values.
            Vector2 mousePos   = cam.ScreenToWorldPoint(Input.mousePosition);

            // Rotates the object in relation to the mouse position.
            Vector2 direction  = mousePos - (Vector2)playerPos.position;
            float   angle      = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle + 270, Vector3.forward);
        }

        HandleWeaponPosition();

        void HandleWeaponPosition()
        {
            // Moves the weapon slightly to the left or right, depending on if the player isFacingRight.
            transform.position = player.IsFacingRight
                ? (Vector2)playerPos.position + new Vector2(-0.3f, 0.5f)
                : (Vector2)playerPos.position + new Vector2(0.3f, 0.5f);
        }

        HandleShooting();

        void HandleShooting()
        {
            // Increments the time since the last shot, and while it is larger than the fireRate, the player can shoot.
            timeSinceLastShot += Time.deltaTime;

            if (!Input.GetMouseButton(0) || !(timeSinceLastShot > fireRate)) return;
            if (bulletPool == null) return;

            // Initialize the bullet before method call to to avoid closure allocation.
            GameObject pooledBullet = bulletPool.GetPooledObject();
            if (pooledBullet == null) return;

            // Activate the bullet, and set its position and rotation.
            pooledBullet.GetComponent<TrailRenderer>().Clear(); //TODO: Probably a bad idea calling GetComponent every time, but it works for now.
            pooledBullet = bulletPool.GetPooledObject(true);
            pooledBullet.transform.position = transform.position;
            pooledBullet.transform.rotation = transform.rotation;

            // Add force to the bullet.
            Rigidbody2D bulletRB = pooledBullet.GetComponent<Rigidbody2D>();
            bulletRB.AddForce(transform.up * bulletForce, ForceMode2D.Impulse);

            // Return the bullet to the pool after bulletLifetime seconds.
            var delayTask = DelayedTaskAsync(() => pooledBullet.SetActive(false), bulletLifetime).AsTask();
            delayTask.ContinueWith(_ => Log("Bullet returned to pool!"));

            // Reset the time since the last shot.
            timeSinceLastShot = 0;
        }
    }
}