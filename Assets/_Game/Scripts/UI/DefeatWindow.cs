using TMPro;
using UnityEngine;

public class DefeatWindow : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text enemiesSpawnedText;
    [SerializeField] private TMP_Text enemiesCollectedText;
    [SerializeField] private TMP_Text locationsCompletedText;
    [SerializeField] private TMP_Text timeText;

    public void Show(string title, GameStats stats)
    {
        if (titleText != null)
        {
            titleText.text = title;
        }

        if (stats != null)
        {
            if (moneyText != null)
            {
                moneyText.text = MoneySystem.Instance != null
                    ? MoneySystem.Instance.TotalEarned.ToString("N0")
                    : "0";
            }

            if (enemiesSpawnedText != null)
            {
                enemiesSpawnedText.text = stats.EnemiesSpawned.ToString("N0");
            }

            if (enemiesCollectedText != null)
            {
                enemiesCollectedText.text = stats.EnemiesCollected.ToString("N0");
            }

            if (locationsCompletedText != null)
            {
                locationsCompletedText.text = stats.LocationsCompleted.ToString("N0");
            }

            if (timeText != null)
            {
                int minutes = Mathf.FloorToInt(stats.RunDuration / 60f);
                int seconds = Mathf.FloorToInt(stats.RunDuration % 60f);
                timeText.text = $"{minutes:0}:{seconds:00}";
            }
        }

        gameObject.SetActive(true);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneController.Instance.LoadGame();
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneController.Instance.LoadMenu();
    }
}
