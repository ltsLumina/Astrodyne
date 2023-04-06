#region
using System;
using UnityEngine;
#endregion

//TODO: only perform speedline when player does mega-dash. (i.e actives the card upgrade.)
[Obsolete] // Obsolete, for the time being.
public class SpeedlineEffect : MonoBehaviour
{
    #region Speedline Variables
    ParticleSystem speedline;
    Dash dashComponent;
    Rigidbody2D playerRB;

    [Header("Emission Variables")]
    [SerializeField] float emissionRate;
    const float VELOCITY_THRESHOLD = 15f;
    #endregion

    void Start()
    {
        playerRB      = FindObjectOfType<Player>().RB;
        dashComponent = FindObjectOfType<Dash>();
        speedline     = GetComponentInChildren<ParticleSystem>();

        AdjustLineSpeed(0);
    }

    void Update() => SpeedlineAdjustment();

    void SpeedlineAdjustment()
    {
        if (dashComponent.IsDashing) AdjustLineSpeed(emissionRate);
        else AdjustLineSpeed(0);

        // List<object> adjustment = new ();
        //
        // switch (playerRB.velocity.x)
        // {
        //     case > VELOCITY_THRESHOLD:
        //         AdjustLineSpeed(emissionRate);
        //         adjustment.Add(new WaitForSeconds(1.5f));
        //         break;
        //
        //     case < -VELOCITY_THRESHOLD:
        //         AdjustLineSpeed(emissionRate);
        //         adjustment.Add(new WaitForSeconds(1.5f));
        //         break;
        //
        //     case < VELOCITY_THRESHOLD:
        //         AdjustLineSpeed(0);
        //         adjustment.Add(new WaitForSeconds(1.5f));
        //         break;
        // }
        //
        // return adjustment.GetEnumerator();
    }

    void AdjustLineSpeed(float acceleration)
    {
        ParticleSystem.EmissionModule emissionMod = speedline.emission;
        emissionMod.rateOverTime = acceleration;
    }
}