using NUnit.Framework;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    public static bool isPaused = false;
    static float originalTimeScale = 1f;
    float speedChangeRate = 1f;

    float holdDurationToQuit = 2f;
    float holdTimer = 0f;
    bool isHolding = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isHolding = true;
            holdTimer = 0f;
        }

        if (isHolding)
        {
            holdTimer += Time.unscaledDeltaTime;

            if (holdTimer >= holdDurationToQuit)
            {
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (holdTimer < holdDurationToQuit)
            {
                TogglePause();
            }

            isHolding = false;
            holdTimer = 0f;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetSimulation();
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
