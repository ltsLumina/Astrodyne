#region
using System.Collections;
using UnityEngine;
using static Essentials;
using static UnityEngine.Debug;
#endregion

public class Dash : InputManager
{
    [Header("Cached References")] Player player;
    Camera cam;

    [Header("Dash"), Space(5), SerializeField] int dashCount;
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

    [Header("Read-only Fields"), SerializeField, ReadOnly] float lastPressedDashTime;
    [SerializeField, ReadOnly] bool isDashing;

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
    }

    void Update()
    {
        LastPressedDashTime -= Time.deltaTime;

        Vector2 dashDirection = Vector2.zero;

        // Check for dash input.
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Space)) dashDirection = GetDashDirection();
        else if (CheckDoubleTap()) dashDirection                                                  = InputDirection();

        // If a dash direction was detected, start the dash routine.
        if (dashDirection != Vector2.zero) DashRoutine(dashDirection);
    }

    Vector2 GetDashDirection()
    {
        // If the player is moving, dash in the direction of the input, otherwise, dash in the direction of the mouse.
        if (player.moveInput != Vector2.zero) return player.moveInput;
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        return mousePos;
    }

    void DashRoutine(Vector2 dashDirection)
    {
        if (CanDash() && LastPressedDashTime <= 0 && !IsDashing)
        {
            IsDashing = true;
            Sleep(dashSleepTime);

            // Run the dash coroutine.
            StartCoroutine(nameof(StartDash), dashDirection);
        }

        //Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
    }

    #region DASH METHODS
    IEnumerator StartDash(Vector2 dashDirection)
    {
        Log("Dashing!");

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
        player.RB.velocity = dashEndSpeed * dashDirection.normalized;

        while (Time.time - startTime <= dashEndTime) yield return null;

        //Dash over
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
    public bool CanDash()
    {
        if (!IsDashing && dashCount < dashMaxAmount && !dashRefilling) StartCoroutine(nameof(RefillDash), 1);

        return dashCount > 0;
    }
    #endregion
}