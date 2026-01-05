using System.Collections;
using UnityEngine;

public class GhostFactory : MonoBehaviour
{
    [Header("НАСТРОЙКИ ГЕЙМДИЗАЙНА")]
    [Header("Настройки спавна призраков")]
    [SerializeField, Tooltip("Минимальная дистанция спавна от центра")]
    private float minDistance;

    [SerializeField, Tooltip("Максимальная дистанция спавна от центра")]
    private float maxDistance;

    [SerializeField, Tooltip("Минимальное время между спавнами")]
    private float minTimeSpawn;

    [SerializeField, Tooltip("Максимальное время между спавнами")]
    private float maxTimeSpawn;

    [Header("Настройки движения призраков")]
    [SerializeField, Tooltip("Для настройки движения необходимо дважды нажать на ячейку справа")]
    private BaseMoveSO baseMoveData;

    [SerializeField, Tooltip("Для настройки движения необходимо дважды нажать на ячейку справа")]
    private RandomMoveSO randomMoveData;

    [Header("СИСТЕМНЫЕ НАСТРОЙКИ")]
    [SerializeField] private Ghost ghostPrefab;
    [SerializeField] private EnemyPool enemyPool;
    [SerializeField] private int startPoolCount = 10;

    private Vector2 spawnDir;
    private IGhostMovement _movement;

    private void Start()
    {
        enemyPool.Init(ghostPrefab, startPoolCount); 
        StartCoroutine(SpawnDelay());
    }

    // Куладун между спавнами призраков
    private IEnumerator SpawnDelay()
    {
        yield return new WaitForSeconds(Random.Range(minTimeSpawn, maxTimeSpawn));
        SpawnGhost();
    }

    // Рандомная выборка типа движения
    private void RandomMovement()
    {
        int index = Random.Range(1, 3);

        switch (index)
        {
            case 1:
                _movement = new BaseMove(baseMoveData);
                break;
            case 2:
                _movement = new RandomMove(randomMoveData);
                break;
        }
    }

    // Задание начальных параметров призраку
    public void SpawnGhost()
    {
        StartCoroutine(SpawnDelay());

        spawnDir = Random.insideUnitCircle.normalized;

        Ghost ghost = enemyPool.Get();

        ghost.transform.position = spawnDir * Random.Range(minDistance, maxDistance);
        ghost.currentLifeTime = 0;
        
        ghost.OnDespawnRequested -= ReturnToPool; 
        ghost.OnDespawnRequested += ReturnToPool;

        RandomMovement();
        ghost.SetMovement(_movement);
    }

    // Возвращение в пул
    private void ReturnToPool(Ghost ghost)
    {
        ghost.OnDespawnRequested -= ReturnToPool;
        ghost.ResetForPool(); 
        enemyPool.Return(ghost);
    }

}
