using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private TMP_Text timer;
    [SerializeField] private TMP_Text money;

    private void Update()
    {
        UpdateTimer();
        UpdateMoney();
    }

    private void UpdateTimer()
    {
        float time = Mathf.Max(0f, EnemySpawner.Instance.ElapsedTime);

        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);

        timer.text = $"{minutes:0}:{seconds:00}";
    }

    private void UpdateMoney()
    {
        money.text = MoneySystem.Instance.Wallet.ToString("N0");
    }
}