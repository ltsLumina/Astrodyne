using System;
using System.Collections;
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
}

internal static class UsefulMethods
{
    /// <summary>
    /// Allows you to call a method after a delay through the use of delegates.
    /// </summary>
    /// <param name="delayInSeconds">The delay before running the method.</param>
    /// <param name="action">The action or method to run.</param>
    /// <param name="debugLog">Whether or not to debug the waiting message and the completion message.</param>
    public static async Task DoAfterDelay(Action action, float delayInSeconds, bool debugLog = false, CancellationToken cancellationToken = default)
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
}

public class ReadOnlyAttribute : PropertyAttribute
{ }

/// <summary>
/// Allows you to add '[ReadOnly]' before a variable so that it is shown but not editable in the inspector.
/// Small but useful script, to make your inspectors look pretty and useful :D
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