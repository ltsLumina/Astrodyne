using UnityEngine;
using Random = UnityEngine.Random;

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
        var slashStruct = new SlashStruct();

        Debug.Log("HIT ENEMY");
        var enemyComponent = other.gameObject.TryGetComponent<Enemy>(out var enemy) ? enemy : null;

        if (enemyComponent != null)
        { // Deal damage to the enemy.
            enemy.TakeDamage(slashStruct.AttackDamage = 1); // = 1 debug value

            // Generate a random offset for the knockback, then apply it to the enemy.
            var randomizedOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)); //TODO: Readjust this/re-code it.
            MeleeSystem.KnockbackRoutine(enemy.gameObject, enemy.transform.position - transform.position + randomizedOffset, slashStruct.RecoilForce);
        }
        else { Debug.LogError("No 'Enemy' script found!"); }
    }
}