using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BarrelSpawner : MonoBehaviour
{
    [SerializeField] private List<Barrel> barrelsType;
    [SerializeField] private List<Transform> spawnerPoints;
    [SerializeField] private float spawnCooldown = 2f;
    [SerializeField] private int maxAlive = 3;

    private float timer;
    private int aliveCount;

    // занятые точки
    private readonly HashSet<Transform> occupiedPoints = new();

    private void Start()
    {
        timer = spawnCooldown;
    }

    private void Update()
    {
        if (aliveCount >= maxAlive) return;

        // если все точки заняты — нечего спавнить
        if (occupiedPoints.Count >= spawnerPoints.Count) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        Spawn();
        timer = spawnCooldown;
    }

    private void Spawn()
    {
        if (barrelsType == null || barrelsType.Count == 0) return;
        if (spawnerPoints == null || spawnerPoints.Count == 0) return;

        // собрать список свободных точек
        List<Transform> free = null;
        for (int i = 0; i < spawnerPoints.Count; i++)
        {
            var p = spawnerPoints[i];
            if (p == null) continue;
            if (occupiedPoints.Contains(p)) continue;

            free ??= new List<Transform>();
            free.Add(p);
        }

        if (free == null || free.Count == 0) return;

        int barrelIndex = Random.Range(0, barrelsType.Count);
        Transform point = free[Random.Range(0, free.Count)];

        Barrel spawned = Instantiate(
            barrelsType[barrelIndex],
            point.position,
            point.rotation
        );

        // помечаем точку занятой и запоминаем в бочке
        occupiedPoints.Add(point);
        spawned.SetSpawnPoint(point);

        aliveCount++;

        // на исчезновение бочки — освобождаем точку
        spawned.OnDespawned += OnBarrelDespawned;
    }

    private void OnBarrelDespawned(Barrel barrel)
    {
        barrel.OnDespawned -= OnBarrelDespawned;

        // освободить точку
        if (barrel.SpawnPoint != null)
            occupiedPoints.Remove(barrel.SpawnPoint);

        aliveCount = Mathf.Max(0, aliveCount - 1);
    }
}
