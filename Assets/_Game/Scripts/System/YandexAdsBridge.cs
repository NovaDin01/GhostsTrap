using System.Runtime.InteropServices;
using UnityEngine;

public class YandexAdsBridge : MonoBehaviour
{
    public static YandexAdsBridge Instance;

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

    private void ShowFullscreenAd(string placement)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        ShowYandexFullscreenAd(placement);
#else
        Debug.Log($"[YandexAdsBridge] Request ad: {placement}");
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void ShowYandexFullscreenAd(string placement);
#endif
}
