#region
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.KeyCode;
#endregion

public abstract class InputManager : MonoBehaviour
{
    readonly List<KeyCode> doubleTapKeys = new ()
    {
        // W, A, S and D keys.
        W, A, S, D
        // UpArrow, LeftArrow, DownArrow and RightArrow keys.
        , UpArrow, LeftArrow, DownArrow, RightArrow
    };
    readonly Dictionary<KeyCode, float> doubleTapTimers = new ();

    //TODO: put this at the bottom of the serializedfields.
    const float DOUBLE_TAP_TIME_THRESHOLD = 0.4f;

    protected bool CheckDoubleTap()
    {
        foreach (KeyCode key in doubleTapKeys.ToList().Where(Input.GetKeyDown))
        {
            if (!Input.GetKeyDown(key)) continue;

            if (doubleTapTimers.ContainsKey(key) && Time.time - doubleTapTimers[key] < DOUBLE_TAP_TIME_THRESHOLD)
            {
                doubleTapTimers[key] = -1f;
                return true;
            }

            doubleTapTimers[key] = Time.time;
        }

        return false;
    }

    protected static Vector2 InputDirection() =>
        new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

    #region SLEEP METHOD
    protected void Sleep(float duration) =>

        //Method used so we don't need to call StartCoroutine everywhere
        StartCoroutine(nameof(PerformSleep), duration);

    IEnumerator PerformSleep(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration); //Must be Realtime since timeScale with be 0
        Time.timeScale = 1;
    }
    #endregion
}