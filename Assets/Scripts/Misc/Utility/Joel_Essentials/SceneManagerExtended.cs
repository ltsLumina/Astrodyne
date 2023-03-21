#region
using UnityEngine;
using UnityEngine.SceneManagement;
#endregion

public static class SceneManagerExtended
{
    static int previousScene;

    /// <summary>
    ///     Loads the scene with the specified build index.
    /// </summary>
    /// <param name="buildIndex"></param>
    public static void LoadScene(int buildIndex)
    {
        previousScene = SceneManager.GetActiveScene().buildIndex;

        SceneManager.LoadScene(ClampBuildIndex(buildIndex));
    }

    /// <summary>
    ///     Reloads the currently active scene.
    /// </summary>
    public static void ReloadScene()
    {
        SceneManager.LoadScene(ClampBuildIndex(SceneManager.GetActiveScene().buildIndex));
    }

    /// <summary>
    ///     Loads the next scene according to build index order.
    /// </summary>
    public static void LoadNextScene()
    {
        previousScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(ClampBuildIndex(SceneManager.GetActiveScene().buildIndex + 1));
    }

    /// <summary>
    ///     Loads the previously loaded scene.
    /// </summary>
    public static void LoadPreviousScene() { SceneManager.LoadScene(ClampBuildIndex(previousScene)); }

    /// <summary>
    ///     If the buildIndex is outside the range of build indexes, return 0.
    /// </summary>
    /// <param name="buildIndex"></param>
    /// <returns></returns>
    static int ClampBuildIndex(int buildIndex)
    {
        if (buildIndex < 0 || buildIndex >= SceneManager.sceneCountInBuildSettings)
        {
            Debug.LogWarning("BuildIndex invalid/unavailable. Loading scene with an index of 0...");
            buildIndex = 0;
        }
        return buildIndex;
    }

    /// <summary>
    ///     Stops playmode if used in the editor, or quits the application if in a build.
    /// </summary>
    public static void QuitGame()
    {
#if UNITY_EDITOR
        Debug.Log("Quitting game...");
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}