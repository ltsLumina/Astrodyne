using Essentials;
using Unity.VisualScripting;
using UnityEngine;
using static GameManager;

/// <summary>
/// TODO: REWORK THE ENEMY SCRIPT.
/// May 4th UPDATE: Work has been done. Not complete.
/// </summary>
public abstract class Enemy : MonoBehaviour, IDamageable
{
    [Header("Enemy Data")]
    [SerializeField] EnemyDataType enemyData;
    [SerializeField] int health;

    [Header("Read-only Fields"), SerializeField, ReadOnly]
    float timeSinceLastAttack;

    WeaponDefinition shootingData;

    public int Health
    {
        get => health;
        set => health = value;
    }

    public EnemyDataType EnemyData
    {
        get => enemyData;
        set => enemyData = value;
    }

    protected abstract bool CanMove();

    protected virtual void Start()
    {
        shootingData = FindObjectOfType<ShootingSystem>().WeaponData;

        // Set the health to the enemy data's health.
        Health = EnemyData.Health;
    }

    protected virtual void Update()
    {
        timeSinceLastAttack += Time.deltaTime;
    }

    // The method to attack.
    protected virtual void OnTriggerEnter2D(Collider2D other) => EnemyAttack(other);

    // TODO: Handle enemies taking damage in a separate class or interface.
    void EnemyAttack(Component other)
    {
        switch (other.gameObject.tag)
        {
            case "Player" when timeSinceLastAttack > EnemyData.AttackDelay:

                timeSinceLastAttack = 0;
                Debug.Log("HIT PLAYER");

                var player = other.gameObject;

                // Take damage.
                player.TryGetComponent(out IDamageable hit);
                hit.Damage(EnemyData.Damage);

                // Get the player's position.
                var playerPos = player.transform.position;

                // Knockback the player and the enemy slightly
                KnockbackRoutine(gameObject, transform.position - playerPos, 50);
                KnockbackRoutine(player, playerPos - transform.position, 25);
                break;

            case "Bullet":
                var bullet = other.gameObject;

                // Take damage.
                TakeDamage(shootingData.damage);

                // Return the bullet to the pool.
                bullet.SetActive(false);

                // Knockback the enemy.
                KnockbackRoutine(gameObject, transform.position - bullet.transform.position, 25f);
                break;
        }
    }

    public virtual void TakeDamage(int damage)
    {
        if (TryGetComponent(out IDamageable hit))
        {
            hit.Damage(damage);
            Debug.Log($"Hit enemy for {damage} damage!");
        }

        StartCoroutine(PlayerAnimationManager.SpriteRoutine(0.5f, GetComponent<SpriteRenderer>()));

        if (Health <= 0) Destroy(gameObject);
    }
}