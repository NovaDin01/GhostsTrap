using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class YandexAdsBridge : MonoBehaviour
{
    public static YandexAdsBridge Instance;

    private Action _rewardedCallback;
    private Action _rewardedFailedCallback;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureInstance()
    {
        if (Instance != null) return;

        var existing = FindObjectOfType<YandexAdsBridge>();
        if (existing != null)
        {
            Instance = existing;
            return;
        }

        var go = new GameObject(nameof(YandexAdsBridge));
        Instance = go.AddComponent<YandexAdsBridge>();
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

    public void ShowStartupAd()
    {
        ShowFullscreenAd("start");
    }

    public void ShowDefeatAd()
    {
        ShowFullscreenAd("defeat");
    }

    public void ShowRewardedAd(Action onRewarded, Action onFailed)
    {
        _rewardedCallback = onRewarded;
        _rewardedFailedCallback = onFailed;

#if UNITY_WEBGL && !UNITY_EDITOR
        Yandex_ShowRewardedAd("revive");
#else
        Debug.Log("[YandexAdsBridge] Rewarded ad simulated in editor");
        _rewardedCallback?.Invoke();
        ClearRewardedCallbacks();
#endif
    }

    public void OnRewardedAdSuccess()
    {
        _rewardedCallback?.Invoke();
        ClearRewardedCallbacks();
    }

    public void OnRewardedAdFailed()
    {
        _rewardedFailedCallback?.Invoke();
        ClearRewardedCallbacks();
    }

    private void ClearRewardedCallbacks()
    {
        _rewardedCallback = null;
        _rewardedFailedCallback = null;
    }

    private void ShowFullscreenAd(string placement)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Yandex_ShowFullscreenAd(placement);
#else
        Debug.Log($"[YandexAdsBridge] Request ad: {placement}");
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void Yandex_ShowFullscreenAd(string placement);

    [DllImport("__Internal")]
    private static extern void Yandex_ShowRewardedAd(string placement);
#endif
}
