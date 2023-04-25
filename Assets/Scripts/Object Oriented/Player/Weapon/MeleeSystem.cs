#region
using Essentials;
using Unity.Mathematics;
using UnityEngine;
#endregion

public class MeleeSystem : MonoBehaviour
{
    #region Configurable Parameters
    [Header("Melee Parameters"), Tooltip("Parameters that govern the melee attack of the Scythe.")]
    [SerializeField] int attackDamage = 10;
    [SerializeField] float attackForce;

    [Space(5), SerializeField, Tooltip("Adjust attack time and attack speed together for the smoothest animation.")]
    float attackDelay, attackSpeed;

    [Header("Read-only Fields"), SerializeField, ReadOnly]
    float timeSinceLastMelee;
    [SerializeField] GameObject slashPrefab;
    [SerializeField] GameObject hitPoint;
    #endregion

    #region Cached References
    Camera cam;
    GameObject slash;
    Animator slashAnimator;

    public GameObject InstantiatedSlash { get; private set; }
    GameObject instantiatedSlash;

    [Header("Cached Hashes")]
    static readonly int Attack = Animator.StringToHash("attack");
    #endregion

    // Delegate for when the player melee attacks.
    public delegate void OnMeleeAttack();
    public event OnMeleeAttack onMeleeAttack;

    #region Properties
    // Returns the direction of the mouse relative to the player.
    public Vector3 MousePlayerOffset => (Input.mousePosition - cam.WorldToScreenPoint(transform.position)).normalized;
    #endregion

    void Start()
    {
        cam = Camera.main;
        //var slashEffect = GetComponentInChildren<SlashEffect>();
        //slashEffect.GetComponent<Animator>();
        //slashAnimator = slashEffect.GetComponent<Animator>();
        //slash = slashEffect.gameObject;

        // Subscribe to the onMeleeAttack event and set the animation speed if the event is invoked.
        onMeleeAttack += () =>
        { // Enter combat, play the animation, and set the animation speed.
            CombatManager.Instance.EnterCombat();

            // Experimental slash effect that instantiates a slash prefab.
            // Instantiates the slash, plays the animation, and destroys the slash after a short delay.
            InstantiatedSlash = Instantiate(slashPrefab, hitPoint.transform.position, Quaternion.identity);
            InstantiatedSlash.GetComponent<Animator>().SetTrigger(Attack);
            InstantiatedSlash.GetComponent<Animator>().speed = attackSpeed;

            //Rotate the slash effect depending on the direction of the mouse.
            float angle = Mathf.Atan2(MousePlayerOffset.y, MousePlayerOffset.x) * Mathf.Rad2Deg;
            InstantiatedSlash.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            InstantiatedSlash.transform.parent   = FindObjectOfType<Player>().transform;




            //Destroy the slash effect after a short delay.
            Destroy(InstantiatedSlash, 0.45f);

            // Old (current) slash effect.
            // slashAnimator.SetTrigger(Attack);
            // slashAnimator.speed = attackSpeed;

            // Dash a short distance towards the mouse to make the attack feel more impactful.
            //KnockbackRoutine(gameObject.transform.parent.gameObject, MousePlayerOffset, attackForce);
        };
    }

    void Update() => MeleeCombat();

    public void MeleeCombat()
    {
        timeSinceLastMelee += Time.deltaTime;

        if (!Input.GetMouseButton(1) || !(timeSinceLastMelee > attackDelay)) return;

        // Invoke the onMeleeAttack event and reset the timer.
        onMeleeAttack?.Invoke();
        timeSinceLastMelee = 0;
    }

    /// <summary>
    ///     Knockback any gameobject with a rigidbody2D.
    /// </summary>
    /// <remarks> Keep in mind that the direction parameter already is normalized. </remarks>
    public static void KnockbackRoutine(GameObject gameObject, Vector2 direction, float force = 10f)
    {
        var rigidbody = gameObject.GetComponent<Rigidbody2D>();
        if (rigidbody != null)
            rigidbody.AddForce(direction.normalized * force, ForceMode2D.Impulse);
    }
}