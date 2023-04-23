using System;
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
    MeleeComboSystem meleeSys;
    SlashEffect slashEffect;
    #endregion

    #region Configurable Parameters
    [Header("Shooting Parameters"), Tooltip("Parameters that govern the shooting of the weapon.")]
    [SerializeField] float fireRate;
    [SerializeField] float defaultCombatTime = 10f;

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
        set
        {
            combatTime = value;
            combatTime = Mathf.Clamp(combatTime, 0, defaultCombatTime);
        }
    }

    void Start()
    {
        cam            = Camera.main;
        playerPos      = FindObjectOfType<Player>().transform;
        player         = FindObjectOfType<Player>();
        sprite         = transform.GetChild(0).GetComponent<SpriteRenderer>(); // TODO: Make this more robust, rather than accessing by index.
        bulletPool     = FindObjectOfType<ObjectPool>();
        meleeSys       = GetComponent<MeleeComboSystem>();
        slashEffect    = GetComponentInChildren<SlashEffect>();

        onShoot += EnterCombat;

        // Assertions:
        // Return bullet to pool after bulletLifetime seconds. Also asserting that the bulletLifetime is greater than 0.
        Assert(bulletLifetime > 0, "Bullet lifetime is less than or equal to 0. " +
                                   "This will cause the bullet to never be returned to the pool.");
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
        HandleWeaponPosition();
        Shooting();

        meleeSys.MeleeCombat();
    }

    void HandleRotation()
    {
        // Converts the mousePos from Screen values to World values.
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        // Rotates the object in relation to the mouse position.
        Vector2 direction  = mousePos - (Vector2)playerPos.position;
        float   angle      = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        slashEffect.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void HandleWeaponPosition()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        // Rotates the object in relation to the mouse position.
        Vector2 direction  = mousePos - (Vector2)playerPos.position;

        transform.up = direction;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.GetChild(0).rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        slashEffect.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void Shooting() //TODO: THERE IS KNOCKBACK? // figured it out, the bullet is hitting the player and pushing them back.
    {
        // Increments the time since the last shot, and while it is larger than the fireRate, the player can shoot.
        timeSinceLastShot += Time.deltaTime;

        if (!Input.GetMouseButton(0) || !(timeSinceLastShot > fireRate) || bulletPool == null) return;

        // Raise the onShoot event.
        onShoot?.Invoke();

        // Initialize the bullet before method call to to avoid closure allocation.
        GameObject pooledBullet = bulletPool.GetPooledObject();
        try {
            if (pooledBullet == null) return;
        } catch (Exception e) {
            Log("Pooled bullet is null! \n" + e);
            throw;
        }

        // Activate the bullet, and set its position and rotation.
        pooledBullet.GetComponent<TrailRenderer>().Clear(); //TODO: Probably a bad idea calling GetComponent every time, but it works for now.
        pooledBullet = bulletPool.GetPooledObject(true);

        //pooledBullet.transform.position = transform.position;
        // Set the position to the weapons position plus an offset relative to the mouse position.
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        pooledBullet.transform.position = transform.position + (mousePos - transform.position).normalized * 0.5f;

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

    public void EnterCombat() => CombatTime = defaultCombatTime;

    public bool IsInCombat()
    {
        if (CombatTime > 0)
            CombatTime -= Time.deltaTime;

        return CombatTime > 0;
    }
}