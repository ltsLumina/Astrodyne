using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SlashEffect : MonoBehaviour
{
    static MeleeSystem meleeSys;
    // Sätt ovan variabel från ett annat script en gång i början! :) Så bör den behållas över alla instanser<

    void Start()
    {
        meleeSys = FindObjectOfType<MeleeSystem>();
    }

    // Controlled through an animation event in the slash animation.
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
            Vector3 randomizedOffset = meleeSys.MousePlayerOffset + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
            MeleeSystem.KnockbackRoutine(enemy.gameObject, meleeSys.MousePlayerOffset + randomizedOffset, slashStruct.RecoilForce);
        }
        else { Debug.LogError("No 'Enemy' script found!"); }
    }
}