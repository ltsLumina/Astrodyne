using UnityEngine;

public class Transition : SingletonPersistent<Transition>
{
    [SerializeField] Animator animLeft;
    [SerializeField] Animator animRight;

    [Space(10), Header("Configurable Parameters")]
    [SerializeField] bool allowInput;
    readonly static int FadeRight = Animator.StringToHash("Fade Right");
    readonly static int FadeLeft = Animator.StringToHash("Fade Left");

    bool curtainsClosed;

    void Update()
    {
        if (!allowInput) return;
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