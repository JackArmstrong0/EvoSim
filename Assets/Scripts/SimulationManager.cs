using Microsoft.Unity.VisualStudio.Editor;
using NUnit.Framework;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

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

    public Image runningSign;
    public Image pausedSign;
    public TextMeshProUGUI simSpeedText;
    public TextMeshProUGUI statusText;
    void Start()
    {
        simSpeedText.text = Time.timeScale.ToString("0.0") + "x";
        statusText.text = "RUNNING";
        statusText.color = Color.green;
        pausedSign.enabled = false;
        runningSign.enabled = true;
    }
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
            simSpeedText.text = Time.timeScale.ToString("0.0") + "x";
        }
    }

    void TogglePause()
    {
        if (isPaused)
        {
            Time.timeScale = originalTimeScale;
            isPaused = false;
            runningSign.enabled = true;
            pausedSign.enabled = false;
            statusText.text = "RUNNING";
            statusText.color = Color.green;
        }
        else
        {
            originalTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            isPaused = true;
            runningSign.enabled = false;
            pausedSign.enabled = true;
            statusText.text = "PAUSED";
            statusText.color = Color.red;
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
