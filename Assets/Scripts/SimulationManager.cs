using NUnit.Framework;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public static bool isPaused = false;
    static float originalTimeScale = 1f;
    float speedChangeRate = 1f;

    float holdDurationToQuit = 2f;
    float quitHoldTimer = 0f;
    bool isHoldingEsc = false;

    float resetHoldTimer = 0f;
    float holdDurationToReset = 2f;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isHoldingEsc = true;
            quitHoldTimer = 0f;
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (quitHoldTimer < holdDurationToQuit)
            {
                TogglePause();
            }

            isHoldingEsc = false;
            quitHoldTimer = 0f;
        }

        if (isHoldingEsc)
        {
            quitHoldTimer += Time.unscaledDeltaTime;

            if (quitHoldTimer >= holdDurationToQuit)
            {
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            resetHoldTimer += Time.unscaledDeltaTime;

            if (resetHoldTimer >= holdDurationToReset)
            {
                ResetSimulation();
            }
        }
        else
        {
            resetHoldTimer = 0f;
        }

        if (Input.GetKey(KeyCode.Comma) || Input.GetKey(KeyCode.Period))
        {
            ChangeSpeed(Input.GetKey(KeyCode.Comma), Input.GetKey(KeyCode.Period));
        }
    }

    void TogglePause()
    {
        if (isPaused)
        {
            Time.timeScale = originalTimeScale;
            isPaused = false;
        }
        else
        {
            originalTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            isPaused = true;
        }
    }
    void ResetSimulation()
    {
        Time.timeScale = originalTimeScale;
        isPaused = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    void ChangeSpeed(bool decrease, bool increase)
    {
        if (decrease)
        {
            Time.timeScale = Mathf.Max(0f, Time.timeScale - speedChangeRate * Time.unscaledDeltaTime);
        }
        else if (increase)
        {
            Time.timeScale = Mathf.Min(10f, Time.timeScale + speedChangeRate * Time.unscaledDeltaTime);
        }
    }
}
