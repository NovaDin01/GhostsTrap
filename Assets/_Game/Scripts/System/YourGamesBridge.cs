using System;
using System.Reflection;
using UnityEngine;

public class YourGamesBridge : MonoBehaviour
{
    public static YourGamesBridge Instance { get; private set; }

    private const string RewardedPlacementId = "revive";

    private Action<bool> _rewardedCallback;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureInstance()
    {
        if (Instance != null) return;

        var existing = FindObjectOfType<YourGamesBridge>();
        if (existing != null)
        {
            Instance = existing;
            return;
        }

        var go = new GameObject(nameof(YourGamesBridge));
        Instance = go.AddComponent<YourGamesBridge>();
        DontDestroyOnLoad(go);
    }

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

    public void ShowRewardedAd(Action<bool> callback)
    {
        _rewardedCallback = callback;

#if UNITY_WEBGL && !UNITY_EDITOR
        if (!TryInvokeStatic("YG.YG2", "RewardedAdvShow", RewardedPlacementId) &&
            !TryInvokeStatic("YG2.YG2", "RewardedAdvShow", RewardedPlacementId) &&
            !TryInvokeStatic("YandexGame", "RewVideoShow", 0))
        {
            Debug.LogWarning("[YourGamesBridge] Не найден метод показа rewarded рекламы. Revive отклонен.");
            CompleteReward(false);
        }
#else
        Debug.Log("[YourGamesBridge] Rewarded ad simulation in editor/standalone.");
        CompleteReward(true);
#endif
    }

    public void SubmitSurvivalScore(int seconds)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (!TryInvokeStatic("YG.YG2", "SetLeaderboard", "survival_time", seconds) &&
            !TryInvokeStatic("YG2.YG2", "SetLeaderboard", "survival_time", seconds) &&
            !TryInvokeStatic("YandexGame", "NewLeaderboardScores", "survival_time", seconds))
        {
            Debug.LogWarning("[YourGamesBridge] Не удалось отправить результат в leaderboard плагина.");
        }
#else
        Debug.Log($"[YourGamesBridge] Leaderboard simulation: survival_time={seconds}");
#endif
    }

    // Методы можно вызывать из плагина через SendMessage.
    public void OnRewardedSuccess()
    {
        CompleteReward(true);
    }

    public void OnRewardedFailed()
    {
        CompleteReward(false);
    }

    private void CompleteReward(bool result)
    {
        var callback = _rewardedCallback;
        _rewardedCallback = null;
        callback?.Invoke(result);
    }

    private static bool TryInvokeStatic(string typeName, string methodName, params object[] args)
    {
        var type = Type.GetType(typeName);
        if (type == null)
        {
            return false;
        }

        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        MethodInfo method = null;
        foreach (var candidate in type.GetMethods(flags))
        {
            if (!string.Equals(candidate.Name, methodName, StringComparison.Ordinal))
            {
                continue;
            }

            var parameters = candidate.GetParameters();
            if (parameters.Length != args.Length)
            {
                continue;
            }

            method = candidate;
            break;
        }

        if (method == null)
        {
            return false;
        }

        method.Invoke(null, args);
        return true;
    }
}
