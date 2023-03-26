#region
using System;
using UnityEngine;
using Cinemachine;
#endregion

public class CinemachineCamera : MonoBehaviour
{
    // Confine the camera to the bounds of the level
    public CinemachineConfiner Confiner;

    void LateUpdate()
    {
        SetConfiner();
    }

    // confine the camera to the bounds of the level
    public void SetConfiner()
    {
        Confiner.m_ConfineScreenEdges = true;
    }
}