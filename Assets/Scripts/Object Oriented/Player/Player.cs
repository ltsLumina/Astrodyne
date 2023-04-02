#region
using Cysharp.Threading.Tasks;
using UnityEngine;
using static Essentials;
using static UnityEngine.Debug;
using static Essentials.UsefulMethods;
#endregion

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(CapsuleCollider2D))]
public class Player : MonoBehaviour
{
    #region Serialized Fields
    [Header("Cached References")]
    new CapsuleCollider2D collider;
    Camera cam;
    SpriteRenderer sprite;
    Transition transition;
    SpriteRenderer spriteRenderer;
    PlayerAnimator playerAnimator;

    // Turn moveInput into property eventually.

    [Header("Movement"), Space(5)]
    [SerializeField] float moveSpeed = 5f;

    [Header("Health"), Space(5)]
    [SerializeField] int maxHealth = 100;
    [SerializeField] int currentHealth = 100;
    int previousHealth;

    [Header("Read-only Fields")]
    [ReadOnly] public Vector2 moveInput;
    [SerializeField, ReadOnly] bool isDead;

    #endregion

    #region Properties
    public Rigidbody2D RB { get; private set; }
    public bool IsFacingRight { get; private set; } = true;
    public int CurrentHealth
    {
        get => currentHealth;
        set
        {
            previousHealth = currentHealth;
            currentHealth  = value;
            currentHealth  = Mathf.Clamp(currentHealth, 0, maxHealth);

            IsDead = currentHealth <= 0;
            if (IsDead) HandleDeath();

            if (currentHealth < previousHealth) //TODO: this method runs even if the player is healed.
                StartCoroutine(playerAnimator.DamageRoutine());
        }
    }

    public bool IsDead
    {
        get => isDead;
        private set => isDead = value;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Randomize Spawn Position
        transform.position = new (Random.Range(-30f, 7f), Random.Range(-3f, 15f), 0);

        cam            = Camera.main;
        RB             = GetComponent<Rigidbody2D>();
        collider       = GetComponent<CapsuleCollider2D>();
        sprite         = GetComponentInChildren<SpriteRenderer>();
        playerAnimator = GetComponentInChildren<PlayerAnimator>();
        transition     = FindObjectOfType<Transition>();
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
        Movement();
        FaceMouse();

        void Boundaries()
        { // Keeps the player within the camera's view.
            Vector3 viewPos = cam.WorldToViewportPoint(transform.position);
            viewPos.x          = Mathf.Clamp01(viewPos.x);
            viewPos.y          = Mathf.Clamp01(viewPos.y);
            transform.position = cam.ViewportToWorldPoint(viewPos);
        }

        void Movement()
        { // Movement: WASD or Arrow Keys
            // Moves the player by getting their input and multiplying it by the move speed.
            // Gives the movement an acceleration/deceleration feel.
            moveInput.x = Input.GetAxis("Horizontal");
            moveInput.y = Input.GetAxis("Vertical");
            RB.velocity = new Vector2(moveInput.x, moveInput.y) * moveSpeed;
        }

        void FaceMouse()
        { // Smoothly rotates player to face mouse.
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

            // Flip the sprite if the mouse is on the left side of the player
            if (mousePos.x > transform.position.x)
            {
                sprite.flipX  = true;
                IsFacingRight = false;
                Assert(!IsFacingRight, "Facing Left!");
            }
            else
            {
                sprite.flipX  = false;
                IsFacingRight = true;
                Assert(IsFacingRight, "Facing Right!");
            }
        }
    }

    void HandleDeath()
    {
        // Debug that the player is dead and freeze their movement.
        Log("Player is dead.");
        RB.FreezeAllConstraints();

        // Close the curtains and wait 2 seconds before loading the game over scene.
        var delayTask = DelayedTaskAsync(() => transition.CloseCurtains(), 2).AsTask();
        delayTask.ContinueWith(_ => Log("//TODO: Load Game Over Scene"));
    }
}