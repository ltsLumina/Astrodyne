#region
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#endregion

public abstract class InputManager : MonoBehaviour
{
    readonly List<KeyCode> doubleTapKeys = new () { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };
    readonly Dictionary<KeyCode, float> doubleTapTimers = new ();

    [SerializeField] float doubleTapTimeThreshold = 0.4f;

    protected bool CheckDoubleTap()
    {

        foreach (KeyCode key in doubleTapKeys.ToList().Where(Input.GetKeyDown))
        {
            if (!Input.GetKeyDown(key)) continue;

            if (doubleTapTimers.ContainsKey(key) && Time.time - doubleTapTimers[key] < doubleTapTimeThreshold)
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
    protected void Sleep(float duration)
    {
        //Method used so we don't need to call StartCoroutine everywhere
        StartCoroutine(nameof(PerformSleep), duration);
    }

    IEnumerator PerformSleep(float duration)
    {
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration); //Must be Realtime since timeScale with be 0
        Time.timeScale = 1;
    }
    #endregion
}