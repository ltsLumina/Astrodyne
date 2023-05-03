using UnityEngine;

public interface IDamageable
{
    public int Health { get; set; }

    public void Damage(int damage)
    {
        Health -= damage;
    }
}