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
        [Header("Префаб врага")]
        public Enemy prefab;

        [Header("Роль (определяет точки спавна)")]
        public EnemyRole role = EnemyRole.Soldier;

        [Header("Базовый вес (шанс появления на старте)")]
        [Min(0f)] public float baseWeight = 1f;

        [Header("Теги спавна (НЕ влияют на характеристики)")]
        public bool isRanged;   // автоматчик
        public bool hasShield;  // щит
        public bool isElectric; // электрическая дубинка

        [Header("Рост появления по сложности")]
        [Tooltip("До этого уровня сложности враг не будет выбираться вообще (мягкий gate).")]
        [Min(0)] public int unlockDifficulty = 0;

        [Tooltip("Насколько растёт вес за каждый уровень сложности.")]
        [Min(0f)] public float weightGrowthPerLevel = 0f;
    }

    [Serializable]
    public class TimerStageConfig
    {
        [Header("Длительность этапа (сек)")]
        [Min(0.1f)] public float stageDuration = 10f;

        [Header("Множитель лимита живых врагов (ритм)")]
        [Min(0.1f)] public float maxAliveMultiplier = 1f;

        [Header("Множитель кулдауна спавна (меньше = чаще)")]
        [Min(0.1f)] public float cooldownMultiplier = 1f;

        [Header("Пул врагов на этапе (если пусто — используются враги по умолчанию)")]
        public List<EnemyEntry> enemiesOverride = new();

        [Header("Ограничения состава на этапе (-1 = без ограничений)")]
        [Tooltip("Сколько дальников (автоматчиков) можно держать одновременно живыми.")]
        public int maxRangedAlive = -1;

        [Tooltip("Сколько врагов со щитом можно держать одновременно живыми.")]
        public int maxShieldAlive = -1;
    }

    [Header("Настройки врагов (по умолчанию)")]
    [SerializeField] private List<EnemyEntry> defaultEnemies = new();

    [Header("Точки спавна")]
    [SerializeField] private List<Transform> scientistSpawnPoints = new();
    [SerializeField] private List<Transform> soldierSpawnPoints = new();

    [Header("Игрок")]
    [SerializeField] private Player player;
    [SerializeField] private Transform playerSpawnPoint;

    [Header("Глобальная сложность (не сбрасывается)")]
    [Tooltip("Каждые N секунд повышаем уровень сложности на 1.")]
    [SerializeField, Min(1f)] private float difficultyStepSeconds = 60f;

    [Tooltip("Базовый лимит живых врагов на сложности 0.")]
    [SerializeField, Min(0)] private int baseMaxAlive = 10;

    [Tooltip("Прибавка к лимиту за каждый уровень сложности.")]
    [SerializeField, Min(0)] private int maxAlivePerDifficulty = 1;

    [Tooltip("Потолок лимита живых врагов.")]
    [SerializeField, Min(1)] private int maxAliveCap = 35;

    [Tooltip("Базовый кулдаун спавна на сложности 0.")]
    [SerializeField, Min(0.05f)] private float baseSpawnCooldown = 2f;

    [Tooltip("На сколько уменьшаем кулдаун за каждый уровень сложности.")]
    [SerializeField, Min(0f)] private float cooldownDecreasePerDifficulty = 0.03f;

    [Tooltip("Нижний предел кулдауна (быстрее нельзя).")]
    [SerializeField, Min(0.05f)] private float minSpawnCooldown = 0.45f;

    [Header("Глобальные ограничения состава (чтобы не было нечестно)")]
    [Tooltip("Максимум автоматчиков одновременно.")]
    [SerializeField, Min(0)] private int globalMaxRangedAlive = 2;

    [Tooltip("Максимум щитовиков одновременно.")]
    [SerializeField, Min(0)] private int globalMaxShieldAlive = 4;

    [Header("Циклические этапы (ритм)")]
    [FormerlySerializedAs("endlessStages")]
    [SerializeField] private List<TimerStageConfig> timerStages = new();

    public event Action<int> OnTimerLevelChanged; // stageIndex + 1
    public event Action<float> OnTimerTick;
    public event Action<Enemy> OnEnemySpawned;

    public float ElapsedTime => _elapsedTime;
    public int TimerLvl => _timerLvl;
    public int DifficultyLevel => _difficultyLevel;

    private float _elapsedTime;
    private float _spawnTimer;

    private int _timerLvl;
    private int _difficultyLevel;

    private int _maxAlive;
    private float _spawnCooldown;

    private List<EnemyEntry> _currentEnemyPool;

    private readonly HashSet<Enemy> _aliveEnemies = new();

    private int _aliveRanged;
    private int _aliveShield;

    private bool _spawningEnabled;
    private bool _hasStarted;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (player == null)
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

        // Stage (cyclic rhythm)
        int activeStageIndex = GetActiveStageIndex(_elapsedTime);
        int computedLvl = activeStageIndex + 1;
        if (computedLvl != _timerLvl)
        {
            _timerLvl = computedLvl;
            OnTimerLevelChanged?.Invoke(_timerLvl);
        }

        // Global difficulty (never resets)
        _difficultyLevel = Mathf.FloorToInt(_elapsedTime / Mathf.Max(1f, difficultyStepSeconds));

        // Apply runtime settings (global base + stage multipliers)
        ApplyRuntimeSettings(activeStageIndex);

        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer <= 0f && _aliveEnemies.Count < _maxAlive)
        {
            _spawnTimer = _spawnCooldown;
            Spawn(activeStageIndex);
        }
    }

    public void StartGame()
    {
        if (_hasStarted) return;
        _hasStarted = true;

        if (playerSpawnPoint != null)
            player.transform.position = playerSpawnPoint.position;

        Enemy.SetFrozen(false);

        _spawningEnabled = true;
        _elapsedTime = 0f;
        _spawnTimer = 0f;

        _aliveEnemies.Clear();
        _aliveRanged = 0;
        _aliveShield = 0;

        int activeStageIndex = GetActiveStageIndex(0f);
        _timerLvl = activeStageIndex + 1;
        _difficultyLevel = 0;

        ApplyRuntimeSettings(activeStageIndex);
        OnTimerLevelChanged?.Invoke(_timerLvl);
    }

    private void ApplyRuntimeSettings(int stageIndex)
    {
        // Global base
        int maxAliveBase = baseMaxAlive + _difficultyLevel * maxAlivePerDifficulty;
        maxAliveBase = Mathf.Clamp(maxAliveBase, 0, maxAliveCap);

        float cooldownBase = baseSpawnCooldown - _difficultyLevel * cooldownDecreasePerDifficulty;
        cooldownBase = Mathf.Max(minSpawnCooldown, cooldownBase);

        // Stage multipliers
        TimerStageConfig stage = GetStageConfig(stageIndex);

        float maxAliveMul = stage != null ? Mathf.Max(0.1f, stage.maxAliveMultiplier) : 1f;
        float cooldownMul = stage != null ? Mathf.Max(0.1f, stage.cooldownMultiplier) : 1f;

        _maxAlive = Mathf.Clamp(Mathf.RoundToInt(maxAliveBase * maxAliveMul), 0, maxAliveCap);
        _spawnCooldown = Mathf.Max(minSpawnCooldown, cooldownBase * cooldownMul);

        // Enemy pool
        _currentEnemyPool = (stage != null && stage.enemiesOverride != null && stage.enemiesOverride.Count > 0)
            ? stage.enemiesOverride
            : defaultEnemies;
    }

    private void Spawn(int stageIndex)
    {
        TimerStageConfig stage = GetStageConfig(stageIndex);

        int stageMaxRanged = stage != null && stage.maxRangedAlive >= 0 ? stage.maxRangedAlive : globalMaxRangedAlive;
        int stageMaxShield = stage != null && stage.maxShieldAlive >= 0 ? stage.maxShieldAlive : globalMaxShieldAlive;

        EnemyEntry entry = PickEnemyEntryByDynamicWeight(
            _currentEnemyPool ?? defaultEnemies,
            _difficultyLevel,
            _aliveRanged, stageMaxRanged,
            _aliveShield, stageMaxShield
        );

        if (entry == null || entry.prefab == null) return;

        Transform sp = GetSpawnPointFor(entry.role);
        if (sp == null) return;

        Enemy enemy = Instantiate(entry.prefab, sp.position, sp.rotation);
        enemy.Init(player);

        _aliveEnemies.Add(enemy);

        if (entry.isRanged) _aliveRanged++;
        if (entry.hasShield) _aliveShield++;

        enemy.OnDespawned += OnEnemyDespawned;

        EnemySpawnMeta meta = enemy.GetComponent<EnemySpawnMeta>();
        if (meta == null) meta = enemy.gameObject.AddComponent<EnemySpawnMeta>();
        meta.isRanged = entry.isRanged;
        meta.hasShield = entry.hasShield;

        OnEnemySpawned?.Invoke(enemy);
    }

    private Transform GetSpawnPointFor(EnemyRole role)
    {
        List<Transform> points = role == EnemyRole.Scientist ? scientistSpawnPoints : soldierSpawnPoints;

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

        EnemySpawnMeta meta = enemy.GetComponent<EnemySpawnMeta>();
        if (meta != null)
        {
            if (meta.isRanged) _aliveRanged = Mathf.Max(0, _aliveRanged - 1);
            if (meta.hasShield) _aliveShield = Mathf.Max(0, _aliveShield - 1);
        }
    }

    private int GetActiveStageIndex(float elapsedSeconds)
    {
        if (timerStages == null || timerStages.Count == 0) return 0;

        float cycleDuration = GetTotalCycleDuration();
        if (cycleDuration <= 0f) return 0;

        float localTime = elapsedSeconds % cycleDuration;
        float accumulated = 0f;

        for (int i = 0; i < timerStages.Count; i++)
        {
            float duration = GetSafeStageDuration(timerStages[i]);
            accumulated += duration;
            if (localTime < accumulated) return i;
        }

        return timerStages.Count - 1;
    }

    private TimerStageConfig GetStageConfig(int stageIndex)
    {
        if (timerStages == null || timerStages.Count == 0) return null;
        if (stageIndex < 0 || stageIndex >= timerStages.Count) return null;
        return timerStages[stageIndex];
    }

    private float GetTotalCycleDuration()
    {
        if (timerStages == null || timerStages.Count == 0) return 0f;

        float duration = 0f;
        for (int i = 0; i < timerStages.Count; i++)
            duration += GetSafeStageDuration(timerStages[i]);

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

    private static EnemyEntry PickEnemyEntryByDynamicWeight(
        List<EnemyEntry> entries,
        int difficultyLevel,
        int aliveRanged, int maxRanged,
        int aliveShield, int maxShield
    )
    {
        if (entries == null || entries.Count == 0) return null;

        List<(EnemyEntry entry, float weight)> candidates = new(entries.Count);

        // Primary pass: with caps + unlock
        for (int i = 0; i < entries.Count; i++)
        {
            EnemyEntry e = entries[i];
            if (e == null || e.prefab == null) continue;

            if (difficultyLevel < e.unlockDifficulty) continue;

            if (maxRanged >= 0 && e.isRanged && aliveRanged >= maxRanged) continue;
            if (maxShield >= 0 && e.hasShield && aliveShield >= maxShield) continue;

            float w = Mathf.Max(0f, e.baseWeight) + Mathf.Max(0f, e.weightGrowthPerLevel) * difficultyLevel;
            if (w <= 0f) continue;

            candidates.Add((e, w));
        }

        // Fallback: if caps removed everything, ignore caps but still keep unlock
        if (candidates.Count == 0)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                EnemyEntry e = entries[i];
                if (e == null || e.prefab == null) continue;

                if (difficultyLevel < e.unlockDifficulty) continue;

                float w = Mathf.Max(0f, e.baseWeight) + Mathf.Max(0f, e.weightGrowthPerLevel) * difficultyLevel;
                if (w <= 0f) continue;

                candidates.Add((e, w));
            }
        }

        if (candidates.Count == 0) return entries[0];

        float sum = 0f;
        for (int i = 0; i < candidates.Count; i++)
            sum += candidates[i].weight;

        float roll = Random.Range(0f, sum);
        float acc = 0f;

        for (int i = 0; i < candidates.Count; i++)
        {
            acc += candidates[i].weight;
            if (roll <= acc) return candidates[i].entry;
        }

        return candidates[candidates.Count - 1].entry;
    }
    
    public void ResumeRun()
    {
        _spawningEnabled = true;
        Enemy.SetFrozen(false);
    }

}


/// <summary>
/// Метаданные спавна (для корректных счётчиков состава при смерти).
/// </summary>
public class EnemySpawnMeta : MonoBehaviour
{
    [Header("Теги состава (НЕ статы)")]
    public bool isRanged;

    [Header("Теги состава (НЕ статы)")]
    public bool hasShield;
}