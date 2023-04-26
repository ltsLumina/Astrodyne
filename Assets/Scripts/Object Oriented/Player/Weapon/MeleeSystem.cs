#region
using System;
using Essentials;
using UnityEngine;
#endregion

public class MeleeSystem : MonoBehaviour
{
    [Header("Slash Attack Info"), SerializeField]
    SlashStruct slashEffectInfo;

    #region Cached References
    Camera cam;
    GameObject slash;
    Animator slashAnimator;

    public GameObject InstantiatedSlash { get; private set; }
    GameObject instantiatedSlash;

    [Header("Cached Hashes")]
    readonly static int Attack = Animator.StringToHash("attack");
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

        // Subscribe to the onMeleeAttack event and set the animation speed if the event is invoked.
        onMeleeAttack += InstantiateSlash;
    }

    void Update()
    { // Set the slash size to the slashSize parameter.
        transform.localScale = new (slashEffectInfo.slashSize, slashEffectInfo.slashSize, 1f);

        // Increment the time since the last melee attack.
        // While the time since last melee is larger than the attackDelay, the player can melee attack.
        MeleeCombat();
    }

    public void MeleeCombat()
    {
        slashEffectInfo.timeSinceLastMelee += Time.deltaTime;

        if (!Input.GetMouseButton(1) || !(slashEffectInfo.timeSinceLastMelee > slashEffectInfo.attackDelay)) return;

        // Invoke the onMeleeAttack event and reset the timer.
        onMeleeAttack?.Invoke();
        slashEffectInfo.timeSinceLastMelee = 0;
    }

    void InstantiateSlash()
    {
        CombatManager.Instance.EnterCombat();

        // Experimental slash effect that instantiates a slash prefab.
        // Instantiates the slash, plays the animation, and destroys the slash after a short delay.
        InstantiatedSlash =
            Instantiate(slashEffectInfo.slashPrefab, slashEffectInfo.hitPoint.position, Quaternion.identity);

        InstantiatedSlash.GetComponent<Animator>().SetTrigger(Attack);
        InstantiatedSlash.GetComponent<Animator>().speed = slashEffectInfo.attackSpeed;

        //Rotate the slash effect depending on the direction of the mouse.
        float angle = Mathf.Atan2(MousePlayerOffset.y, MousePlayerOffset.x) * Mathf.Rad2Deg;
        InstantiatedSlash.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);


        // Set the parent of the slash effect to the player.
        InstantiatedSlash.transform.parent = FindObjectOfType<Player>().transform;

        //Destroy the slash effect after a short delay.
        // Keep in mind that this is temporary, and will be replaced by a object pool.
        Destroy(InstantiatedSlash, 0.45f);

        // Dash a short distance towards the mouse to make the attack feel more impactful.
        KnockbackRoutine(gameObject.transform.parent.gameObject, MousePlayerOffset, slashEffectInfo.stepForce);
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

[Serializable]
public struct SlashStruct
{ // A struct with data for the slash effect.

    public GameObject slashPrefab;
    public Transform hitPoint;

    public int AttackDamage { get; set; }
    public float RecoilForce { get; set; }

    // These values are only used in the MeleeSystem.
    [Tooltip("The delay between the attack and the slash effect.")]
    public float attackDelay;

    [Tooltip("The speed of the slash effect animation. Scales with the attack delay.")]
    public float attackSpeed;

    [Tooltip("The size of the slash effect.")]
    public float slashSize;

    [Header("Knockback"), Tooltip("The distance the player dashes when attacking.")]
    public float stepForce;

    [Header("Read-only Fields"), SerializeField, ReadOnly]
    public float timeSinceLastMelee;

}