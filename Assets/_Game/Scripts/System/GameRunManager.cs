using UnityEngine;
using YG;

public class GameRunManager : MonoBehaviour
{
    public static GameRunManager Instance;

    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private Player player;
    [SerializeField] private DefeatWindow defeatWindow;
    [SerializeField] private ReviveOfferWindow reviveOfferWindow;
    [SerializeField] private GameObject[] UIObjects;

    [Header("Optional title text")]
    [SerializeField] private string defeatTitle = "Поражение";
    [SerializeField] private AudioClip mainMusic;
    [SerializeField] private AudioClip endMusic;
    [SerializeField] private AudioSource audioSource;

    [Header("Leaderboard")]
    [SerializeField] private string survivalLeaderboardName = "survival_time";
    [SerializeField] private string localBestKey = "BestSurvivalTimeSeconds";

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

        Time.timeScale = 1f;
        SetHudActive(true);

        if (reviveOfferWindow != null)
            reviveOfferWindow.Hide();

        if (AdManager.Instance != null)
            AdManager.Instance.ResetRunState();
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

        if (reviveOfferWindow != null && AdManager.Instance != null)
        {
            PauseForReviveChoice();
            reviveOfferWindow.Show(HandleWatchAdClicked, HandleReviveCancelled);
            return;
        }

        EndRun(defeatTitle);
    }

    private void PauseForReviveChoice()
    {
        if (enemySpawner != null) enemySpawner.StopRun();
        Time.timeScale = 0f;
    }

    private void HandleWatchAdClicked()
    {
        if (reviveOfferWindow != null)
            reviveOfferWindow.Hide();

        Time.timeScale = 1f;

        AdManager.Instance.TryRevivePlayer(RevivePlayerAfterAd, HandleReviveCancelled);
    }

    private void RevivePlayerAfterAd()
    {
        if (_runEnded) return;

        if (player != null)
            player.ReviveFullHp();

        if (enemySpawner != null)
            enemySpawner.ResumeRun();

        Time.timeScale = 1f;
        SetHudActive(true);

        if (audioSource != null && mainMusic != null && !audioSource.isPlaying)
            audioSource.PlayOneShot(mainMusic);
    }

    private void HandleReviveCancelled()
    {
        EndRun(defeatTitle);
    }

    private void EndRun(string title)
    {
        if (_runEnded) return;
        _runEnded = true;

        if (reviveOfferWindow != null)
            reviveOfferWindow.Hide();

        _stats.StopRun();
        TryUpdateLeaderboard();

        if (enemySpawner != null) enemySpawner.StopRun();

        Time.timeScale = 0f;

        if (defeatWindow != null)
        {
            if (audioSource != null)
            {
                audioSource.Stop();
                if (endMusic != null)
                    audioSource.PlayOneShot(endMusic);
            }

            defeatWindow.Show(title, _stats);
            SetHudActive(false);
        }
    }

    private void TryUpdateLeaderboard()
    {
        float currentRun = Mathf.Max(0f, _stats.RunDuration);
        float localBest = PlayerPrefs.GetFloat(localBestKey, 0f);

        if (currentRun <= localBest)
            return;

        PlayerPrefs.SetFloat(localBestKey, currentRun);
        PlayerPrefs.Save();

        if (YG2.isSDKEnabled && !string.IsNullOrEmpty(survivalLeaderboardName))
            YG2.SetLBTimeConvert(survivalLeaderboardName, currentRun);
    }

    private void SetHudActive(bool active)
    {
        if (UIObjects == null) return;

        for (int i = 0; i < UIObjects.Length; i++)
        {
            if (UIObjects[i] != null)
                UIObjects[i].SetActive(active);
        }
    }
}
