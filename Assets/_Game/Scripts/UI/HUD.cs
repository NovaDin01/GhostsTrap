using System;
using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private TMP_Text timer;
    [SerializeField] private TMP_Text money;

    private void Update()
    {
        timer.text = EnemySpawner.Instance.TimerTime.ToString("0:00");
        money.text = MoneySystem.Instance.Wallet.ToString("N0");
    }
}
