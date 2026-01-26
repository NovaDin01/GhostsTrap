using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private BarrelSpawner barrelSpawner;
    [SerializeField] private Player player;
    [SerializeField] private bool startOnAwake = true;

    private bool isGameRunning;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (startOnAwake)
        {
            StartGame();
        }
        else
        {
            SetGameplayActive(false);
        }
    }

    public void StartGame()
    {
        if (isGameRunning) return;
        isGameRunning = true;

        SetGameplayActive(true);

        if (enemySpawner != null)
        {
            enemySpawner.StartFirstLocation();
        }
    }

    private void SetGameplayActive(bool isActive)
    {
        if (player != null)
        {
            player.enabled = isActive;
        }

        if (barrelSpawner != null)
        {
            barrelSpawner.enabled = isActive;
        }

        if (enemySpawner != null)
        {
            enemySpawner.SetSpawningEnabled(isActive);
        }
    }
}
