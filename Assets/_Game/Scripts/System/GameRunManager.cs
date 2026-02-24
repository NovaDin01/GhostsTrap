using UnityEngine;

public class GameRunManager : MonoBehaviour
{
    public static GameRunManager Instance;

    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private Player player;
    [SerializeField] private DefeatWindow defeatWindow;
    [SerializeField] private ReviveOfferWindow reviveOfferWindow;
    [SerializeField] private SurvivalLeaderboard survivalLeaderboard;

    [Header("Optional title text")]
    [SerializeField] private string defeatTitle = "Поражение";
    [SerializeField] private string winTitle = "Забег завершен";

    [SerializeField] private AudioClip mainMusic;
    [SerializeField] private AudioClip endMusic;
    [SerializeField] private AudioSource audioSource;

    private readonly GameStats _stats = new();
    private bool _runEnded;
    private bool _isAwaitingReviveChoice;
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
        _isAwaitingReviveChoice = false;
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
        EndRun(winTitle);
    }

    private void HandlePlayerDied()
    {
        if (_runEnded || _isAwaitingReviveChoice)
        {
            return;
        }

        _isAwaitingReviveChoice = true;
        Time.timeScale = 0f;

        if (reviveOfferWindow != null)
        {
            reviveOfferWindow.Show(OnWatchAdForRevive, OnCancelRevive);
            return;
        }

        OnCancelRevive();
    }

    private void OnWatchAdForRevive()
    {
        if (YourGamesBridge.Instance == null)
        {
            OnCancelRevive();
            return;
        }

        YourGamesBridge.Instance.ShowRewardedAd(OnRewardedReviveCompleted);
    }

    private void OnRewardedReviveCompleted(bool success)
    {
        if (!success)
        {
            OnCancelRevive();
            return;
        }

        if (reviveOfferWindow != null)
        {
            reviveOfferWindow.Hide();
        }

        _isAwaitingReviveChoice = false;

        if (player != null)
        {
            player.ReviveFullHp();
        }

        Time.timeScale = 1f;
    }

    private void OnCancelRevive()
    {
        if (reviveOfferWindow != null)
        {
            reviveOfferWindow.Hide();
        }

        if (_runEnded)
        {
            return;
        }

        _runEnded = true;
        _isAwaitingReviveChoice = false;
        EndRun(defeatTitle);
    }

    private void EndRun(string title)
    {
        _stats.StopRun();

        if (enemySpawner != null)
        {
            enemySpawner.StopRun();
        }

        int bestTimeSeconds = -1;
        if (survivalLeaderboard != null)
        {
            survivalLeaderboard.TryUpdateRecord(_stats.RunDuration);
            bestTimeSeconds = survivalLeaderboard.BestSurvivalSeconds;
        }

        Time.timeScale = 0f;
        if (defeatWindow != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(endMusic);
            defeatWindow.Show(title, _stats, bestTimeSeconds);
        }
    }
}
