using System;
using System.Reflection;
using UnityEngine;

public class RunLeaderboard : MonoBehaviour
{
    private const string LocalRecordKey = "best_run_time_sec";
    private const string LeaderboardName = "best_survival_time";

    public static RunLeaderboard Instance;

    public float BestRunSeconds { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        BestRunSeconds = PlayerPrefs.GetFloat(LocalRecordKey, 0f);
    }

    public bool TryUpdateRecord(float runDuration)
    {
        if (runDuration <= BestRunSeconds)
        {
            return false;
        }

        BestRunSeconds = runDuration;
        PlayerPrefs.SetFloat(LocalRecordKey, BestRunSeconds);
        PlayerPrefs.Save();
        SubmitToYourGamesLeaderboard(Mathf.RoundToInt(BestRunSeconds));
        return true;
    }

    private void SubmitToYourGamesLeaderboard(int seconds)
    {
        // Поддержка нескольких версий API плагина Your Games через reflection.
        if (TryCall("YG.YandexGame", "NewLeaderboardScores", LeaderboardName, seconds)) return;
        if (TryCall("YG.YandexGame", "SetLeaderboard", LeaderboardName, seconds)) return;
        if (TryCall("YandexGame", "NewLeaderboardScores", LeaderboardName, seconds)) return;
        if (TryCall("YandexGame", "SetLeaderboard", LeaderboardName, seconds)) return;

        Debug.Log($"[RunLeaderboard] Your Games leaderboard API not found. Local record: {seconds}s");
    }

    private bool TryCall(string typeName, string methodName, string leaderboard, int score)
    {
        var type = Type.GetType(typeName);
        if (type == null) return false;

        var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
        if (method == null) return false;

        method.Invoke(null, new object[] { leaderboard, score });
        return true;
    }
}
