using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Fires bullets from the player's weapon to the mouse position.
/// TODO: ADD THE ULTRAKILL MELEE + SHOOT ATTACK. (alternatively; you can dash into bullets to make them go faster)
/// </summary>
public class ShootingSystem : WeaponSystem
{
    [Header("Shooting Parameters")]
    [SerializeField] int bulletDamage;
    [SerializeField] float bulletForce;
    [SerializeField] float bloom;

    // Properties
    public float BulletForce
    {
        get => bulletForce;
        set => bulletForce = value;
    }

    // The currently active slash effect. Used to flip the slash effect depending on the direction of the mouse.
    public GameObject ActiveBullet { get; private set; }

    protected override bool CanAttack() =>
        Input.GetMouseButton(0) && TimeSinceLastAttack > attackRate && AttackPool != null;

    public override void Attack()
    {
        base.Attack();

        // Initialize the bullet before method call to to avoid closure allocation.
        GameObject pooledBullet = AttackPool.GetPooledObject();

        // If the pooled bullet is null, return.
        try
        {
            if (pooledBullet == null) return;
        }
        catch (Exception e)
        {
            Debug.Log($"{weaponPrefab.name} is null! \n" + e);
            throw;
        }

        // Activate the bullet, and set its position and rotation.
        //pooledBullet.GetComponent<TrailRenderer>().Clear(); //TODO: don't like this but it works. Tried to make a work around but it proved to be too much work.
        pooledBullet = AttackPool.GetPooledObject(true);
        ActiveBullet = pooledBullet;

        // Set the position to the shooting position plus an offset relative to the mouse position.
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        // Set the bullet's position and rotation.
        pooledBullet.transform.position = Scythe.HitPoint.transform.position + (mousePos - transform.position).normalized * 0.5f;
        //pooledBullet.transform.rotation = transform.rotation;
        //TODO: add random bloom to the bullets. Then continue with the SlashEffect ultrakill thing.

        // Add force to the bullet.
        var bulletRB = pooledBullet.GetComponent<Rigidbody2D>();
        bulletRB.AddForce(transform.up * BulletForce, ForceMode2D.Impulse);

        // Return the bullet to the objectPool and reset the attack timer.
        PostAttack(pooledBullet);
    }

    // TODO: bro idk what im doing im writing garbage code rn
    protected override void PostAttack(GameObject pooledAttack)
    {
        var pooledAttackTrail = pooledAttack.GetComponent<TrailRenderer>();

        pooledAttackTrail.Clear();
        pooledAttackTrail.startColor = new Color(0.36f, 0.49f, 0.69f);
        pooledAttack.transform.localScale = new Vector2(0.75f, 0.75f);

        base.PostAttack(pooledAttack);
    }
}