// GameRunManager.cs (оставляю как у тебя; он и так триггерится по OnDied)
using System;
using UnityEngine;

public class GameRunManager : MonoBehaviour
{
    public static GameRunManager Instance;

    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private Player player;
    [SerializeField] private DefeatWindow defeatWindow;
    [SerializeField] private GameObject[] UIObjects;

    [Header("Optional title text")]
    [SerializeField] private string defeatTitle = "Поражение";
    [SerializeField] private AudioClip mainMusic;
    [SerializeField] private AudioClip endMusic;
    [SerializeField] private AudioSource audioSource;

    private readonly GameStats _stats = new();
    private bool _runEnded;
    private bool _playerSubscribed;
    private bool _spawnerSubscribed;
    private bool _moneySubscribed;
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
        if (enemySpawner == null) enemySpawner = EnemySpawner.Instance;
        if (player == null) player = Player.Instance;
        if (_moneySystem == null) _moneySystem = MoneySystem.Instance;
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
        _stats.StartRun();
    }

    private void HandleEnemySpawned(Enemy enemy)
    {
        _stats.RegisterEnemySpawned();
        if (enemy != null) enemy.OnCollectedEnemy += HandleEnemyCollected;
    }

    private void HandleEnemyCollected(Enemy enemy)
    {
        if (enemy != null) enemy.OnCollectedEnemy -= HandleEnemyCollected;
        _stats.RegisterEnemyCollected();
    }

    private void HandleMoneyAdded(int amount)
    {
        _stats.RegisterMoneyEarned(amount);
    }

    private void HandlePlayerDied()
    {
        if (_runEnded) return;
        _runEnded = true;
        EndRun(defeatTitle);
    }

    private void EndRun(string title)
    {
        _stats.StopRun();
        if (enemySpawner != null) enemySpawner.StopRun();

        Time.timeScale = 0f;

        if (defeatWindow != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(endMusic);
            defeatWindow.Show(title, _stats);
            foreach (var obj in UIObjects)
            {
                obj.SetActive(false);
            }
        }
    }
}
