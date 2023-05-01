using UnityEngine;
using static GameManager;
using Random = UnityEngine.Random;

/// <summary>
/// TODO: I NEED TO FIX THIS SCRIPT. THE SLASH STRUCT DOESN'T RETURN THE CORRECT VALUES BECAUSE I AM CREATING A NEW INSTANCE IN THIS SCRIPT, RATHER THAN USING THE VALUES
/// FROM THE SLASH STRUCT IN THE WEAPON-SYSTEM SCRIPT.
/// </summary>
public class SlashEffect : MonoBehaviour
{
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
            }
        }
    }

    void PerformSlash(Component other)
    {
        var slashStruct = new SlashParameters();

        Debug.Log("HIT ENEMY   |   " + slashStruct.SlashDamage); //TODO: this is a debug value
        var enemyComponent = other.gameObject.TryGetComponent<Enemy>(out var enemy) ? enemy : null;

        if (enemyComponent != null)
        { // Deal damage to the enemy.
            enemy.TakeDamage(slashStruct.SlashDamage = 1); // = 1 debug value

            // Generate a random offset for the knockback, then apply it to the enemy.
            var randomizedOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)); //TODO: Readjust this/re-code it.
            KnockbackRoutine(enemy.gameObject, enemy.transform.position - transform.position + randomizedOffset, slashStruct.KnockbackForce);
        }
        else { Debug.LogError("No 'Enemy' script found!"); }
    }
}