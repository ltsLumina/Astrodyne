using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SlashEffect : MonoBehaviour
{
    [SerializeField] int attackDamage;
    [SerializeField] float slashSize;
    [SerializeField] float recoilForce = 10f;

    MeleeSystem parent;
    SpriteRenderer slashSprite;

    void Start()
    {
        parent      = GetComponentInParent<MeleeSystem>();
        slashSprite = GetComponent<SpriteRenderer>();
    }

    void Update() => transform.localScale = new Vector3(slashSize, slashSize, 1f);

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
        Debug.Log("HIT ENEMY");
        var enemyComponent = other.gameObject.TryGetComponent<Enemy>(out var enemy) ? enemy : null;

        if (enemyComponent != null)
        {
            enemy.TakeDamage(attackDamage = 1); // = 1 debug value
            Vector3 randomizedOffset = parent.MousePlayerOffset + new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
            MeleeSystem.KnockbackRoutine(enemy.gameObject, parent.MousePlayerOffset + randomizedOffset, recoilForce);
        }
        else { Debug.LogError("No Enemy script found!"); }
    }
}