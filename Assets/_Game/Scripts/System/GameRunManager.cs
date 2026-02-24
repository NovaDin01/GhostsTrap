using System;
using UnityEngine;

public class GameRunManager : MonoBehaviour
{
    public static GameRunManager Instance;

    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private Player player;
    [SerializeField] private DefeatWindow defeatWindow;
    [SerializeField] private ReviveOfferWindow reviveOfferWindow;
    [SerializeField] private RunLeaderboard runLeaderboard;

    [Header("Optional title text")]
    [SerializeField] private string defeatTitle = "Поражение";
    [SerializeField] private string winTitle = "Забег завершен";

    [SerializeField] private AudioClip mainMusic;
    [SerializeField] private AudioClip endMusic;
    [SerializeField] private AudioSource audioSource;

    private readonly GameStats _stats = new();
    private bool _runEnded;
    private bool _playerSubscribed;
    private bool _spawnerSubscribed;
    private bool _moneySubscribed;
    private bool _reviveUsed;
    private MoneySystem _moneySystem;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ResolveDependencies();
        StartRun();
    }

    private void OnEnable()
    {
        ResolveDependencies();
        TrySubscribe();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void ResolveDependencies()
    {
        if (enemySpawner == null)
        {
            enemySpawner = EnemySpawner.Instance;
        }

        if (player == null)
        {
            player = Player.Instance;
        }

        if (_moneySystem == null)
        {
            _moneySystem = MoneySystem.Instance;
        }

        if (reviveOfferWindow == null)
        {
            reviveOfferWindow = FindObjectOfType<ReviveOfferWindow>();
            if (reviveOfferWindow == null)
            {
                var go = new GameObject(nameof(ReviveOfferWindow));
                reviveOfferWindow = go.AddComponent<ReviveOfferWindow>();
            }
        }

        if (runLeaderboard == null)
        {
            runLeaderboard = FindObjectOfType<RunLeaderboard>();
            if (runLeaderboard == null)
            {
                var go = new GameObject(nameof(RunLeaderboard));
                runLeaderboard = go.AddComponent<RunLeaderboard>();
            }
        }
    }

    private void Update()
    {
        if (!_playerSubscribed || !_spawnerSubscribed)
        {
            ResolveDependencies();
            TrySubscribe();
        }
    }

    private void TrySubscribe()
    {
        if (player != null && !_playerSubscribed)
        {
            player.OnDied += HandlePlayerDied;
            _playerSubscribed = true;
        }

        if (enemySpawner != null && !_spawnerSubscribed)
        {
            enemySpawner.OnEnemySpawned += HandleEnemySpawned;
            enemySpawner.OnLocationsCompleted += HandleLocationCompleted;
            enemySpawner.OnRunCompleted += HandleRunCompleted;
            _spawnerSubscribed = true;
        }

        if (_moneySystem != null && !_moneySubscribed)
        {
            _moneySystem.OnMoneyAdded += HandleMoneyAdded;
            _moneySubscribed = true;
        }
    }

    private void Unsubscribe()
    {
        if (player != null && _playerSubscribed)
        {
            player.OnDied -= HandlePlayerDied;
            _playerSubscribed = false;
        }

        if (enemySpawner != null && _spawnerSubscribed)
        {
            enemySpawner.OnEnemySpawned -= HandleEnemySpawned;
            enemySpawner.OnLocationsCompleted -= HandleLocationCompleted;
            enemySpawner.OnRunCompleted -= HandleRunCompleted;
            _spawnerSubscribed = false;
        }

        if (_moneySystem != null && _moneySubscribed)
        {
            _moneySystem.OnMoneyAdded -= HandleMoneyAdded;
            _moneySubscribed = false;
        }
    }

    private void StartRun()
    {
        _runEnded = false;
        _reviveUsed = false;
        _stats.StartRun();
    }

    private void HandleEnemySpawned(Enemy enemy)
    {
        _stats.RegisterEnemySpawned();
        if (enemy != null)
        {
            enemy.OnCollectedEnemy += HandleEnemyCollected;
        }
    }

    private void HandleEnemyCollected(Enemy enemy)
    {
        if (enemy != null)
        {
            enemy.OnCollectedEnemy -= HandleEnemyCollected;
        }

        _stats.RegisterEnemyCollected();
    }

    private void HandleLocationCompleted(int completed)
    {
        _stats.RegisterLocationCompleted();
    }

    private void HandleMoneyAdded(int amount)
    {
        _stats.RegisterMoneyEarned(amount);
    }

    private void HandleRunCompleted()
    {
        if (_runEnded) return;
        _runEnded = true;
        EndRun(winTitle, false);
    }

    private void HandlePlayerDied()
    {
        if (_runEnded) return;

        if (!_reviveUsed)
        {
            _reviveUsed = true;
            ShowReviveOffer();
            return;
        }

        _runEnded = true;
        EndRun(defeatTitle, false);
    }

    private void ShowReviveOffer()
    {
        Time.timeScale = 0f;
        reviveOfferWindow.Show(HandleWatchAdSelected, HandleReviveCancelled);
    }

    private void HandleWatchAdSelected()
    {
        if (YandexAdsBridge.Instance == null)
        {
            HandleReviveCancelled();
            return;
        }

        YandexAdsBridge.Instance.ShowRewardedAd(HandleReviveRewardReceived, HandleReviveCancelled);
    }

    private void HandleReviveRewardReceived()
    {
        reviveOfferWindow.Hide();
        Time.timeScale = 1f;
        if (player != null)
        {
            player.ReviveWithFullHp();
        }
    }

    private void HandleReviveCancelled()
    {
        reviveOfferWindow.Hide();
        _runEnded = true;
        EndRun(defeatTitle, false);
    }

    private void EndRun(string title, bool showDefeatAd)
    {
        _stats.StopRun();
        if (enemySpawner != null)
        {
            enemySpawner.StopRun();
        }

        runLeaderboard?.TryUpdateRecord(_stats.RunDuration);

        if (showDefeatAd && YandexAdsBridge.Instance != null)
        {
            YandexAdsBridge.Instance.ShowDefeatAd();
        }

        Time.timeScale = 0f;
        if (defeatWindow != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(endMusic);
            defeatWindow.Show(title, _stats);
        }
    }
}
