using System;
using System.Collections.Generic;
using UnityEngine;
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
    public class LocationConfig
    {
        public Location location;

        [Header("Кого спавним в этой локации")]
        public List<EnemyEntry> enemies = new();

        [Header("Точки спавна")]
        public List<Transform> scientistSpawnPoints = new();
        public List<Transform> soldierSpawnPoints = new();

        [Header("Ограничение по количеству (по уровням таймера 1..4)")]
        public int[] allCountPerLvl = new int[4] { 10, 15, 24, 30 };

        [Header("КД спавна (по уровням таймера 1..4)")]
        public float[] spawnCooldownPerLvl = new float[4] { 3f, 2f, 1.5f, 1f };
    }


    [Header("Configs")]
    [SerializeField] private List<LocationConfig> _locations = new();

    [Header("Runtime refs")]
    [SerializeField] private Player _player;

    [Header("Timer settings")]
    [SerializeField] private float _roundDuration = 120f;   // 2 минуты
    [SerializeField] private float _levelInterval = 30f;    // каждые 30 секунд +1 lvl
    [SerializeField] private int _maxTimerLvl = 4;          // 1..4
    [SerializeField] private float _transitionDelay = 1.0f; // сколько длится анимация перехода


    // events
    public event Action<int> OnTimerLevelChanged;           // когда lvl повысился
    public event Action<float> OnTimerTick;                // если надо UI (остаток времени)
    public event Action<Location> OnLocationStarted;        // локация стартовала
    public event Action<Location> OnLocationFinished;       // 2 минуты закончились (перед переходом)
    

    public float RemainingTime => _remainingTime;
    public int TimerLvl => _timerLvl;
    public Location CurrentLocation => _currentLocation;

    private Location _currentLocation;
    private LocationConfig _currentConfig;

    private float _remainingTime;
    private float _spawnTimer;

    private int _timerLvl;         // 1..4
    private int _currentCount;
    private int _allCount;
    private float _spawnCooldown;

    private bool _spawningEnabled = true;
    private bool _hasStarted;

    private void Awake()
    {
        // singleton
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

        // ---- TIMER: 120 -> 0 ----
        _remainingTime = Mathf.Max(0f, _remainingTime - Time.deltaTime);
        OnTimerTick?.Invoke(_remainingTime);

        // вычисляем lvl по прошедшему времени: каждые 30 сек +1
        float elapsed = _roundDuration - _remainingTime;
        int computedLvl = _levelInterval > 0f
            ? Mathf.Clamp(1 + Mathf.FloorToInt(elapsed / _levelInterval), 1, _maxTimerLvl)
            : 1;

        if (computedLvl != _timerLvl)
        {
            _timerLvl = computedLvl;
            ApplyLevelSettings(_timerLvl);
            OnTimerLevelChanged?.Invoke(_timerLvl);
        }

        // конец раунда (2 минуты)
        if (_remainingTime <= 0f)
        {
            FinishLocation();
            return;
        }

        // ---- SPAWN ----
        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer <= 0f && _currentCount < _allCount)
        {
            _spawnTimer = _spawnCooldown;
            Spawn();
        }
    }

    // -------------------- PUBLIC API --------------------

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

        // reset round
        _spawningEnabled = true;
        _remainingTime = _roundDuration;
        _spawnTimer = 0f;

        _timerLvl = 1;
        _currentCount = 0;

        ApplyLevelSettings(_timerLvl);
        OnTimerLevelChanged?.Invoke(_timerLvl);
        OnLocationStarted?.Invoke(_currentLocation);
    }

    // вызови это извне, если хочешь принудительно перейти к следующей локации
    public void GoNextLocation()
    {
        Location next = GetNextLocation(_currentLocation);
        StartLocation(next);
    }

    // -------------------- CORE --------------------

    private void FinishLocation()
    {
        _spawningEnabled = false;
        OnLocationFinished?.Invoke(_currentLocation);

        // например триггер анимации
        // _player.PlayTransitionAnimation();

        StartCoroutine(TransitionToNextLocation());
    }

    private System.Collections.IEnumerator TransitionToNextLocation()
    {
        yield return new WaitForSeconds(_transitionDelay);
        GoNextLocation();
    }


    private void ApplyLevelSettings(int lvl)
    {
        int idx = Mathf.Clamp(lvl - 1, 0, _maxTimerLvl - 1);

        // защита от кривых массивов
        _allCount = GetArrayValueSafe(_currentConfig.allCountPerLvl, idx, 10);
        _spawnCooldown = GetArrayValueSafe(_currentConfig.spawnCooldownPerLvl, idx, 2f);
    }

    private void Spawn()
    {
        EnemyEntry entry = PickEnemyEntryByWeight(_currentConfig.enemies);
        if (entry == null || entry.prefab == null) return;

        Transform sp = GetSpawnPointFor(entry.role);
        if (sp == null) return;

        Enemy enemy = Instantiate(entry.prefab, sp.position, sp.rotation);
        enemy.Init(_player);

        _currentCount++;
        enemy.OnDespawned += OnEnemyDespawned;
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
        _currentCount = Mathf.Max(0, _currentCount - 1);
    }

    // -------------------- HELPERS --------------------

    private LocationConfig GetConfig(Location loc)
    {
        for (int i = 0; i < _locations.Count; i++)
            if (_locations[i].location == loc)
                return _locations[i];

        return null;
    }

    private static int GetArrayValueSafe(int[] arr, int index, int fallback)
    {
        if (arr == null || arr.Length == 0) return fallback;
        if (index < 0) index = 0;
        if (index >= arr.Length) index = arr.Length - 1;
        return arr[index];
    }

    private static float GetArrayValueSafe(float[] arr, int index, float fallback)
    {
        if (arr == null || arr.Length == 0) return fallback;
        if (index < 0) index = 0;
        if (index >= arr.Length) index = arr.Length - 1;
        return arr[index];
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

    private Location GetNextLocation(Location current)
    {
        if (_locations != null && _locations.Count > 0)
        {
            int currentIndex = _locations.FindIndex(config => config.location == current);
            if (currentIndex >= 0)
            {
                int nextIndex = (currentIndex + 1) % _locations.Count;
                return _locations[nextIndex].location;
            }
        }

        int count = Enum.GetValues(typeof(Location)).Length;
        int enumIndex = ((int)current + 1) % count;
        return (Location)enumIndex;
    }
}
