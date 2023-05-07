using System.Collections;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Essentials.Deprecated;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI loadingText;
    [SerializeField] Button playButton;
    [SerializeField] Button quitButton;

    SpriteRenderer spriteRenderer;

    float delayInSeconds = 2;
    float timer;
    int dotCount;

    void Start() => spriteRenderer = GetComponent<SpriteRenderer>();

    void Update()
    {
        TimerLogic();

        void TimerLogic()
        { // For each second that passes since the game started, increment the delayInSeconds variable by 0.25, independent of the frame rate.
            // This is simply for fun, and to make the loading screen more interesting.
            timer += Time.deltaTime;
            if (!(timer >= 1)) return;
            timer          =  0;
            delayInSeconds += 0.25f;

            if (timer >= 10)
            {
                timer          = 0;
                delayInSeconds = 2;
            }
        }
    }

    /// <summary>
    /// Handles the loading screen by enabling the loading screen and disabling the buttons, then displaying a "Loading" text.
    /// After the specified delay, the next scene is loaded. However, I had fun making this and decided that I wanted to make the loading text more interesting.
    /// Thereby, for each second that passes, the delayInSeconds variable is incremented by 0.25 to make the loading screen more interesting as it "takes longer" to load.
    /// </summary>
    public void LoadingScreenRoutine()
    {
        // Enable the loading screen and disable the buttons.
        spriteRenderer.enabled         = true;
        playButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);

        // move the loading text and set it to "Loading"
        loadingText.transform.localPosition = new (0, 0, 0);
        loadingText.text = "Loading...";

        Debug.Log($"Time to wait: {delayInSeconds} seconds.");
        DelayedTaskAsync(SceneManagerExtended.LoadNextScene, delayInSeconds).AsTask();
        StartCoroutine(LoadingProcess());
    }

    IEnumerator LoadingProcess()
    {
        while (true)
        {
            // add a dot to the loading text
            dotCount++;
            if (dotCount > 3) dotCount = 1;
            string dots   = "";
            for (int i = 0; i < dotCount; i++) dots += ".";
            loadingText.text = "Loading" + dots;

            // wait for 1 second
            yield return new WaitForSeconds(1f);

            // increment the timer
            timer += 1f;
            if (timer >= delayInSeconds)
            {
                // stop the loading process
                StopCoroutine(LoadingProcess());
                break;
            }
        }
    }
}