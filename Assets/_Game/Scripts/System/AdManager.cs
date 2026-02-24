using UnityEngine;
using YG;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    private bool _adShownOnStart = false;      // Реклама при старте игры
    private bool _reviveUsed = false;          // Воскрешение за просмотр рекламы

    [Header("Rewarded Ads")]
    [SerializeField] private string reviveRewardId = "revive_player";

    private void Awake()
    {
        // Singleton, чтобы один объект был на всю игру
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
        ShowAdOnStartOnce();
    }

    /// <summary>
    /// Показ рекламы один раз при старте игры
    /// </summary>
    private void ShowAdOnStartOnce()
    {
        if (_adShownOnStart) return;

        if (YG2.isSDKEnabled)
        {
            Debug.Log("SDK готов, показываем стартовую рекламу...");
            YG2.InterstitialAdvShow();
            _adShownOnStart = true;
        }
        else
        {
            Debug.LogWarning("SDK еще не готов, стартовая реклама не показана");
        }
    }

    public void ResetRunState()
    {
        _reviveUsed = false;
    }

    /// <summary>
    /// Воскрешение игрока через просмотр rewarded-рекламы.
    /// </summary>
    public void TryRevivePlayer(System.Action onRevive, System.Action onFailed = null)
    {
        if (_reviveUsed)
        {
            Debug.Log("Воскрешение за рекламу уже использовано!");
            onFailed?.Invoke();
            return;
        }

        if (YG2.isSDKEnabled)
        {
            Debug.Log("Показываем rewarded-рекламу для воскрешения игрока...");
            YG2.RewardedAdvShow(reviveRewardId, () =>
            {
                _reviveUsed = true;
                onRevive?.Invoke();
            });
        }
        else
        {
            Debug.LogWarning("SDK не готов, нельзя воскресить игрока через рекламу");
            onFailed?.Invoke();
        }
    }
}
