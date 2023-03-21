using UnityEngine;

public class GameManager : SingletonPersistent<GameManager>
{
    [Header("Cached References")]
    internal Player Player;

    [Header("Score")]
    [ReadOnly, SerializeField] int score;

    void Start()
    {
        Player  = FindObjectOfType<Player>();
    }

    /// <summary>
    /// Game Manager versions of the LoadNextScene and QuitGame methods to be used in conjunction with the OnClick method in the Unity Editor.
    /// </summary>
    public void LoadNextScene() => SceneManagerExtended.LoadNextScene();
    public void QuitGame() => SceneManagerExtended.QuitGame();
}