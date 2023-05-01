using System;
using UnityEngine;

/// <summary>
/// Fires bullets from the player's weapon to the mouse position.
/// TODO: ADD THE ULTRAKILL MELEE + SHOOT ATTACK. (alternatively; you can dash into bullets to make them go faster)
/// </summary>
public class ShootingSystem : WeaponSystem
{
    [Header("Shooting Parameters"), SerializeField, Space(10)]
    BulletParameters bulletInfo;

    protected override bool CanAttack() =>
        Input.GetMouseButton(0) && timeSinceLastAttack > attackRate && attackPool != null;

    protected override void Attack()
    {
        base.Attack();

        // Initialize the bullet before method call to to avoid closure allocation.
        GameObject pooledBullet = attackPool.GetPooledObject();

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
        pooledBullet.GetComponent<TrailRenderer>().Clear(); //TODO: don't like this but it works. Tried to make a work around but it proved to be too much work.
        pooledBullet.SetActive(true);

        // Set the position to the shooting position plus an offset relative to the mouse position.
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        // Set the bullet's position and rotation.
        pooledBullet.transform.position = Scythe.HitPoint.transform.position + (mousePos - transform.position).normalized * 0.5f;
        pooledBullet.transform.rotation = transform.rotation;

        // Add force to the bullet.
        var bulletRB = pooledBullet.GetComponent<Rigidbody2D>();
        bulletRB.AddForce(transform.up * bulletInfo.bulletForce, ForceMode2D.Impulse);

        // Return the bullet to the objectPool and reset the attack timer.
        PostAttack(pooledBullet);
    }
}

[Serializable]
public struct BulletParameters
{
    [Tooltip("The amount of damage the bullet does.")]
    public int bulletDamage;

    [Tooltip("The force applied to the bullet when it is fired. Alternatively known as the bullet's speed.")]
    public float bulletForce;
}