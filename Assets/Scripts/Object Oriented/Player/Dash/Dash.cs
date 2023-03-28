using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using static Essentials;

public class Dash : MonoBehaviour
{
    Player player;

    [Header("Dash"), Space(5)]
    [SerializeField] int dashCount;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashEndSpeed;
    [SerializeField] float dashMaxCount;

    [Header("Dash Timers"), Space(5), SerializeField, ReadOnly]
    bool dashRefilling;
    [SerializeField] float dashRefillTime;
    [SerializeField] float dashEndTime;
    [SerializeField] float dashAttackTime;
    [SerializeField] float dashSleepTime;

    public bool IsDashing { get; set; }

    void Start()
    {
        player = GetComponent<Player>();
    }

    protected internal IEnumerator StartDash(Vector2 dir)
    {
        Debug.Log("Dash Start");

        IsDashing = true;
        //Overall this method of dashing aims to mimic Celeste, if you're looking for
        // a more physics-based approach try a method similar to that used in the jump

        float startTime = Time.time;

        dashCount--;

        //We keep the player's velocity at the dash speed during the "attack" phase (in celeste the first 0.15s)
        while (Time.time - startTime <= dashAttackTime)
        {
            player.RB.velocity = dir.normalized * dashSpeed;
            //Pauses the loop until the next frame, creating something of a Update loop.
            //This is a cleaner implementation opposed to multiple timers and this coroutine approach is actually what is used in Celeste :D
            yield return null;
        }

        startTime = Time.time;

        //_isDashAttacking = false;

        //Begins the "end" of our dash where we return some control to the player but still limit run acceleration (see Update() and Run())
        player.RB.velocity = dashEndSpeed * dir.normalized;

        while (Time.time - startTime <= dashEndTime) yield return null;

        //Dash over
        IsDashing = false;
    }

    IEnumerator RefillDash(int amount)
    {
        //Shoot cooldown, so we can't constantly dash along the ground, again this is the implementation in Celeste, feel free to change it up
        dashRefilling = true;
        yield return new WaitForSeconds(dashRefillTime);
        dashRefilling = false;
        dashCount     = Mathf.Min(amount, dashCount + 1);
    }

    #region DASH CHECKS
    public bool CanDash()
    {
        if (!IsDashing && dashCount < dashMaxCount && !dashRefilling)
            StartCoroutine(nameof(RefillDash), 1);
        return dashCount > 0;
    }
    #endregion
}