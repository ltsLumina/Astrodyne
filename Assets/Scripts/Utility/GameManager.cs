using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static SceneManagerExtended;
using ReadOnly = Essentials.ReadOnlyAttribute;

public class GameManager : SingletonPersistent<GameManager>
{
    public Player Player { get; private set; }

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
}