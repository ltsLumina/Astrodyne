using System;
using UnityEngine;

public class WeaponAnimatorManager : MonoBehaviour
{
    #region Cached References
    [Header("Cached References")]
    Weapon weapon;
    MeleeComboSystem meleeSys;
    Animator weaponAnimator;
    SpriteRenderer slashSprite;
    #endregion

    [Header("Cached Hashes")]
    static readonly int InCombat = Animator.StringToHash("inCombat");

    public static Animator WeaponAnim { get; private set; }

    void Start()
    {
        WeaponAnim = GetComponent<Animator>();

        // Cached References to other components.
        Transform scytheGameObject = transform.parent;
        weapon         = scytheGameObject.GetComponent<Weapon>();
        meleeSys       = scytheGameObject.GetComponent<MeleeComboSystem>();
        weaponAnimator = scytheGameObject.GetComponentInChildren<Animator>();
        var slashEffect = scytheGameObject.GetComponentInChildren<SlashEffect>();
        slashSprite = slashEffect.GetComponent<SpriteRenderer>();

        // Subscribe to events.
        meleeSys.onMeleeAttack += RandomizeSlashDirection;
    }

    void Update()
    {
        #region Timers
        // Decrement the combat timer.
        weapon.IsInCombat();
        #endregion

        // Adjust the combat bool in the animator, depending on whether the player is in combat or not.
        PlayerAnimationManager.PlayerAnim.SetBool(InCombat, weapon.IsInCombat());
    }

    void RandomizeSlashDirection()
    {
        int rand = UnityEngine.Random.Range(0, 2);
        slashSprite.flipY = rand == 1;

        weaponAnimator.SetTrigger(rand == 1 ? "SlashUp" : "SlashDown");
    }
}