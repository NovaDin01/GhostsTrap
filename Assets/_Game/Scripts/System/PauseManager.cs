using System;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    public static bool IsPaused { get; private set; }

    [SerializeField] private GameObject pausePanel;

    private void Awake()
    {
        if(Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void PauseSwitch()
    {
        if(IsPaused) Resume();
        else Pause();
    }

    private void Pause()
    {
        Time.timeScale = 0f;
        IsPaused = true;
        pausePanel.SetActive(true);
    }

    private void Resume()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        pausePanel.SetActive(false);
    }
}