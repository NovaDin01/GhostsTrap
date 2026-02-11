using System.Collections.Generic;
using UnityEngine;
#if PLUGIN_YG_2
using YG;
#endif

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Systems")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private GameRunManager gameRunManager;
    [SerializeField] private List<MonoBehaviour> systemsToEnable = new();

    [Header("Startup ad")]
    [SerializeField] private bool showStartupAdBeforeRun = true;
    [SerializeField] private string startupRewardedAdId = "startup";

    private bool _hasStarted;
    private bool _startupAdShown;
    private bool _waitingStartupAd;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        EnsureGameRunManager();
        for (int i = 0; i < systemsToEnable.Count; i++)
        {
            if (systemsToEnable[i] != null)
            {
                systemsToEnable[i].enabled = false;
            }
        }
    }

    private void OnEnable()
    {
#if PLUGIN_YG_2
        YG2.onCloseRewardedAdv += HandleStartupAdClosed;
        YG2.onErrorRewardedAdv += HandleStartupAdError;
#endif
    }

    private void OnDisable()
    {
#if PLUGIN_YG_2
        YG2.onCloseRewardedAdv -= HandleStartupAdClosed;
        YG2.onErrorRewardedAdv -= HandleStartupAdError;
#endif
    }

    private void EnsureGameRunManager()
    {
        if (gameRunManager == null)
        {
            gameRunManager = FindObjectOfType<GameRunManager>();
        }

        if (gameRunManager == null)
        {
            var managerObject = new GameObject(nameof(GameRunManager));
            gameRunManager = managerObject.AddComponent<GameRunManager>();
            DontDestroyOnLoad(managerObject);
        }

        if (!systemsToEnable.Contains(gameRunManager))
        {
            systemsToEnable.Add(gameRunManager);
        }
    }

    public void StartGame()
    {
        if (_hasStarted || _waitingStartupAd) return;

        if (showStartupAdBeforeRun && !_startupAdShown)
        {
            _waitingStartupAd = true;
            _startupAdShown = true;

#if PLUGIN_YG_2
            YG2.RewardedAdvShow(startupRewardedAdId);
#else
            _waitingStartupAd = false;
            StartGameInternal();
#endif
            return;
        }

        StartGameInternal();
    }

    private void HandleStartupAdClosed()
    {
        if (!_waitingStartupAd) return;

        _waitingStartupAd = false;
        StartGameInternal();
    }

    private void HandleStartupAdError()
    {
        if (!_waitingStartupAd) return;

        _waitingStartupAd = false;
        StartGameInternal();
    }

    private void StartGameInternal()
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
