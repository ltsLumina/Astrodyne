#region
using System;
using UnityEngine;
using static GameManager;
#endregion

public class MeleeSystem : WeaponSystem
{
    [Header("Slashing Parameters"), SerializeField, Space(10)]
    SlashParameters slashInfo;

    // Cached Hashes
    readonly static int AttackID = Animator.StringToHash("attack");

    protected override bool CanAttack() =>
        Input.GetMouseButton(1) && timeSinceLastAttack > attackRate && attackPool != null;

    protected override void Attack()
    {
        base.Attack();

        // Initialize the bullet before method call to to avoid closure allocation.
        GameObject pooledSlash = attackPool.GetPooledObject();

        // If the pooled bullet is null, return.
        try
        {
            if (pooledSlash == null) return;
        } catch (Exception e)
        {
            Debug.Log("Pooled slash is null! \n" + e);
            throw;
        }

        // Activate the slash, and set its position and rotation.
        pooledSlash = attackPool.GetPooledObject(true);

        pooledSlash.transform.position = Scythe.HitPoint.position;
        pooledSlash.transform.rotation = transform.rotation; //TODO: This is probably wrong.

        pooledSlash.GetComponent<Animator>().SetTrigger(AttackID);
        pooledSlash.GetComponent<Animator>().speed = slashInfo.slashAnimSpeed;
        pooledSlash.transform.localScale = new Vector2(slashInfo.slashSize, slashInfo.slashSize);

        // //Rotate the slash effect depending on the direction of the mouse.
        float angle = Mathf.Atan2(MousePlayerOffset.y, MousePlayerOffset.x) * Mathf.Rad2Deg;
        pooledSlash.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Set the parent of the slash effect to the player.
        pooledSlash.transform.parent = FindObjectOfType<Player>().transform;

        PostAttack(pooledSlash);

        // Dash a short distance towards the mouse to make the attack feel more impactful.
        KnockbackRoutine(gameObject.transform.parent.gameObject, MousePlayerOffset, slashInfo.stepForce);
    }
}

[Serializable]
public struct SlashParameters
{ // A struct with data for the slash effect.

    public int SlashDamage
    {
        readonly get => slashDamage;
        set => slashDamage = value;
    }
    public float KnockbackForce
    {
        readonly get => knockbackForce;
        set => knockbackForce = value;
    }

    [Tooltip("The damage the attack does.")]
    public int slashDamage;

    [Tooltip("The force of the recoil when attacking.")]
    public float knockbackForce;

    [Tooltip("The speed of the slash effect animation. Scales with the attack delay.")]
    public float slashAnimSpeed;

    [Tooltip("The size of the slash effect.")]
    public float slashSize;

    [Header("Knockback"), Tooltip("The distance the player dashes when attacking.")]
    public float stepForce;
}