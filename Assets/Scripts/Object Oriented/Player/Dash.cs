using System.Collections;
using UnityEngine;
using static Essentials;
using static UnityEngine.Debug;

public class Dash : MonoBehaviour
{
    [Header("Cached References")]
    Player player;
    Camera cam;
    bool doubleTap;
    float doubleTapTime;

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
        player         = GetComponent<Player>();
        cam            = Camera.main;
    }

    void Update()
    {
        LastPressedDashTime -= Time.deltaTime;

        DoubleTap();
        
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.Space))
            DashRoutine();

        void DashRoutine() //TODO: refactor.
        {
            LastPressedDashTime = inputBufferTime;

            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition) - transform.position;

            if (CanDash() && LastPressedDashTime > 0)
                //Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
                Sleep(dashSleepTime); // Implement this once there are more visual effects to the dash.

            //TODO: Find a different way to handle the failed dash. This is a bit of a hack.
            StartCoroutine(nameof(StartDash), player.moveInput != Vector2.zero ? player.moveInput : mousePos);
        }
    }

    void DoubleTap()
    {
        //TODO: switch-case

        if (Input.GetKeyDown(KeyCode.W) && doubleTap)
        {
            if (Time.time - doubleTapTime < 0.4f)
            {
                Log("Double-tapped");
                doubleTapTime = 0f;
            }

            doubleTap = false;
        }

        if (!Input.GetKeyDown(KeyCode.W) || doubleTap) return;
        doubleTap     = true;
        doubleTapTime = Time.time;

        if (Input.GetKeyDown(KeyCode.S) && doubleTap)
        {
            if (Time.time - doubleTapTime < 0.4f)
            {
                Log("Double-tapped");
                doubleTapTime = 0f;
            }

            doubleTap = false;
        }

        if (!Input.GetKeyDown(KeyCode.S) || doubleTap) return;
        doubleTap     = true;
        doubleTapTime = Time.time;

    }

    #region DASH METHODS
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
    #endregion

    #region DASH CHECKS
    //TODO: dash only refreshes to a maximum of one.
    public bool CanDash()
    {
        if (!IsDashing && dashCount < dashAmount && !dashRefilling)
            StartCoroutine(nameof(RefillDash), 1);
        return dashCount > 0;
    }
    #endregion

    #region SLEEP METHOD
    public void Sleep(float duration)
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