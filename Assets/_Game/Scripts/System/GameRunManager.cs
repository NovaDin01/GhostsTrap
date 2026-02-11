// GameRunManager.cs (оставляю как у тебя; он и так триггерится по OnDied)
using System;
using UnityEngine;
using UnityEngine.UI;
#if PLUGIN_YG_2
using YG;
#endif

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

    [Header("Revive ad")]
    [SerializeField] private GameObject reviveOfferWindow;
    [SerializeField] private Button reviveAcceptButton;
    [SerializeField] private Button reviveDeclineButton;
    [SerializeField] private int reviveHpAfterAd = 3;
    [SerializeField] private string reviveRewardAdId = "revive";

    private readonly GameStats _stats = new();
    private bool _runEnded;
    private bool _playerSubscribed;
    private bool _spawnerSubscribed;
    private bool _moneySubscribed;
    private bool _reviveAdUsed;
    private bool _waitingReviveAdResult;
    private bool _reviveRewardGranted;
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

        if (reviveAcceptButton != null) reviveAcceptButton.onClick.AddListener(OnReviveAccepted);
        if (reviveDeclineButton != null) reviveDeclineButton.onClick.AddListener(OnReviveDeclined);

        HideReviveOfferWindow();
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
#if PLUGIN_YG_2
        YG2.onCloseRewardedAdv += HandleReviveAdClosed;
        YG2.onErrorRewardedAdv += HandleReviveAdError;
#endif
    }

    private void OnDisable()
    {
        Unsubscribe();
#if PLUGIN_YG_2
        YG2.onCloseRewardedAdv -= HandleReviveAdClosed;
        YG2.onErrorRewardedAdv -= HandleReviveAdError;
#endif
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

        if (CanShowReviveOffer())
        {
            ShowReviveOffer();
            return;
        }

        _runEnded = true;
        EndRun(defeatTitle);
    }

    private bool CanShowReviveOffer()
    {
        return !_reviveAdUsed && reviveOfferWindow != null;
    }

    private void ShowReviveOffer()
    {
        Time.timeScale = 0f;
        reviveOfferWindow.SetActive(true);
    }

    private void HideReviveOfferWindow()
    {
        if (reviveOfferWindow != null)
        {
            reviveOfferWindow.SetActive(false);
        }
    }

    private void OnReviveAccepted()
    {
        if (_runEnded || _reviveAdUsed) return;

        _reviveAdUsed = true;
        _waitingReviveAdResult = true;
        _reviveRewardGranted = false;
        HideReviveOfferWindow();

#if PLUGIN_YG_2
        YG2.RewardedAdvShow(reviveRewardAdId, GrantReviveReward);
#else
        GrantReviveReward();
        FinalizeReviveFlow();
#endif
    }

    private void OnReviveDeclined()
    {
        if (_runEnded) return;

        HideReviveOfferWindow();
        _runEnded = true;
        EndRun(defeatTitle);
    }

    private void GrantReviveReward()
    {
        _reviveRewardGranted = true;
    }

    private void HandleReviveAdClosed()
    {
        if (!_waitingReviveAdResult) return;
        FinalizeReviveFlow();
    }

    private void HandleReviveAdError()
    {
        if (!_waitingReviveAdResult) return;
        _reviveRewardGranted = false;
        FinalizeReviveFlow();
    }

    private void FinalizeReviveFlow()
    {
        _waitingReviveAdResult = false;

        if (_reviveRewardGranted)
        {
            RevivePlayer();
            return;
        }

        _runEnded = true;
        EndRun(defeatTitle);
    }

    private void RevivePlayer()
    {
        if (player != null)
        {
            player.Revive(reviveHpAfterAd);
        }

        Time.timeScale = 1f;
    }

    private void EndRun(string title)
    {
        _stats.StopRun();
        if (enemySpawner != null) enemySpawner.StopRun();

        Time.timeScale = 0f;

        if (defeatWindow != null)
        {
            if (audioSource != null)
            {
                audioSource.Stop();
                if (endMusic != null) audioSource.PlayOneShot(endMusic);
            }

            defeatWindow.Show(title, _stats);
            foreach (var obj in UIObjects)
            {
                if (obj != null) obj.SetActive(false);
            }
        }
    }
}
