#region
using System;
using Essentials;
using UnityEngine;
using static GameManager;
#endregion

/// <summary>
/// The system that handles all melee combat. Currently only supports the slash.
/// </summary>
public class MeleeSystem : WeaponSystem
{
    [Header("Slashing Parameters"), SerializeField]
    SlashParameters slashParameters;

    // Cached Hashes
    readonly static int AttackID = Animator.StringToHash("attack");

    // Properties
    // The currently active slash effect. Used to flip the slash effect depending on the direction of the mouse.
    public GameObject ActiveSlash { get; private set; }

    protected override bool CanAttack() =>
        (Input.GetMouseButton(1) || Input.GetKey(KeyCode.V)) && TimeSinceLastAttack > WeaponData.attackDelay && AttackPool != null;

    public override void Attack()
    {
        base.Attack();

        // Dash a short distance towards the mouse to make the attack feel more impactful.
        KnockbackRoutine(gameObject.transform.parent.gameObject, MousePlayerOffset, WeaponData.stepDistance);

        // Initialize the bullet before method call to to avoid closure allocation.
        GameObject pooledSlash = AttackPool.GetPooledObject();

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
        pooledSlash = AttackPool.GetPooledObject(true);
        ActiveSlash = pooledSlash;

        pooledSlash.transform.position = Scythe.HitPoint.position;
        pooledSlash.transform.rotation = transform.rotation;

        var pooledSlashAnim = pooledSlash.GetComponent<Animator>();

        pooledSlashAnim.SetTrigger(AttackID);
        pooledSlashAnim.speed = WeaponData.animationSpeedScalar;

        float slashSize = dash.IsDashAttacking ? slashParameters.dashAttackSlashSize : slashParameters.slashSize;
        pooledSlash.transform.localScale = new Vector2(slashSize, slashSize);

        //Rotate the slash effect depending on the direction of the mouse.
        float angle = Mathf.Atan2(MousePlayerOffset.y, MousePlayerOffset.x) * Mathf.Rad2Deg;
        pooledSlash.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Set the parent of the slash effect to the player.
        pooledSlash.transform.parent = FindObjectOfType<Player>().transform;

        PostAttack(pooledSlash);
    }
}