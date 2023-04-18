using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Debug;
using static Essentials.UsefulMethods;
using ReadOnly = Essentials.ReadOnlyAttribute;

public class Weapon : MonoBehaviour
{
    #region Cached References
    [Header("Cached References")]
    Camera cam;
    Transform playerPos;
    Player player;
    SpriteRenderer sprite;
    ObjectPool bulletPool;
    #endregion

    #region Configurable Parameters
    [Header("Shooting Parameters"), Tooltip("Parameters that govern the shooting of the weapon.")]
    [SerializeField] float fireRate;
    [SerializeField] float combatDuration = 10f;

    [Header("Bullet Fields")]
    [SerializeField] float bulletLifetime;
    [SerializeField] float bulletForce;

    [Header("Read-Only Fields")]
    [SerializeField, ReadOnly] float timeSinceLastShot;
    [SerializeField, ReadOnly] float combatTime;
    #endregion

    // Delegate for when the player shoots.
    public delegate void OnShoot();
    public event OnShoot onShoot;

    public float CombatTime
    {
        get => combatTime;
        set => combatTime = value;
    }

    void Start()
    {
        cam            = Camera.main;
        playerPos      = FindObjectOfType<Player>().transform;
        player         = FindObjectOfType<Player>();
        sprite         = GetComponent<SpriteRenderer>();
        bulletPool     = FindObjectOfType<ObjectPool>();

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
        //HandleRotation();
        FaceMouse();
        //HandleWeaponPosition();
        HandleShooting();

        if (Input.GetKeyDown(KeyCode.O))
        {
            // disable all the trail renderers
            foreach (TrailRenderer trail in GetComponentsInChildren<TrailRenderer>())
            {
                trail.enabled = false;
            }
        }

        //TODO: MAKE COMBO SCRIPT
        //USE THIS: https://www.youtube.com/watch?v=Jm0mbHEFPfE
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GetComponentInChildren<TrailRenderer>().Clear();
            GetComponentInChildren<TrailRenderer>().enabled = true;
            GetComponent<Animator>().SetTrigger("doAttack1");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GetComponentInChildren<TrailRenderer>().Clear();
            GetComponentInChildren<TrailRenderer>().enabled = true;
            GetComponent<Animator>().SetTrigger("doAttack2");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GetComponentInChildren<TrailRenderer>().Clear();
            GetComponentInChildren<TrailRenderer>().enabled = true;
            GetComponent<Animator>().SetTrigger("doAttack3");
        }
    }

    void HandleRotation()
    {
        // Converts the mousePos from Screen values to World values.
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        // Rotates the object in relation to the mouse position.
        Vector2 direction  = mousePos - (Vector2)playerPos.position;
        float   angle      = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle + 270, Vector3.forward);
    }

    void FaceMouse()
    { // Smoothly rotates player to face mouse.
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        // Flip the sprite if the mouse is on the left side of the player
        sprite.flipX = mousePos.x < transform.position.x;
    }

    void HandleWeaponPosition()
    {
        // Moves the weapon slightly to the left or right, depending on if the player isFacingRight.
        transform.position = player.IsFacingRight
            ? (Vector2) playerPos.position + new Vector2(-0.3f, 0.5f)
            : (Vector2) playerPos.position + new Vector2(0.3f, 0.5f);
    }

    void HandleShooting() //TODO: THERE IS KNOCKBACK? // figured it out, the bullet is hitting the player and pushing them back.
    {
        // Increments the time since the last shot, and while it is larger than the fireRate, the player can shoot.
        timeSinceLastShot += Time.deltaTime;
        onShoot?.Invoke();

        if (!Input.GetMouseButton(0) || !(timeSinceLastShot > fireRate)) return;
        if (bulletPool == null) return;

        // Enter combat and begin to countdown the combat timer.
        CombatTime = combatDuration; // TODO: Make this a configurable parameter.

        // Initialize the bullet before method call to to avoid closure allocation.
        GameObject pooledBullet = bulletPool.GetPooledObject();
        if (pooledBullet == null) return;

        // Activate the bullet, and set its position and rotation.
        pooledBullet.GetComponent<TrailRenderer>().Clear(); //TODO: Probably a bad idea calling GetComponent every time, but it works for now.
        pooledBullet = bulletPool.GetPooledObject(true);
        pooledBullet.transform.position = transform.position;
        pooledBullet.transform.rotation = transform.rotation;

        // Add force to the bullet.
        var bulletRB = pooledBullet.GetComponent<Rigidbody2D>();
        bulletRB.AddForce(transform.up * bulletForce, ForceMode2D.Impulse);

        PostShooting();

        void PostShooting()
        { // Return the bullet to the pool after bulletLifetime seconds.
            Task delayTask = DelayedTaskAsync(() => pooledBullet.SetActive(false), bulletLifetime).AsTask();
            delayTask.ContinueWith(_ => Log("Bullet returned to pool!"));

            // Reset the time since the last shot.
            timeSinceLastShot = 0;
        }
    }

    public bool IsInCombat()
    {
        if (CombatTime > 0)
        {
            CombatTime -= Time.deltaTime;
        }

        return CombatTime > 0;
    }
}