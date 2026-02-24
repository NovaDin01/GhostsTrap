using UnityEngine;
using YG;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    private bool _adShownOnStart = false;      // Реклама при старте игры
    private bool _reviveUsed = false;          // Воскрешение за просмотр рекламы

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

    /// <summary>
    /// Воскрешение игрока через просмотр рекламы
    /// </summary>
    public void TryRevivePlayer(System.Action onRevive)
    {
        if (_reviveUsed)
        {
            Debug.Log("Воскрешение за рекламу уже использовано!");
            return;
        }

        if (YG2.isSDKEnabled)
        {
            Debug.Log("Показываем рекламу для воскрешения игрока...");
            YG2.InterstitialAdvShow();

            // Считаем, что игрок "воскрес" после показа рекламы
            _reviveUsed = true;
            onRevive?.Invoke();
        }
        else
        {
            Debug.LogWarning("SDK не готов, нельзя воскресить игрока через рекламу");
        }
    }
}