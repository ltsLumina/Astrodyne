#region
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.KeyCode;
#endregion

public abstract class DashInputManager : MonoBehaviour
{
    readonly List<KeyCode> doubleTapKeys = new ()
    {
        // W, A, S and D keys.
        W, A, S, D
        // Arrow keys.
        , UpArrow, LeftArrow, DownArrow, RightArrow
    };
    readonly Dictionary<KeyCode, float> doubleTapTimers = new ();

    //TODO: put this at the bottom of the serializedfields. // nevermind, need to make custom editor script and that's not worth it.
    const float DOUBLE_TAP_TIME_THRESHOLD = 0.25f;
    
    protected static Vector2 InputDirection() =>
        new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

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

}