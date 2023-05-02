#region
using System;
using UnityEngine;
using static GameManager;
#endregion

/// <summary>
/// //TODO: I want to move all of the slash logic to another class, or a struct perhaps.
/// //TODO: I.e, I want this class to only handle the weapon and attacking logic, not the slash parameters or settings.
/// //TODO: Only issue with using structs is that I necessarily use the serialized value, because the value can't be changed in runtime.
/// </summary>
public class MeleeSystem : WeaponSystem
{
    [Header("Slashing Parameters"), Space(10)]
    [SerializeField] int slashDamage;
    [SerializeField] float knockbackForce;
    [SerializeField] float slashAnimSpeed;
    [SerializeField] float slashSize;

    [Header("Knockback"), Tooltip("The distance the player dashes when attacking.")]
    public float stepForce;

    public int SlashDamage
    {
        get => slashDamage;
        set => slashDamage = value;
    }
    public float KnockbackForce
    {
        get => knockbackForce;
        set => knockbackForce = value;
    }

    // Cached Hashes
    readonly static int AttackID = Animator.StringToHash("attack");

    // Properties
    // The currently active slash effect. Used to flip the slash effect depending on the direction of the mouse.
    public GameObject ActiveSlash { get; private set; }

    protected override bool CanAttack() =>
        (Input.GetMouseButton(1) || Input.GetKeyDown(KeyCode.V)) && TimeSinceLastAttack > attackRate && AttackPool != null;

    public override void Attack()
    {
        base.Attack();

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
        pooledSlash.transform.rotation = transform.rotation; //TODO: This is probably wrong.

        pooledSlash.GetComponent<Animator>().SetTrigger(AttackID);
        pooledSlash.GetComponent<Animator>().speed = slashAnimSpeed;
        pooledSlash.transform.localScale = new Vector2(slashSize, slashSize);

        //Rotate the slash effect depending on the direction of the mouse.
        float angle = Mathf.Atan2(MousePlayerOffset.y, MousePlayerOffset.x) * Mathf.Rad2Deg;
        pooledSlash.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Set the parent of the slash effect to the player.
        pooledSlash.transform.parent = FindObjectOfType<Player>().transform;

        PostAttack(pooledSlash);

        // Dash a short distance towards the mouse to make the attack feel more impactful.
        KnockbackRoutine(gameObject.transform.parent.gameObject, MousePlayerOffset, stepForce);
    }
}