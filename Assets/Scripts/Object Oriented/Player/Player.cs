#region
using Cysharp.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Debug;
using static Essentials.UsefulMethods;
#endregion

[RequireComponent(typeof(Rigidbody2D))] [RequireComponent(typeof(CapsuleCollider2D))]
public class Player : MonoBehaviour
{
    #region Serialized Fields
    [Header("Cached References")]
    // ReSharper disable once InconsistentNaming
    Rigidbody2D RB;
    new CapsuleCollider2D collider;
    Camera cam;
    SpriteRenderer sr;
    Transition transition;

    SpriteRenderer spriteRenderer;

    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;

    [SerializeField] float dashForce = 5f;
    [SerializeField] float dashDuration = 0.5f;
    [SerializeField] float dashCooldown = 1f;

    [Header("Attacking")]
    [SerializeField] float attackRange = 1f;
    [SerializeField] float attackDamage = 1f;
    [SerializeField] float attackCooldown = 1f;

    [Header("Abilities")]
    [SerializeField] float abilityRange = 1f;
    [SerializeField] float abilityDamage = 1f;
    [SerializeField] float abilityCooldown = 1f;
    [SerializeField] float drawCooldown;

    [Header("Health")]
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth = 100f;
    #endregion

    #region Properties
    [field: Header("Configurable Variables")]
    public bool IsFacingRight { get; private set; } = true;

    public float CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = value;
            IsDead = currentHealth <= 0;
        }
    }

    public bool IsDead { get; private set; }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Randomize Spawn Position
        transform.position = new (Random.Range(-30f, 7f), Random.Range(-3f, 15f), 0);

        cam        = Camera.main;
        RB         = GetComponent<Rigidbody2D>();
        collider   = GetComponent<CapsuleCollider2D>();
        sr         = GetComponentInChildren<SpriteRenderer>();
        transition = FindObjectOfType<Transition>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsDead) PlayerLogic();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    ///     Player logic is handled entirely in this method where each part of the logic is split into its own local function.
    /// </summary>
    void PlayerLogic()
    {
        Boundaries();

        void Boundaries()
        {
            // Keeps the player within the camera's view.
            Vector3 viewPos = cam.WorldToViewportPoint(transform.position);
            viewPos.x          = Mathf.Clamp01(viewPos.x);
            viewPos.y          = Mathf.Clamp01(viewPos.y);
            transform.position = cam.ViewportToWorldPoint(viewPos);
        }

        Movement();

        void Movement()
        { // Movement: WASD or Arrow Keys
            // Moves the player by getting their input and multiplying it by the move speed.
            // Gives the movement an acceleration/deceleration feel.
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");
            RB.velocity = new Vector2(x, y) * moveSpeed;
        }

        FaceMouse();

        void FaceMouse()
        {
            // Smoothly rotates player to face mouse.
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

            // flip the sprite if the mouse is on the left side of the player
            if (mousePos.x > transform.position.x)
            {
                sr.flipX      = true;
                IsFacingRight = false;
                Assert(!IsFacingRight, "Facing Left");
            }
            else
            {
                sr.flipX      = false;
                IsFacingRight = true;
                Assert(IsFacingRight, "Facing Right");
            }
        }

        HealthLogic();

        void HealthLogic()
        {
            // If the player is dead, run the HandleDeath method.
            if (IsDead) HandleDeath();
        }

        void HandleDeath()
         {
             // Debug that the player is dead and freeze their movement.
             Log("Player is dead.");
             RB.FreezeAllConstraints();

             // Close the curtains and wait 2 seconds before loading the game over scene.
             var delayTask = DelayedTaskAsync(() => transition.CloseCurtains(), 2, true).AsTask();
             delayTask.ContinueWith(_ => Log("Closing Curtains..."));
         }
    }
}