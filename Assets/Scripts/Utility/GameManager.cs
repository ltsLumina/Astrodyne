using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static SceneManagerExtended;
using ReadOnly = Essentials.ReadOnlyAttribute;

public class GameManager : SingletonPersistent<GameManager>
{
    // Singleton reference to the player.
    public Player Player { get; private set; } //TODO: remove this, it's bad. Only used in essentials though.

    [Header("Score"), ReadOnly, SerializeField]
    int score;

    void Start() => Player = FindObjectOfType<Player>();

    /// <summary>
    /// Game Manager versions of the LoadNextScene and QuitGame methods to be used in conjunction with the OnClick method in the Unity Editor.
    /// </summary>
    public void LoadNextScene() => SceneManagerExtended.LoadNextScene();
    public void QuitGame() => SceneManagerExtended.QuitGame();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
            Essentials.UsefulMethods.DelayedTaskAsync(ReloadScene, 1.5f).AsTask();
    }

    /// <summary>
    ///     Knockback any gameobject with a rigidbody2D.
    /// </summary>
    /// <remarks> Keep in mind that the direction parameter already is normalized. </remarks>
    public static void KnockbackRoutine(GameObject gameObject, Vector2 direction, float force = 10)
    {
        var rigidbody = gameObject.GetComponent<Rigidbody2D>();
        if (rigidbody != null)
            rigidbody.AddForce(direction.normalized * force, ForceMode2D.Impulse);
    }

    #region SLEEP METHOD
    public void Sleep(float duration) =>

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