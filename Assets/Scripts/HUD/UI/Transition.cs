using System;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// //TODO: Write a summary.
/// </summary>
public class Transition : SingletonPersistent<Transition>
{
    [SerializeField] Animator animLeft;
    [SerializeField] Animator animRight;
    readonly static int fadeRight = Animator.StringToHash("Fade Right");
    readonly static int fadeLeft = Animator.StringToHash("Fade Left");

    bool curtainsClosed;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)      && !curtainsClosed) CloseCurtains();
        else if (Input.GetKeyDown(KeyCode.Escape) && curtainsClosed) OpenCurtains();
    }

    public void CloseCurtains()
    {
        if (curtainsClosed) return;
        curtainsClosed = !curtainsClosed;
        animRight.SetBool(fadeRight, true);
        animLeft.SetBool(fadeLeft, true);
    }

    public void OpenCurtains()
    {
        if (!curtainsClosed) return;
        curtainsClosed = !curtainsClosed;
        animRight.SetBool(fadeRight, false);
        animLeft.SetBool(fadeLeft, false);
    }
}