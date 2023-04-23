using Essentials;
using UnityEngine;

public class MeleeComboSystem : MonoBehaviour
{
    #region Configurable Parameters
    [SerializeField] int attackDamage = 10;
    [SerializeField] float attackTime = 0.45f;
    [SerializeField] float attackForce;
    [SerializeField, Tooltip("Adjust attack time and attack speed together for the smoothest animation.")]
    float attackSpeed;

    [Header("Read-only Fields"), SerializeField, ReadOnly]
    float timeSinceLastMelee;
    #endregion

    #region Cached References
    Camera cam;
    Weapon weapon;
    Animator slashAnimator;

    [Header("Cached Hashes")]
    static readonly int Attack = Animator.StringToHash("attack");
    #endregion

    // Delegate for when the player melee attacks.
    public delegate void OnMeleeAttack();
    public event OnMeleeAttack onMeleeAttack;

    #region Properties
    // Returns the direction of the mouse relative to the player.
    public Vector3 MousePlayerOffset =>
        (Input.mousePosition - cam.WorldToScreenPoint(transform.position)).normalized;
    #endregion

    void Start()
    {
        cam           = Camera.main;
        weapon        = GetComponent<Weapon>();
        var slashEffect = GetComponentInChildren<SlashEffect>();
        slashAnimator = slashEffect.GetComponent<Animator>();

        // Subscribe to the onMeleeAttack event and set the animation speed if the event is invoked.
        onMeleeAttack += () =>
        {
            weapon.EnterCombat();
            slashAnimator.speed = attackSpeed;
        };
    }

    public void MeleeCombat()
    {
        timeSinceLastMelee += Time.deltaTime;

        //TODO: MAKE COMBO SCRIPT
        //USE THIS: https://www.youtube.com/watch?v=Jm0mbHEFPfE
        if (Input.GetMouseButton(1) && timeSinceLastMelee > attackTime)
        {
            onMeleeAttack?.Invoke();

            // Attack routine. Start the Animation and dash a short distance towards the mouse.
            // This makes the swing more impactful and feel smoother.
            slashAnimator.SetTrigger(Attack);
            KnockbackRoutine(gameObject, MousePlayerOffset, attackForce);

            timeSinceLastMelee = 0;
        }
    }

    /// <summary>
    /// Knockback any gameobject with a rigidbody2D.
    /// </summary>
    /// <remarks> Keep in mind that the direction parameter already is normalized. </remarks>
    public static void KnockbackRoutine(GameObject gameObject, Vector2 direction, float force = 10f)
    {
        var rigidbody = gameObject.GetComponent<Rigidbody2D>();
        if (rigidbody != null)
            rigidbody.AddForce(direction.normalized * force, ForceMode2D.Impulse);
    }
}