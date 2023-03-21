using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Debug;
using static UsefulMethods;

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
        cam        = Camera.main;
        playerPos  = FindObjectOfType<Player>().transform;
        player     = FindObjectOfType<Player>();
        bulletPool = FindObjectOfType<ObjectPool>();

        // Assertions:
        // Return bullet to pool after bulletLifetime seconds. Also asserting that the bulletLifetime is greater than 0.
        Assert(bulletLifetime > 0, "Bullet lifetime is less than or equal to 0. This will cause the bullet to never be returned to the pool.");
    }

    void Update()
    {
        if (!player.IsDead) WeaponLogic();
    }

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
            // Using fixedDeltaTime instead of deltaTime as it felt better; might be a placebo.
            timeSinceLastShot += Time.fixedDeltaTime;

            if (!Input.GetMouseButton(0) || !(timeSinceLastShot > fireRate)) return;
            if (bulletPool == null) return;

            // Get a bullet from the object pool.
            GameObject pooledBullet = bulletPool.GetPooledObject(true);
            pooledBullet.transform.position = transform.position;
            pooledBullet.transform.rotation = transform.rotation;

            // Add force to the bullet.
            Rigidbody2D bulletRB = pooledBullet.GetComponent<Rigidbody2D>();
            bulletRB.AddForce(transform.up * bulletForce, ForceMode2D.Impulse);

            // Reset the time since the last shot.
            timeSinceLastShot = 0;

            // Pass the cancellation token to the DoAfterDelay method
            DoAfterDelay(() => pooledBullet.SetActive(false), bulletLifetime);

            //TODO: As it stands right now, the cancellationToken cancels the task which ruins the objectpool.
            //TODO: As a whole, the issue is that if I exit the game while *A* bullet is still active, unity logs a MissingReferenceException error.
            //TODO: Apparently it also still breaks every other time I enter runtime.
        }
    }
}