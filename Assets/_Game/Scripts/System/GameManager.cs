using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Systems")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private List<MonoBehaviour> systemsToEnable = new();

    private bool _hasStarted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < systemsToEnable.Count; i++)
        {
            if (systemsToEnable[i] != null)
            {
                systemsToEnable[i].enabled = false;
            }
        }
    }

    public void StartGame()
    {
        if (_hasStarted) return;
        _hasStarted = true;

        for (int i = 0; i < systemsToEnable.Count; i++)
        {
            if (systemsToEnable[i] != null)
            {
                systemsToEnable[i].enabled = true;
            }
        }

        var spawner = enemySpawner != null ? enemySpawner : EnemySpawner.Instance;
        if (spawner != null)
        {
            spawner.StartGame();
        }
    }
}