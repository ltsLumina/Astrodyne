#region
using Cinemachine;
using UnityEngine;
#endregion

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    CinemachineVirtualCamera cinemachineVCam;
    float shakeTimer;

    void Awake()
    {
        Instance = this;

        // if (Instance == null)
        // {
        //     Instance = this;
        //     DontDestroyOnLoad(gameObject);
        // }
        // else
        // {
        //     Destroy(gameObject);
        // }

        cinemachineVCam = GetComponent<CinemachineVirtualCamera>();
    }

    /// <summary>
    /// Camera Shake for Cinemachine.
    /// <remarks>Syntax: CameraShake.instance.ShakeCamera(1.5f, .2f);</remarks>
    /// </summary>
    /// <param name="intensity"></param>
    /// <param name="time"></param>
    public void ShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachinePerlin =
            cinemachineVCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachinePerlin.m_AmplitudeGain = intensity;
        shakeTimer = time;
    }

    void Update()
    {
        if (!(shakeTimer > 0)) return;
        shakeTimer -= Time.deltaTime;
        if (!(shakeTimer <= 0f)) return;

        //time over
        CinemachineBasicMultiChannelPerlin cinemachinePerlin =
            cinemachineVCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachinePerlin.m_AmplitudeGain = 0f;
    }
}