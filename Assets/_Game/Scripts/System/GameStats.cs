using UnityEngine;

[System.Serializable]
public class GameStats
{
    public int MoneyEarned { get; private set; }
    public int EnemiesSpawned { get; private set; }
    public int EnemiesCollected { get; private set; }
    public int LocationsCompleted { get; private set; }
    public float RunDuration { get; private set; }

    private float _startTime;
    private bool _isRunning;

    public void StartRun()
    {
        _startTime = Time.time;
        _isRunning = true;
        MoneyEarned = 0;
        EnemiesSpawned = 0;
        EnemiesCollected = 0;
        LocationsCompleted = 0;
        RunDuration = 0f;
    }

    public void StopRun()
    {
        if (!_isRunning) return;
        RunDuration = Mathf.Max(0f, Time.time - _startTime);
        _isRunning = false;
    }

    public void RegisterEnemySpawned()
    {
        if (!_isRunning) return;
        EnemiesSpawned++;
    }

    public void RegisterEnemyCollected()
    {
        if (!_isRunning) return;
        EnemiesCollected++;
    }

    public void RegisterLocationCompleted()
    {
        if (!_isRunning) return;
        LocationsCompleted++;
    }

    public void RegisterMoneyEarned(int amount)
    {
        if (!_isRunning) return;
        MoneyEarned += amount;
    }
}
