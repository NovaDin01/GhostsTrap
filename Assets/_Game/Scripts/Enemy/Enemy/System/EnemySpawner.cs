using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Location
{
    Lab,
    forest,
    river
}

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;
    
    [SerializeField] private List<Enemy> _enemiesPrefabs;
    [SerializeField] private List<Transform> _scientistSpawnPoint;
    [SerializeField] private List<Transform> _soldierSpawnPoint;
    [SerializeField] private Player player;

    [Header("Количество врагов")]
    private int allCount;
    private int currentCount;

    [Header("Кулдаун спавна")]
    [SerializeField] private float spawnCooldown;

    private float spawnTime;

    [Header("Кулдаун Таймера")]
    private float levelUpCooldown;
    private int timerLvl;
    private float timerTime;
    
    private Location _location;

    [Header("Вероятность спавна")]
    private float rareScientist;
    private float rareSoilderBaton;
    private float rareSoilderRifle;

    private int index;

    public float TimerTime => timerTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }

        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  
        }
        
        // минимальные проверки, чтобы не падать
        if (_enemiesPrefabs == null || _enemiesPrefabs.Count < 3)
        {
            Debug.LogError("EnemySpawner: нужно минимум 3 префаба врагов (0 ученый, 1 дубинка, 2 автомат).");
            enabled = false;
            return;
        }

        if (_scientistSpawnPoint == null || _scientistSpawnPoint.Count == 0 ||
            _soldierSpawnPoint == null || _soldierSpawnPoint.Count == 0)
        {
            Debug.LogError("EnemySpawner: нет точек спавна (ученые/солдаты).");
            enabled = false;
            return;
        }

        timerLvl = 1;
        _location = Location.Lab;

        ApplyLevelSettings();
        timerTime = levelUpCooldown;
        spawnTime = spawnCooldown;
        //spawnTime = Random.Range(minSpawnCooldown, maxSpawnCooldown);
    }

    private void Update()
    {
        spawnTime -= Time.deltaTime;
        
        if(timerLvl < 4)
            timerTime -= Time.deltaTime;

        if (spawnTime <= 0 && currentCount < allCount)
        {
            spawnTime = spawnCooldown;
            //spawnTime = Random.Range(minSpawnCooldown, maxSpawnCooldown);
            Spawn();
        }

        // ап опасности по времени - как ты и хочешь
        if (timerTime <= 0)
        {
            timerLvl++;
            ApplyLevelSettings();
            timerTime = levelUpCooldown;
        }
    }

    private void Spawn()
    {
        index = PickEnemyIndexByChance();
        Transform spawnPoint = GetSpawnPointFor(index);
        Enemy prefab = _enemiesPrefabs[index];

        Enemy enemy = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        enemy.Init(player);

        currentCount++;

        enemy.OnCollectedEnemy += OnEnemyCollected;   // если надо награду/эффекты
        enemy.OnDespawned += OnEnemyDespawned;        
    }

    private void OnEnemyCollected(Enemy enemy)
    {
        // тут можно начислить награду/сделать визуал
        
    }

    private void OnEnemyDespawned(Enemy enemy)
    {
        enemy.OnCollectedEnemy -= OnEnemyCollected;
        enemy.OnDespawned -= OnEnemyDespawned;

        currentCount = Mathf.Max(0, currentCount - 1);
    }


    private Transform GetSpawnPointFor(int enemyIndex)
    {
        // 0 = scientist, 1/2 = soldiers
        if (enemyIndex == 0)
            return _scientistSpawnPoint[Random.Range(0, _scientistSpawnPoint.Count)];

        return _soldierSpawnPoint[Random.Range(0, _soldierSpawnPoint.Count)];
    }

    private void ApplyLevelSettings()
    {
        // КАП: после 4 уровня оставляем настройки 4-го (уровень может расти бесконечно)
        int lvl = Mathf.Min(timerLvl, 4);

        switch (lvl)
        {
            case 1:
                levelUpCooldown = 5;
                rareScientist = 100;
                rareSoilderBaton = 0;
                rareSoilderRifle = 0;
                allCount = 10;
                spawnCooldown = 3;
                break;

            case 2:
                levelUpCooldown = 5;
                rareScientist = 60;
                rareSoilderBaton = 40;
                rareSoilderRifle = 0;
                allCount = 15;
                spawnCooldown = 2;
                break;

            case 3:
                levelUpCooldown = 5;
                rareScientist = 55;
                rareSoilderBaton = 30;
                rareSoilderRifle = 15;
                allCount = 24;
                spawnCooldown = 1.5f;
                break;

            case 4:
                levelUpCooldown = 55;
                rareScientist = 50;
                rareSoilderBaton = 25;
                rareSoilderRifle = 25;
                allCount = 30;
                spawnCooldown = 1;
                break;
        }
    }

    private int PickEnemyIndexByChance()
    {
        float w0 = Mathf.Max(0f, rareScientist);
        float w1 = Mathf.Max(0f, rareSoilderBaton);
        float w2 = Mathf.Max(0f, rareSoilderRifle);

        float sum = w0 + w1 + w2;
        if (sum <= 0f) return 0;

        float roll = Random.Range(0f, sum);

        if (roll < w0) return 0;
        if (roll < w0 + w1) return 1;
        return 2;
    }
}
