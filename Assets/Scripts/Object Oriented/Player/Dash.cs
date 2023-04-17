#region
using System.Collections;
using UnityEngine;
using Essentials;
using static UnityEngine.Debug;
#endregion

public class Dash : InputManager
{
    [Header("Cached References")]
    Player player;
    Camera cam;
    PlayerAnimator playerAnimator;

    [Header("Dash"), Space(5), SerializeField]
    int dashCount;
    [SerializeField] float dashSpeed;
    [SerializeField] Vector2 dashEndSpeed;
    [SerializeField] int dashMaxAmount;

    [Header("Dash Timers"), Space(5), SerializeField, ReadOnly]
    bool dashRefilling;
    [SerializeField] float dashRefillTime;
    [SerializeField] float dashEndTime;
    [SerializeField] float dashAttackTime;
    [SerializeField] float dashSleepTime;
    [Space(5), SerializeField, Range(0.01f, 2f), Tooltip("The amount of time the player has to wait before dashing again. A higher value means the player can dash less often.")]
    float dashBufferTime;

    [Header("Read-only Fields")]
    [SerializeField, ReadOnly] float lastPressedDashTime;
    [SerializeField, ReadOnly] bool isDashing;

    // Delegates
    public delegate void OnDash();
    public event OnDash onDash;

    public bool IsDashing
    {
        get => isDashing;
        private set => isDashing = value;
    }

    public float LastPressedDashTime
    {
        get => lastPressedDashTime;
        set => lastPressedDashTime = value;
    }

    void Awake()
    {
        player = GetComponent<Player>();
        cam    = Camera.main;
        playerAnimator = GetComponentInChildren<PlayerAnimator>();
    }

    void Update()
    {
        LastPressedDashTime -= Time.deltaTime;

        Vector2 dashDirection = Vector2.zero;

        // Check for dash input.
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Space)) dashDirection = GetDashDirection();
        else if (CheckDoubleTap()) dashDirection = InputDirection();

        // If a dash direction was detected, start the dash routine.
        if (dashDirection != Vector2.zero) DashRoutine(dashDirection);
    }

    Vector2 GetDashDirection()
    {
        // If the player is moving, dash in the direction of the input, otherwise, dash in the direction of the mouse.
        if (player.MoveInput != Vector2.zero) return player.MoveInput;
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        return mousePos;
    }

    void DashRoutine(Vector2 dashDirection)
    {
        if (CanDash() && LastPressedDashTime <= 0 && !IsDashing)
        {
            IsDashing = true;
            //Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
            Sleep(dashSleepTime);

            // Run the dash coroutine.
            onDash?.Invoke();
            StartCoroutine(nameof(StartDash), dashDirection);
        }
    }

    #region DASH METHODS
    IEnumerator StartDash(Vector2 dashDirection)
    {
        Log("Dashing!");
        Vector2 originalVelocity = player.RB.velocity;

        LastPressedDashTime = dashBufferTime;

        float startTime = Time.time;

        dashCount--;

        //isDashAttacking = false;

        //We keep the player's velocity at the dash speed during the "attack" phase (in celeste the first 0.15s)
        while (Time.time - startTime <= dashAttackTime)
        {
            player.RB.velocity = dashDirection.normalized * dashSpeed;

            //Pauses the loop until the next frame, creating something of a Update loop.
            //This is a cleaner implementation opposed to multiple timers and this coroutine approach is actually what is used in Celeste :D
            yield return null;
        }

        startTime = Time.time;

        //isDashAttacking = false;

        //Begins the "end" of our dash where we return some control to the player but still limit run acceleration (see Update() and Run())
        while (Time.time - startTime <= dashEndTime)
        {
            // Get the current movement input
            Vector2 moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            // If the character is not moving, continue the loop
            if (moveInput == Vector2.zero || moveInput != Vector2.zero)
            {
                // Set the character's velocity to the dash direction multiplied by the dash end speed
                player.RB.velocity = dashDirection.normalized * dashEndSpeed;

                yield return null;
                continue;
            }

            // Get the normalized movement input
            Vector2 moveDirection = moveInput.normalized;

            // Calculate the movement velocity while dashing
            Vector2 dashVelocity = moveDirection * dashEndSpeed;

            // Set the character's velocity to the dash velocity plus the original velocity
            player.RB.velocity = dashDirection.normalized * dashSpeed + dashVelocity;

            yield return null;
        }

        player.RB.velocity = originalVelocity;

        //Dash finished.
        IsDashing = false;

    }

    //TODO: dash only refreshes to a maximum of one. This is a bug.
    IEnumerator RefillDash(int refillAmount)
    {
        dashRefilling = true;
        yield return new WaitForSeconds(dashRefillTime);
        dashRefilling = false;
        dashCount     = Mathf.Min(dashMaxAmount, dashCount + refillAmount);
    }
    #endregion

    #region DASH CHECKS
    protected bool CanDash()
    {
        if (!IsDashing && dashCount < dashMaxAmount && !dashRefilling)
            StartCoroutine(nameof(RefillDash), 1);

        return dashCount > 0;
    }
    #endregion
}