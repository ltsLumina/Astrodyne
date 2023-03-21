using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

internal static class UsefulShortcuts
{
    /// <summary>
    /// Alt+ C to clear the console.
    /// </summary>
    [Shortcut("Clear Console", KeyCode.C, ShortcutModifiers.Alt)]
    public static void ClearConsole()
    {
        var assembly = Assembly.GetAssembly(typeof(SceneView));
        var type     = assembly.GetType("UnityEditor.LogEntries");
        var method   = type.GetMethod("Clear");
        method?.Invoke(new (), null);
    }

    /// <summary>
    /// A collection of debugging shortcuts.
    /// Includes keyboard shortcuts tied to the F-keys, as well as context menus.
    /// Note: These methods are local functions, and are only accessible within this method.
    /// </summary>

#if UNITY_EDITOR
    [Shortcut("Damage Player", KeyCode.F1), ContextMenu("Damage Player")]
    static void DamagePlayer()
    {
        // Damage the player by 10.
        GameManager.Instance.Player.CurrentHealth -= 10;
        Debug.Log("Player damaged.");
    }

    [Shortcut("Heal Player", KeyCode.F2), ContextMenu("Heal Player")]
    static void HealPlayer()
    {
        // Heal the player by 10.
        GameManager.Instance.Player.CurrentHealth += 10;
        Debug.Log("Player healed.");
    }

    [Shortcut("Kill Player", KeyCode.F3), ContextMenu("Kill Player")]
    static void KillPlayer()
    {
        // Kill the player.
        GameManager.Instance.Player.CurrentHealth = 0;
        Debug.Log("Player killed.");
    }

    [Shortcut("Reload Scene", KeyCode.F5), ContextMenu("Reload Scene")]
    static void ReloadScene()
    {
        // Reload Scene
        SceneManagerExtended.ReloadScene();
        Debug.Log("Scene reloaded.");
    }
#endif
}

internal static class UsefulMethods
{
    /// <summary>
    /// Allows you to call a method after a delay through the use of an asynchronous operation.
    /// Keep in mind, that the method using this method needs to be an asynchronous method as well
    /// UNLESS it is called at the very end of the method, where you can discard ( _= ) the task.
    /// <example> _= DoAfterDelayAsync( () => action(), delayInSeconds);  </example>
    /// </summary>
    /// <param name="action">The action or method to run.</param>
    /// <param name="delayInSeconds">The delay before running the method.</param>
    /// <param name="debugLog">Whether or not to debug the waiting message and the completion message.</param>
    /// <param name="cancellationToken">Cancellation Token for cancelling the currently running task. </param>
    public static async Task DoAfterDelayAsync(Action action, float delayInSeconds, bool debugLog = false, CancellationToken cancellationToken = default)
    {
        if (debugLog) Debug.Log("Waiting for " + delayInSeconds + " seconds...");
        var timeSpan = TimeSpan.FromSeconds(delayInSeconds);
        try
        {
            await Task.Delay(timeSpan, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            if (debugLog) Debug.Log("Action cancelled.");
            return;
        }
        action();
        if (debugLog) Debug.Log("Action completed.");
    }

    /// <summary>
    /// !WARNING! This method is not asynchronous, and will block the main thread, causing the game to freeze.
    /// However, the purpose of this method is to allow you to call a method after a delay, without having to make the method asynchronous.
    /// Unsure if this method even works in its current state.
    /// </summary>
    /// <param name="action">The action or method to run.</param>
    /// <param name="delayInSeconds">The delay before running the method.</param>
    /// <param name="debugLog">Whether or not to debug the waiting message and the completion message.</param>
    /// <param name="onComplete">An action to be completed after the initial action is finished. Not required to be used.</param>
    public static void DoAfterDelay(Action action, float delayInSeconds, bool debugLog = false, Action onComplete = null)
    {
        if (debugLog) Debug.Log("Waiting for " + delayInSeconds + " seconds...");
        var timeSpan = TimeSpan.FromSeconds(delayInSeconds);
        Task.Delay(timeSpan).ContinueWith(_ =>
        {
            action();
            if (debugLog) Debug.Log("Action completed.");
            onComplete?.Invoke();
        });
    }
}

public class ReadOnlyAttribute : PropertyAttribute
{ }

/// <summary>
/// Allows you to add '[ReadOnly]' before a variable so that it is shown but not editable in the inspector.
/// Small but useful script, to make your inspectors look pretty and useful :D
/// <example> [SerializedField, ReadOnly] int myInt; </example>
/// </summary>
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = true;
    }
}