using System;
using UnityEngine;
using static GameManager;
using Random = UnityEngine.Random;

/// <summary>
/// The script that controls the slash effect.
/// </summary>
public class SlashEffect : MonoBehaviour
{
    // Fields
    bool isDashingAttacking;

    // Cached References
    SlashParameters slashParameters;
    WeaponDefinition weaponData;
    MeleeSystem meleeSystem;
    Dash dash;


    void Awake()
    {
        meleeSystem = FindObjectOfType<MeleeSystem>();
        dash        = FindObjectOfType<Dash>();
    }

    void OnEnable()
    {
        slashParameters    = meleeSystem.SlashData;
        weaponData         = meleeSystem.WeaponData;
        isDashingAttacking = dash.IsDashAttacking;
    }

    // Controlled through an animation event in the slash animation. (Assets\Animations\Player\Slash\Slash.anim)
    public void OnSlashHit()
    {
        var collider = GetComponent<PolygonCollider2D>();
        if (collider == null) return;

        var bounds = collider.bounds;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(bounds.center, bounds.size, 0f);
        foreach (Collider2D other in colliders)
        {
            if (other.CompareTag("Enemy")) PerformSlash(other);

            // Deprecated for the time being; will be re-implemented in a future update.
            //if (other.CompareTag("Bullet")) BoostProjectile(other);
        }
    }

    void PerformSlash(Component other)
    {
        Debug.Log($"Hit {other.gameObject.name} for {weaponData.damage} damage!");
        var enemyComponent = other.gameObject.TryGetComponent<Enemy>(out var enemy) ? enemy : null;

        if (enemyComponent != null)
        { // Deal damage to the enemy.
            enemy.TakeDamage(weaponData.damage);

            // Generate a random offset for the knockback, then apply it to the enemy.
            var randomizedOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)); //TODO: Readjust this/re-code it.
            Vector3 knockbackDir = enemy.transform.position - transform.position + randomizedOffset;

            //TODO: THIS WILL BE REWORKED IN THE FUTURE.
            // Knockback the enemy away from the player.
            KnockbackRoutine(enemy.gameObject,
                             knockbackDir,
                             isDashingAttacking ? slashParameters.dashAttackKnockbackForce : slashParameters.knockbackForce);
        }
        else { Debug.LogError("No 'Enemy' script found!"); }
    }

    [Obsolete] //TODO: Implement this in a future update.
    void BoostProjectile(Component other)
    {
        var bulletComponent = other.gameObject;

        var randomizedOffset = new Vector3(Random.Range(-20f, 20f), Random.Range(-20f, 20f)); //TODO: Readjust this/re-code it.
        KnockbackRoutine(bulletComponent.gameObject, bulletComponent.transform.position - transform.position + randomizedOffset, 10);

        // Activate multiple bullets.
        for (int i = 0; i < 3; i++)
        {
            //shootingSys.ActiveBullet.GetComponent<TrailRenderer>().startColor = Color.red;
            //shootingSys.ActiveBullet.transform.localScale = new (1.5f, 1.5f, 1.5f);
            //shootingSys.Attack();
        }

        CameraShake.Instance.ShakeCamera(5f, 0.2f);
    }
}