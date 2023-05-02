#region
using UnityEngine;
#endregion

public class WeaponAnimatorManager : MonoBehaviour
{
    bool slashUp;

    #region Cached References
    MeleeSystem meleeSys;

    [Header("Cached Hashes")]
    readonly static int InCombat = Animator.StringToHash("inCombat");
    #endregion

    public static Animator WeaponAnim { get; private set; }

    void Start()
    {
        WeaponAnim = GetComponent<Animator>();

        // Cached References to other components.
        Transform scytheGameObject = transform.parent;
        meleeSys = scytheGameObject.GetComponent<MeleeSystem>();

        // Subscribe to events.
        meleeSys.onAttack += SlashDirection;
    }

    void Update()
    {
        #region Timers
        // Decrement the combat timer.
        CombatManager.Instance.IsInCombat();
        #endregion

        // Adjust the "inCombat" bool parameter in the animator, depending on whether the player is in combat or not.
        PlayerAnimationManager.PlayerAnim.SetBool(InCombat, CombatManager.Instance.IsInCombat());
    }

    void SlashDirection()
    {
        slashUp = !slashUp;

        meleeSys.ActiveSlash.GetComponent<SpriteRenderer>().flipY = slashUp;

        //WeaponAnim.SetTrigger(slashUp ? "slashUp" : "slashDown");
        // unused, but I'm keeping it here for reference.
    }
}