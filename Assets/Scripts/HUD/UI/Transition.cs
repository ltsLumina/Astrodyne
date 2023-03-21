using System;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// TODO: I want to make this inherit from the Singleton class, but that caused an error outside of runtime.
/// </summary>
public class Transition : SingletonPersistent<Transition>
{
    [SerializeField] Animator animLeft;
    [SerializeField] Animator animRight;
    readonly static int FadeRight = Animator.StringToHash("Fade Right");
    readonly static int FadeLeft = Animator.StringToHash("Fade Left");

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
        animRight.SetBool(FadeRight, true);
        animLeft.SetBool(FadeLeft, true);
    }

    public void OpenCurtains()
    {
        if (!curtainsClosed) return;
        curtainsClosed = !curtainsClosed;
        animRight.SetBool(FadeRight, false);
        animLeft.SetBool(FadeLeft, false);
    }
}