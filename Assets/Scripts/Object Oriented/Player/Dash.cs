using System.Collections;
using UnityEngine;
using static Essentials;
using static UnityEngine.Debug;

public class Dash : MonoBehaviour
{
    Player player;
    float buttonCooldown = 0.5f; // Half a second before reset
    int buttonCount = 0;
    string lastKeyPressed;

              [Header("Dash"), Space(5)]
    [SerializeField] int dashCount;
    [SerializeField] float dashSpeed;
    [SerializeField] Vector2 dashEndSpeed;
    [SerializeField] int dashAmount;

    [Header("Dash Timers"), Space(5), SerializeField, ReadOnly]
    bool dashRefilling;
    [SerializeField] float dashRefillTime;
    [SerializeField] float dashEndTime;
    [SerializeField] float dashAttackTime;
    [SerializeField] float dashSleepTime;
    [SerializeField, Range(0.01f, 0.5f)] float inputBufferTime;

    [Header("Read-only Fields")]
    [SerializeField, ReadOnly] float lastPressedDashTime;
    [SerializeField, ReadOnly] bool isDashing;
    Camera cam;
    Vector2 mousePos;

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

        PerformDash();

        void PerformDash()
        {
            void DashRoutine()
            {
                LastPressedDashTime = inputBufferTime;

                mousePos = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;

                if (CanDash() && LastPressedDashTime > 0)
                    //Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
                    Sleep(dashSleepTime); // Implement this once there are more visual effects to the dash.

                if (player.moveInput != Vector2.zero) { StartCoroutine(nameof(StartDash), player.moveInput); }
                else
                {
                    Log("Can't dash; standing still. (No input)");
                    StartCoroutine(nameof(StartDash), mousePos);
                }
            }

            if (Input.anyKeyDown) //TODO: a bug occurs here if you press two different keys, it counts as double tapping and proceeds to dash.
            {
                if (buttonCooldown > 0 && buttonCount == 1 /*Number of Taps you want Minus One*/) { DashRoutine(); }
                else
                {
                    buttonCooldown =  0.5f;
                    buttonCount    += 1;
                }
            }

            if (buttonCooldown > 0) buttonCooldown -= 1 * Time.deltaTime;
            else buttonCount =  0;

            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Space))
                DashRoutine();

            //TODO: Find a different way to handle the failed dash. This is a bit of a hack.
        }
    }

    IEnumerator StartDash(Vector2 dashDirection)
    {
        Log("Dash Start");
        IsDashing = true;

        LastPressedDashTime = 0;

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

    IEnumerator RefillDash(int amount)
    {
        dashRefilling = true;
        yield return new WaitForSeconds(dashRefillTime);
        dashRefilling = false;
        dashCount     = Mathf.Min(dashAmount, dashCount + 1);
    }

    #region DASH CHECKS
    //TODO: dash only refreshes to a maximum of one.
    public bool CanDash()
    {
        if (!IsDashing && dashCount < dashAmount && !dashRefilling)
            StartCoroutine(nameof(RefillDash), 1);
        return dashCount > 0;
    }

    void Sleep(float duration)
    {
        //Method used so we don't need to call StartCoroutine everywhere
        StartCoroutine(nameof(PerformSleep), duration);
    }

    IEnumerator PerformSleep(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration); //Must be Realtime since timeScale with be 0
        Time.timeScale = 1;
    }
    #endregion
}