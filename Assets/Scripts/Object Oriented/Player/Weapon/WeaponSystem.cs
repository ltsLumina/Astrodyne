﻿#region
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Essentials;
using UnityEngine;
#endregion

/// <summary>
/// Base class for all weapon systems.
/// </summary>
public abstract class WeaponSystem : MonoBehaviour
{
    [Header("Common Parameters"), SerializeField, Tooltip("The prefab to be used in instantiating the attack. I.e the bullet, slash, etc.")]
    protected GameObject weaponPrefab;

    [Space(5)]

    [SerializeField, Tooltip("How long the player must wait before attacking again.")] //TODO: set attack rate value to bullets per minute
    protected float attackRate;

    [SerializeField, Tooltip("How long the attack will last before being returned to the pool.")]
    protected float attackLifetime;

    [Space(5)]

    [Header("Read-only Fields"), SerializeField, ReadOnly]
    float timeSinceLastAttack;

    // Cached references.
    protected Camera cam;
    public ObjectPool AttackPool { get; set; }

    // Returns the direction of the mouse relative to the player.
    protected Vector3 MousePlayerOffset => (Input.mousePosition - cam.WorldToScreenPoint(transform.position)).normalized;

    protected float TimeSinceLastAttack
    {
        get => timeSinceLastAttack;
        private set => timeSinceLastAttack = value;
    }

    // Delegate for when the player attacks.
    public delegate void OnAttack();
    public event OnAttack onAttack;

    protected virtual void Start()
    {
        // Cache references.
        cam = Camera.main;

        // Initialize the object pool.
        AttackPool = ObjectPoolManager.CreateNewPool(weaponPrefab, 10);
        AttackPool.gameObject.name = $"{weaponPrefab.name} Pool";

        // Subscribe to the onAttack event.
        onAttack += Attack;

        // Asserting that the attackLifetime is greater than 0 to prevent the attack from never being returned to the pool.
        Debug.Assert(attackLifetime >= 0, "Attack lifetime is less than or equal to 0. " +
                                         "This will cause the attack to never be returned to the pool.");
    }

    protected virtual void Update()
    {
        // Increment the time since the last attack, and while it is larger than the attackRate, the player can attack.
        TimeSinceLastAttack += Time.deltaTime;

        if (CanAttack())
            // Raise the onAttack event.
            onAttack?.Invoke();
    }

    protected abstract bool CanAttack();

    public virtual void Attack()
    {
        // Reset the attack timer.
        TimeSinceLastAttack = 0;

        // Enter combat.
        CombatManager.Instance.EnterCombat();
    }

    protected virtual void PostAttack(GameObject pooledAttack)
    {
        // Return the attack to the pool after attackLifetime seconds.
        Task delayTask = UsefulMethods.DelayedTaskAsync(() => pooledAttack.SetActive(false), attackLifetime).AsTask();
        delayTask.ContinueWith(_ => Debug.Log($"{weaponPrefab.name} returned to pool!"));

        // Reset the attack timer.
        TimeSinceLastAttack = 0;
    }
}