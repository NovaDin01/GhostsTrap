using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum Location
{
    Lab,
    Office,
    MilitaryBase,
    Forest
}

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    public enum EnemyRole
    {
        Scientist,
        Soldier
    }

    [Serializable]
    public class EnemyEntry
    {
        public Enemy prefab;
        public EnemyRole role = EnemyRole.Soldier;
        [Min(0f)] public float weight = 1f;
    }

    [Serializable]
    public class EndlessStageConfig
    {
        [Min(0f)] public float startsAfterSeconds;
        [Header("Кого спавним после этой секунды")]
        public List<EnemyEntry> enemies = new();
        [Min(0)] public int maxAlive = 10;
        [Min(0f)] public float spawnCooldown = 2f;
    }

    [Serializable]
    public class LocationConfig
    {
        public Location location;

        [Header("Кого спавним в этой локации")]
        public List<EnemyEntry> enemies = new();

        [Header("Точки спавна")]
        public List<Transform> scientistSpawnPoints = new();
        public List<Transform> soldierSpawnPoints = new();

        [Header("Стартовая точка игрока")]
        public Transform playerSpawnPoint;

        [Header("Дефолтные настройки спавна (если нет подходящего этапа)")]
        public int defaultMaxAlive = 10;
        public float defaultSpawnCooldown = 2f;

        [Header("Настройки этапов бесконечного таймера")]
        [FormerlySerializedAs("timerStages")]
        public List<EndlessStageConfig> endlessStages = new();
    }

    [Header("Configs")]
    [SerializeField] private List<LocationConfig> _locations = new();

    [Header("Runtime refs")]
    [SerializeField] private Player _player;

    // events
    public event Action<int> OnTimerLevelChanged;
    public event Action<float> OnTimerTick;
    public event Action<Location> OnLocationStarted;
    public event Action<Enemy> OnEnemySpawned;

    public float ElapsedTime => _elapsedTime;
    public float RemainingTime => _elapsedTime;
    public int TimerLvl => _timerLvl;
    public Location CurrentLocation => _currentLocation;

    private Location _currentLocation;
    private LocationConfig _currentConfig;

    private float _elapsedTime;
    private float _spawnTimer;

    private int _timerLvl;
    private int _currentCount;
    private int _allCount;
    private float _spawnCooldown;
    private List<EnemyEntry> _stageEnemies;

    private readonly HashSet<Enemy> _aliveEnemies = new();

    private bool _spawningEnabled = true;
    private bool _hasStarted;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (_player == null)
        {
            Debug.LogError("EnemySpawner: Player не назначен в инспекторе.");
            enabled = false;
            return;
        }

        if (_locations == null || _locations.Count == 0)
        {
            Debug.LogError("EnemySpawner: нет конфигов локаций.");
            enabled = false;
            return;
        }

        _spawningEnabled = false;
    }

    private void Update()
    {
        if (!_spawningEnabled) return;

        _elapsedTime += Time.deltaTime;
        OnTimerTick?.Invoke(_elapsedTime);

        int activeStageIndex = GetActiveStageIndex(_elapsedTime);
        int computedLvl = activeStageIndex + 1;
        if (computedLvl != _timerLvl)
        {
            _timerLvl = computedLvl;
            ApplyLevelSettings(_timerLvl);
            OnTimerLevelChanged?.Invoke(_timerLvl);
        }

        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer <= 0f && _currentCount < _allCount)
        {
            _spawnTimer = _spawnCooldown;
            Spawn();
        }
    }

    public void StartGame()
    {
        if (_hasStarted) return;
        _hasStarted = true;
        StartLocation(_locations[0].location);
    }

    public void StartLocation(Location location)
    {
        _currentLocation = location;
        _currentConfig = GetConfig(location);

        if (_currentConfig == null)
        {
            Debug.LogError($"EnemySpawner: не найден конфиг для локации {location}");
            enabled = false;
            return;
        }

        if ((_currentConfig.scientistSpawnPoints == null || _currentConfig.scientistSpawnPoints.Count == 0) &&
            (_currentConfig.soldierSpawnPoints == null || _currentConfig.soldierSpawnPoints.Count == 0))
        {
            Debug.LogError($"EnemySpawner: у локации {location} нет ни scientistSpawnPoints, ни soldierSpawnPoints.");
            enabled = false;
            return;
        }

        if (_currentConfig.enemies == null || _currentConfig.enemies.Count == 0)
        {
            Debug.LogError($"EnemySpawner: у локации {location} нет enemies.");
            enabled = false;
            return;
        }

        if (_currentConfig.playerSpawnPoint != null)
        {
            _player.transform.position = _currentConfig.playerSpawnPoint.position;
        }

        Enemy.SetFrozen(false);

        _spawningEnabled = true;
        _elapsedTime = 0f;
        _spawnTimer = 0f;

        _timerLvl = GetActiveStageIndex(0f) + 1;
        _currentCount = 0;

        ApplyLevelSettings(_timerLvl);
        OnTimerLevelChanged?.Invoke(_timerLvl);
        OnLocationStarted?.Invoke(_currentLocation);
    }

    private void ApplyLevelSettings(int lvl)
    {
        int idx = lvl - 1;

        EndlessStageConfig stage = GetStageConfig(idx);
        if (stage != null)
        {
            _allCount = Mathf.Max(0, stage.maxAlive);
            _spawnCooldown = Mathf.Max(0f, stage.spawnCooldown);
            _stageEnemies = stage.enemies != null && stage.enemies.Count > 0
                ? stage.enemies
                : _currentConfig.enemies;
            return;
        }

        _allCount = Mathf.Max(0, _currentConfig.defaultMaxAlive);
        _spawnCooldown = Mathf.Max(0f, _currentConfig.defaultSpawnCooldown);
        _stageEnemies = _currentConfig.enemies;
    }

    private void Spawn()
    {
        EnemyEntry entry = PickEnemyEntryByWeight(_stageEnemies ?? _currentConfig.enemies);
        if (entry == null || entry.prefab == null) return;

        Transform sp = GetSpawnPointFor(entry.role);
        if (sp == null) return;

        Enemy enemy = Instantiate(entry.prefab, sp.position, sp.rotation);
        enemy.Init(_player);

        _aliveEnemies.Add(enemy);
        _currentCount++;
        enemy.OnDespawned += OnEnemyDespawned;
        OnEnemySpawned?.Invoke(enemy);
    }

    private Transform GetSpawnPointFor(EnemyRole role)
    {
        List<Transform> points = (role == EnemyRole.Scientist)
            ? _currentConfig.scientistSpawnPoints
            : _currentConfig.soldierSpawnPoints;

        if (points == null || points.Count == 0)
        {
            Debug.LogError($"EnemySpawner: нет spawnPoints для роли {role} в локации {_currentLocation}");
            return null;
        }

        return points[Random.Range(0, points.Count)];
    }

    private void OnEnemyDespawned(Enemy enemy)
    {
        enemy.OnDespawned -= OnEnemyDespawned;
        _aliveEnemies.Remove(enemy);
        _currentCount = Mathf.Max(0, _currentCount - 1);
    }

    private LocationConfig GetConfig(Location loc)
    {
        for (int i = 0; i < _locations.Count; i++)
            if (_locations[i].location == loc)
                return _locations[i];

        return null;
    }

    private int GetActiveStageIndex(float elapsedSeconds)
    {
        if (_currentConfig == null || _currentConfig.endlessStages == null || _currentConfig.endlessStages.Count == 0)
        {
            return 0;
        }

        int bestIndex = -1;
        float bestTime = float.MinValue;

        for (int i = 0; i < _currentConfig.endlessStages.Count; i++)
        {
            EndlessStageConfig stage = _currentConfig.endlessStages[i];
            if (stage == null) continue;

            float stageStart = Mathf.Max(0f, stage.startsAfterSeconds);
            if (stageStart <= elapsedSeconds && stageStart >= bestTime)
            {
                bestTime = stageStart;
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    private EndlessStageConfig GetStageConfig(int stageIndex)
    {
        if (_currentConfig == null || _currentConfig.endlessStages == null || _currentConfig.endlessStages.Count == 0)
        {
            return null;
        }

        if (stageIndex < 0) return null;
        if (stageIndex >= _currentConfig.endlessStages.Count)
        {
            stageIndex = _currentConfig.endlessStages.Count - 1;
        }

        return _currentConfig.endlessStages[stageIndex];
    }

    public void StopRun()
    {
        _spawningEnabled = false;
        Enemy.SetFrozen(true);
    }

    private static EnemyEntry PickEnemyEntryByWeight(List<EnemyEntry> entries)
    {
        if (entries == null || entries.Count == 0) return null;

        float sum = 0f;
        for (int i = 0; i < entries.Count; i++)
            sum += Mathf.Max(0f, entries[i].weight);

        if (sum <= 0f) return entries[0];

        float roll = Random.Range(0f, sum);
        float acc = 0f;

        for (int i = 0; i < entries.Count; i++)
        {
            acc += Mathf.Max(0f, entries[i].weight);
            if (roll <= acc) return entries[i];
        }

        return entries[entries.Count - 1];
    }
}
