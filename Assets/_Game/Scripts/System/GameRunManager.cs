using UnityEngine;

public class GameRunManager : MonoBehaviour
{
    public static GameRunManager Instance;

    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private Player player;
    [SerializeField] private DefeatWindow defeatWindow;

    [Header("Optional title text")]
    [SerializeField] private string defeatTitle = "Поражение";
    [SerializeField] private string winTitle = "Забег завершен";

    private readonly GameStats _stats = new();
    private bool _runEnded;

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
        Subscribe();
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
    }

    private void Subscribe()
    {
        if (player != null)
        {
            player.OnDied += HandlePlayerDied;
        }

        if (enemySpawner != null)
        {
            enemySpawner.OnEnemySpawned += HandleEnemySpawned;
            enemySpawner.OnLocationsCompleted += HandleLocationCompleted;
            enemySpawner.OnRunCompleted += HandleRunCompleted;
        }
    }

    private void Unsubscribe()
    {
        if (player != null)
        {
            player.OnDied -= HandlePlayerDied;
        }

        if (enemySpawner != null)
        {
            enemySpawner.OnEnemySpawned -= HandleEnemySpawned;
            enemySpawner.OnLocationsCompleted -= HandleLocationCompleted;
            enemySpawner.OnRunCompleted -= HandleRunCompleted;
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

    private void HandleRunCompleted()
    {
        if (_runEnded) return;
        _runEnded = true;
        EndRun(winTitle);
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
        if (enemySpawner != null)
        {
            enemySpawner.StopRun();
        }

        Time.timeScale = 0f;
        if (defeatWindow != null)
        {
            defeatWindow.Show(title, _stats);
        }
    }
}
