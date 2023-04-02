#region
using UnityEngine;
#endregion

public class SpeedlineEffect : MonoBehaviour
{
    #region Speedline Variables
    ParticleSystem speedline;
    Rigidbody2D playerRB;

    [Header("Emission Variables")]
    [SerializeField] float emissionRate;
    #endregion

    void Start()
    {
        playerRB  = FindObjectOfType<Player>().RB;
        speedline = GetComponentInChildren<ParticleSystem>();

        AdjustLineSpeed(0);
    }

    void Update() => SpeedlineAdjustment();

    void SpeedlineAdjustment()
    {
        switch (playerRB.velocity.x)
        {
            case > 30:
                AdjustLineSpeed(emissionRate);
                break;

            case < -30:
                AdjustLineSpeed(emissionRate);
                break;

            case < 30:
                AdjustLineSpeed(0);
                break;
        }
    }

    void AdjustLineSpeed(float acceleration)
    {
        ParticleSystem.EmissionModule emissionMod = speedline.emission;
        emissionMod.rateOverTime = acceleration;
    }
}