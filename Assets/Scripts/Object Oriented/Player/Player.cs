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
    SpriteRenderer sr;
    Transition transition;
    SpriteRenderer spriteRenderer;

    // Turn moveInput into property eventually.

    [Header("Movement"), Space(5)]
    [SerializeField] float moveSpeed = 5f;
    [ReadOnly] public Vector2 moveInput;

    [Header("Health"), Space(20)]
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth = 100f;
    #endregion

    #region Properties
    public Rigidbody2D RB { get; set; }
    public bool IsFacingRight { get; private set; } = true;
    public float CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = value;
            IsDead        = currentHealth <= 0;
            if (IsDead) HandleDeath();
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
        Movement();
        FaceMouse();
        HealthLogic();

        void Boundaries()
        {
            // Keeps the player within the camera's view.
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

        void HealthLogic()
        {
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }
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