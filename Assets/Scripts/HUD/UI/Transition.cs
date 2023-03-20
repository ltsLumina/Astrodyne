using System;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// TODO: I want to make this inherit from the Singleton class, but that caused an error outside of runtime.
/// </summary>
public class Transition : MonoBehaviour
{
    [SerializeField] Animator animLeft;
    [SerializeField] Animator animRight;
    readonly static int FadeRight = Animator.StringToHash("Fade Right");
    readonly static int FadeLeft = Animator.StringToHash("Fade Left");

    bool hasFaded;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)      && !hasFaded) CloseCurtains();
        else if (Input.GetKeyDown(KeyCode.Escape) && hasFaded) OpenCurtains();
    }

    public void CloseCurtains()
    {
        if (hasFaded) return;
        hasFaded = !hasFaded;
        animRight.SetBool(FadeRight, true);
        animLeft.SetBool(FadeLeft, true);

    }

    public void OpenCurtains()
    {
        if (!hasFaded) return;
        hasFaded = !hasFaded;
        animRight.SetBool(FadeRight, false);
        animLeft.SetBool(FadeLeft, false);

    }
}