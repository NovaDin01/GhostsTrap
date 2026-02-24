using UnityEngine;

public class SurvivalLeaderboard : MonoBehaviour
{
    private const string BestSurvivalKey = "best_survival_time_seconds";

    public int BestSurvivalSeconds { get; private set; }

    private void Awake()
    {
        BestSurvivalSeconds = PlayerPrefs.GetInt(BestSurvivalKey, 0);
    }

    public bool TryUpdateRecord(float runDuration)
    {
        int survivedSeconds = Mathf.Max(0, Mathf.RoundToInt(runDuration));
        if (survivedSeconds <= BestSurvivalSeconds)
        {
            return false;
        }

        BestSurvivalSeconds = survivedSeconds;
        PlayerPrefs.SetInt(BestSurvivalKey, BestSurvivalSeconds);
        PlayerPrefs.Save();

        if (YourGamesBridge.Instance != null)
        {
            YourGamesBridge.Instance.SubmitSurvivalScore(BestSurvivalSeconds);
        }

        return true;
    }
}
