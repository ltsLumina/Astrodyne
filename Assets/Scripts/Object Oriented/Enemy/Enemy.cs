using UnityEngine;
using static GameManager;

/// <summary>
/// TODO: REWORK THE ENEMY SCRIPT
/// </summary>
public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] int health;

    public int Health
    {
        get => health;
        set => health = value;
    }

    void Update()
    {
        // move towards player
        Vector2 playerPos = FindObjectOfType<Player>().transform.position;
        transform.position = Vector2.MoveTowards(transform.position, playerPos, 1f * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Player":
                Debug.Log("HIT PLAYER");
                var player = other.gameObject;

                // Take damage.
                player.GetComponent<Player>().CurrentHealth--;

                // Knockback the player and the enemy slightly
                var playerPos = player.transform.position;
                KnockbackRoutine(gameObject, transform.position - playerPos, 10);
                KnockbackRoutine(player, playerPos - transform.position, 10);
                break;

            case "Bullet":
                Debug.Log("HIT BY BULLET");
                var bullet = other.gameObject;

                // Take damage.
                TakeDamage(1);

                // Return the bullet to the pool.
                bullet.SetActive(false);

                // Knockback the enemy.
                KnockbackRoutine(gameObject, transform.position - bullet.transform.position, 25f);
                break;
        }
    }

    public void TakeDamage(int damage)
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