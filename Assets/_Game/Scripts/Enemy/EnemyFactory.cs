using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyFactory : MonoBehaviour
{
    [Header("НАСТРОЙКИ ГЕЙМДИЗАЙНА")]
    [Header("Настройки спавна врагов")]
    [SerializeField, Tooltip("Минимальная дистанция спавна от центра")]
    private float minDistance;

    [SerializeField, Tooltip("Максимальная дистанция спавна от центра")]
    private float maxDistance;

    [SerializeField, Tooltip("Минимальное время между спавнами")]
    private float minTimeSpawn;

    [SerializeField, Tooltip("Максимальное время между спавнами")]
    private float maxTimeSpawn;

    [Header("Настройки движения врагов")]
    //[SerializeField, Tooltip("Для настройки движения необходимо дважды нажать на ячейку справа")]

    [FormerlySerializedAs("randomMoveData")] [SerializeField, Tooltip("Для настройки движения необходимо дважды нажать на ячейку справа")]
    private PanicMoveSO panicMoveData;

    [Header("СИСТЕМНЫЕ НАСТРОЙКИ")]
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private EnemyPool enemyPool;
    [SerializeField] private int startPoolCount = 10;

    private Vector2 spawnDir;
    private IEnemyMovement _movement;

    private void Start()
    {
        enemyPool.Init(enemyPrefab, startPoolCount); 
        StartCoroutine(SpawnDelay());
    }

    // Куладун между спавнами призраков
    private IEnumerator SpawnDelay()
    {
        yield return new WaitForSeconds(Random.Range(minTimeSpawn, maxTimeSpawn));
        SpawnEnemy();
    }

    // Рандомная выборка типа движения
    private void RandomMovement()
    {
        int index = Random.Range(0, 2);

        switch (index)
        {
            case 0:
                _movement = new PanicMove(panicMoveData);
                break;
        }
    }

    // Задание начальных параметров призраку
    public void SpawnEnemy()
    {
        StartCoroutine(SpawnDelay());

        spawnDir = Random.insideUnitCircle.normalized;

        Enemy enemy = enemyPool.Get();

        enemy.transform.position = spawnDir * Random.Range(minDistance, maxDistance);
        
        enemy.OnDespawnRequested -= ReturnToPool; 
        enemy.OnDespawnRequested += ReturnToPool;

        RandomMovement();
        //enemy.SetMovement(_movement);
    }

    // Возвращение в пул
    private void ReturnToPool(Enemy enemy)
    {
        enemy.OnDespawnRequested -= ReturnToPool;
        enemy.ResetForPool(); 
        enemyPool.Return(enemy);
    }

}
