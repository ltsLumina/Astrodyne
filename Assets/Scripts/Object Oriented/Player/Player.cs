#region
using Cysharp.Threading.Tasks;
using UnityEngine;
using static Essentials.Attributes;
using static Essentials.Sequencing;
using static Essentials.Deprecated;
using static UnityEngine.Debug;
#endregion

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(CapsuleCollider2D))]
public class Player : MonoBehaviour, IDamageable
{
    #region Cached References
    CapsuleCollider2D hitbox;
    Camera cam;
    SpriteRenderer sprite;
    Transition transition;
    #endregion

    #region Configurable Parameters
    [Header("Movement"), Space(5)]
    [SerializeField] float moveSpeed = 5f;

    [Header("Health"), Space(5), SerializeField]
    int health = 100;
    const int MAX_HEALTH = 100;
    int previousHealth;

    [Header("Read-only Fields")]
    [SerializeField, ReadOnly] Vector2 moveInput;
    [SerializeField, ReadOnly] Vector2 lastMoveInput;
    [SerializeField, ReadOnly] bool isDead;
    [SerializeField, ReadOnly] bool isFacingRight;
    #endregion

    public delegate void OnMoveInputChanged(Vector2 moveInput);
    public event OnMoveInputChanged onMoveInputChanged;

    public delegate void OnPlayerTakeDamage();
    public event OnPlayerTakeDamage onPlayerTakeDamage;

    #region Properties
    public Rigidbody2D RB { get; private set; }

    public bool IsFacingRight
    {
        get => isFacingRight;
        set => isFacingRight = value;
    }

    public int Health
    {
        get => health;
        set
        {
            previousHealth = health;
            health  = value;
            health  = Mathf.Clamp(health, 0, MAX_HEALTH);

            if (health < previousHealth)
                onPlayerTakeDamage?.Invoke();

            IsDead = health <= 0;
            if (IsDead) HandleDeath();
        }
    }
    public bool IsDead
    {
        get => isDead;
        private set => isDead = value;
    }
    public Vector2 MoveInput
    {
        get => moveInput;
        private set
        {
            lastMoveInput = moveInput;
            moveInput     = value;

            // Notify the player animator of the change in move input.
            if (moveInput != lastMoveInput)
            {
                lastMoveInput = moveInput;
                onMoveInputChanged?.Invoke(moveInput);
            }
        }
    }

    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }
    #endregion

    void Start()
    {
        // Randomize Spawn Position
        transform.position = new (Random.Range(-30f, 7f), Random.Range(-3f, 15f), 0);

        cam        = Camera.main;
        RB         = GetComponent<Rigidbody2D>();
        hitbox     = GetComponent<CapsuleCollider2D>();
        sprite     = GetComponentInChildren<SpriteRenderer>();
        transition = FindObjectOfType<Transition>();

        // Delegate for when the player takes damage.
        onPlayerTakeDamage += () =>
        {
            PerformOnTakeDamage();
            CombatManager.Instance.EnterCombat();
        };
    }

    void Update()
    {
        if (IsDead) return; PlayerLogic();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    ///     Player logic is handled entirely in this method where each part of the logic is split into its own local function.
    /// </summary>
    void PlayerLogic()
    {
        Boundaries();
        Movement();

        void Boundaries()
        { // Keeps the player within the camera's view.
            Vector3 viewPos    = cam.WorldToViewportPoint(transform.position);
            viewPos.x          = Mathf.Clamp01(viewPos.x);
            viewPos.y          = Mathf.Clamp01(viewPos.y);
            transform.position = cam.ViewportToWorldPoint(viewPos);
        }

        void Movement()
        { // Movement: WASD or Arrow Keys
            // Moves the player by getting their input and multiplying it by the move speed.
            // Gives the movement an acceleration/deceleration feel.
            MoveInput = new(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            // Delegate to PlayerAnimator
            if (MoveInput != lastMoveInput)
            {
                lastMoveInput = MoveInput;
                onMoveInputChanged?.Invoke(MoveInput);
            }

            //RB.velocity = new Vector2(moveInput.x, moveInput.y) * moveSpeed;
            Vector2 force = new Vector2(MoveInput.x, MoveInput.y).normalized * MoveSpeed;

            // adjust RB.drag based on moveInput
            RB.AddForce(force, ForceMode2D.Force);
        }
    }

    // Void method necessary to subscribe to the delegate in the CurrentHealth property.
    public void PerformOnTakeDamage() => OnTakeDamage();

    void OnTakeDamage()
    {
        // If the player is dead, stop the coroutine.
        if (Health == 0) return; // yield break;

        // Camera shake to indicate damage has been taken.
        CameraShake.Instance.ShakeCamera(1.5f, 0.2f);

        // Disable the hitbox for 1 second to prevent the player from taking damage multiple times.
        StartCoroutine(SequenceActions(() => hitbox.enabled = false, 1f, () => hitbox.enabled = true));

        // Blink the sprite to indicate damage has been taken and invincibility frames.
        StartCoroutine(PlayerAnimationManager.SpriteRoutine(0.5f, sprite));
        //TODO: Keep in mind that we are disabling the collider for a time here. I.e, the player becomes "ghosted", meaning they can pass through walls and enemies.
    }

    void HandleDeath()
    {
        // Debug that the player is dead and freeze their movement.
        Log("Player is dead.");
        RB.FreezeAllConstraints();
        RB.FreezeConstraints(RigidbodyConstraints2DExtended.Constraints.FreezeX);

        // Unsubscribe from the delegate when the player is dead.
        onPlayerTakeDamage -= PerformOnTakeDamage;

        // Play the death animation (This is just a placeholder/for fun).
        transform.GetChild(0).transform.GetChild(2).GetComponentInChildren<ParticleSystem>().gameObject.SetActive(true);
        sprite.enabled = false;

        // Close the curtains and wait 2 seconds before loading the game over scene.
        var delayTask = DelayedTaskAsync(() => transition.CloseCurtains(), 2).AsTask();
        delayTask.ContinueWith(_ => Log("//TODO: Load Game Over Scene"));
    }
}