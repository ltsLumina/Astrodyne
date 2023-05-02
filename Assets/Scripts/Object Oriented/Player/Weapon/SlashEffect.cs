using Cysharp.Threading.Tasks;
using UnityEngine;
using static GameManager;
using Random = UnityEngine.Random;

/// <summary>
/// TODO: RE-FORMAT THIS SCRIPT TO ALLOW FOR SERIALIZED FIELDS SOMEWHERE. I.e, I WANT TO BE ABLE TO ADJUST THE BOOSTED PROJECTILE SPEED IN THE UNITY EDITOR.
/// </summary>
public class SlashEffect : MonoBehaviour
{
    MeleeSystem meleeSys;
    ShootingSystem shootingSys;
    WeaponSystem weaponSys;

    void OnEnable()
    { // Get the slash struct from the player.
        meleeSys    = FindObjectOfType<MeleeSystem>();
        shootingSys = meleeSys.GetComponent<ShootingSystem>();
    }

    // Controlled through an animation event in the slash animation. (Assets\Animations\Player\Slash\Slash.anim)
    public void OnSlashHit()
    {
        var collider = GetComponent<PolygonCollider2D>();
        if (collider != null)
        {
            var bounds = collider.bounds;

            Collider2D[] colliders = Physics2D.OverlapBoxAll(bounds.center, bounds.size, 0f);
            foreach (Collider2D other in colliders)
            {
                if (other.CompareTag("Enemy")) PerformSlash(other);

                if (other.CompareTag("Bullet")) BoostProjectile(other);
            }
        }
    }

    void PerformSlash(Component other)
    {
        Debug.Log("HIT ENEMY   |   " + meleeSys.SlashDamage); //TODO: this is a debug value
        var enemyComponent = other.gameObject.TryGetComponent<Enemy>(out var enemy) ? enemy : null;

        if (enemyComponent != null)
        { // Deal damage to the enemy.
            enemy.TakeDamage(meleeSys.SlashDamage);

            // Generate a random offset for the knockback, then apply it to the enemy.
            var randomizedOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)); //TODO: Readjust this/re-code it.
            KnockbackRoutine(enemy.gameObject, enemy.transform.position - transform.position + randomizedOffset, meleeSys.KnockbackForce);
        }
        else { Debug.LogError("No 'Enemy' script found!"); }
    }

    void BoostProjectile(Component other)
    {
        var bulletComponent = other.gameObject;

        var randomizedOffset = new Vector3(Random.Range(-20f, 20f), Random.Range(-20f, 20f)); //TODO: Readjust this/re-code it.
        KnockbackRoutine(bulletComponent.gameObject, bulletComponent.transform.position - transform.position + randomizedOffset, 10);

        // Activate multiple bullets.
        for (int i = 0; i < 3; i++)
        {
            shootingSys.ActiveBullet.GetComponent<TrailRenderer>().startColor = Color.red;
            shootingSys.ActiveBullet.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            shootingSys.Attack();
        }

        CameraShake.Instance.ShakeCamera(5f, 0.2f);
    }
}