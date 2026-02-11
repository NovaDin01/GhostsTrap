using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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
    public class TimerStageConfig
    {
        [Min(0.1f)] public float stageDuration = 10f;
        [Header("Кого спавним на этом этапе (если пусто, используются defaultEnemies)")]
        public List<EnemyEntry> enemies = new();
        [Min(0)] public int maxAlive = 10;
        [Header("Опциональная прибавка к maxAlive для этого этапа")]
        public bool useCustomMaxAliveIncrease;
        [Min(0)] public int customMaxAliveIncrease = 1;
        [Min(0f)] public float spawnCooldown = 2f;
    }

    public enum MaxAliveProgressionMode
    {
        PerStageValue,
        AdditivePerStage
    }

    [Header("Enemy settings")]
    [SerializeField] private List<EnemyEntry> defaultEnemies = new();

    [Header("Spawn points")]
    [SerializeField] private List<Transform> scientistSpawnPoints = new();
    [SerializeField] private List<Transform> soldierSpawnPoints = new();

    [Header("Player")]
    [SerializeField] private Player _player;
    [SerializeField] private Transform playerSpawnPoint;

    [Header("Default spawn settings")]
    [SerializeField, Min(0)] private int defaultMaxAlive = 10;
    [SerializeField, Min(0f)] private float defaultSpawnCooldown = 2f;

    [Header("Настройка роста maxAlive")]
    [SerializeField] private MaxAliveProgressionMode maxAliveProgressionMode = MaxAliveProgressionMode.AdditivePerStage;
    [SerializeField, Min(0)] private int maxAliveIncreasePerStage = 1;

    [Header("Циклические этапы таймера")]
    [FormerlySerializedAs("endlessStages")]
    [SerializeField] private List<TimerStageConfig> timerStages = new();

    public event Action<int> OnTimerLevelChanged;
    public event Action<float> OnTimerTick;
    public event Action<Enemy> OnEnemySpawned;

    public float ElapsedTime => _elapsedTime;
    public float RemainingTime => _elapsedTime;
    public int TimerLvl => _timerLvl;

    private float _elapsedTime;
    private float _spawnTimer;

    private int _timerLvl;
    private float _spawnCooldown;
    private int _maxAlive;
    private List<EnemyEntry> _stageEnemies;

    private readonly HashSet<Enemy> _aliveEnemies = new();

    private bool _spawningEnabled;
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

        if ((scientistSpawnPoints == null || scientistSpawnPoints.Count == 0) &&
            (soldierSpawnPoints == null || soldierSpawnPoints.Count == 0))
        {
            Debug.LogError("EnemySpawner: не назначены spawn points для Scientist и Soldier.");
            enabled = false;
            return;
        }

        if (defaultEnemies == null || defaultEnemies.Count == 0)
        {
            Debug.LogError("EnemySpawner: список defaultEnemies пуст.");
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
            ApplyLevelSettings(activeStageIndex);
            OnTimerLevelChanged?.Invoke(_timerLvl);
        }

        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer <= 0f && _aliveEnemies.Count < _maxAlive)
        {
            _spawnTimer = _spawnCooldown;
            Spawn();
        }
    }

    public void StartGame()
    {
        if (_hasStarted) return;
        _hasStarted = true;

        if (playerSpawnPoint != null)
        {
            _player.transform.position = playerSpawnPoint.position;
        }

        Enemy.SetFrozen(false);

        _spawningEnabled = true;
        _elapsedTime = 0f;
        _spawnTimer = 0f;

        int activeStageIndex = GetActiveStageIndex(0f);
        _timerLvl = activeStageIndex + 1;

        ApplyLevelSettings(activeStageIndex);
        OnTimerLevelChanged?.Invoke(_timerLvl);
    }

    private void ApplyLevelSettings(int stageIndex)
    {
        TimerStageConfig stage = GetStageConfig(stageIndex);
        if (stage != null)
        {
            _maxAlive = ResolveMaxAlive(stage, stageIndex);
            _spawnCooldown = Mathf.Max(0f, stage.spawnCooldown);
            _stageEnemies = stage.enemies != null && stage.enemies.Count > 0
                ? stage.enemies
                : defaultEnemies;
            return;
        }

        _maxAlive = Mathf.Max(0, defaultMaxAlive);
        _spawnCooldown = Mathf.Max(0f, defaultSpawnCooldown);
        _stageEnemies = defaultEnemies;
    }

    private int ResolveMaxAlive(TimerStageConfig stage, int stageIndex)
    {
        if (maxAliveProgressionMode == MaxAliveProgressionMode.PerStageValue)
        {
            return Mathf.Max(0, stage.maxAlive);
        }

        int resolvedMaxAlive = Mathf.Max(0, defaultMaxAlive);

        for (int i = 0; i <= stageIndex; i++)
        {
            TimerStageConfig currentStage = GetStageConfig(i);
            if (currentStage == null) continue;

            int stageIncrease = currentStage.useCustomMaxAliveIncrease
                ? currentStage.customMaxAliveIncrease
                : maxAliveIncreasePerStage;

            resolvedMaxAlive += Mathf.Max(0, stageIncrease);
        }

        return resolvedMaxAlive;
    }

    private void Spawn()
    {
        EnemyEntry entry = PickEnemyEntryByWeight(_stageEnemies ?? defaultEnemies);
        if (entry == null || entry.prefab == null) return;

        Transform sp = GetSpawnPointFor(entry.role);
        if (sp == null) return;

        Enemy enemy = Instantiate(entry.prefab, sp.position, sp.rotation);
        enemy.Init(_player);

        _aliveEnemies.Add(enemy);
        enemy.OnDespawned += OnEnemyDespawned;
        OnEnemySpawned?.Invoke(enemy);
    }

    private Transform GetSpawnPointFor(EnemyRole role)
    {
        List<Transform> points = role == EnemyRole.Scientist
            ? scientistSpawnPoints
            : soldierSpawnPoints;

        if (points == null || points.Count == 0)
        {
            Debug.LogError($"EnemySpawner: нет spawnPoints для роли {role}");
            return null;
        }

        return points[Random.Range(0, points.Count)];
    }

    private void OnEnemyDespawned(Enemy enemy)
    {
        enemy.OnDespawned -= OnEnemyDespawned;
        _aliveEnemies.Remove(enemy);
    }

    private int GetActiveStageIndex(float elapsedSeconds)
    {
        if (timerStages == null || timerStages.Count == 0)
        {
            return 0;
        }

        float cycleDuration = GetTotalCycleDuration();
        if (cycleDuration <= 0f)
        {
            return 0;
        }

        float localTime = elapsedSeconds % cycleDuration;
        float accumulated = 0f;

        for (int i = 0; i < timerStages.Count; i++)
        {
            float duration = GetSafeStageDuration(timerStages[i]);
            accumulated += duration;
            if (localTime < accumulated)
            {
                return i;
            }
        }

        return timerStages.Count - 1;
    }

    private TimerStageConfig GetStageConfig(int stageIndex)
    {
        if (timerStages == null || timerStages.Count == 0)
        {
            return null;
        }

        if (stageIndex < 0 || stageIndex >= timerStages.Count)
        {
            return null;
        }

        return timerStages[stageIndex];
    }

    private float GetTotalCycleDuration()
    {
        if (timerStages == null || timerStages.Count == 0)
        {
            return 0f;
        }

        float duration = 0f;
        for (int i = 0; i < timerStages.Count; i++)
        {
            duration += GetSafeStageDuration(timerStages[i]);
        }

        return duration;
    }

    private static float GetSafeStageDuration(TimerStageConfig stage)
    {
        if (stage == null) return 0.1f;
        return Mathf.Max(0.1f, stage.stageDuration);
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
