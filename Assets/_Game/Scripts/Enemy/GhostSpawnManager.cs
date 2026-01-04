using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

//Класс появления призраков - место появления, временные лимиты нахождения
public class GhostSpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _ghostPrefab;

    [SerializeField] private int _ghostTrapCountInLevel = 10;
    public int currentGhostTrapCount = 0;
    private float[] _ghostSpawnPositions = { -4, -3, -2, 2, 3, 4 };
    private SpriteRenderer _ghostRenderer;
    //[SerializeField] private Sprite[] _ghostSprites;
    
    [Header("Ссылки")] 
    [SerializeField] private EnemyPool _enemyPool;
    [SerializeField] private TrapController _trapController;

    private void Awake()
    {
        _enemyPool.Init(_ghostPrefab, 5);
    }

    private void Start()
    {
        StartCoroutine(GhostSpawnTime());
    }
    public void GhostSpawn()
    {
        int RandomIndexX = Random.Range(0, _ghostSpawnPositions.Length);
        int RandomIndexY = Random.Range(0, _ghostSpawnPositions.Length);

        float x = _ghostSpawnPositions[RandomIndexX];
        float y = _ghostSpawnPositions[RandomIndexY];

        Vector2 spawnPosition = new Vector2(x, y);
        
        GameObject newGhost = _enemyPool.GetObject();
        newGhost.transform.SetParent(null); 
        newGhost.transform.position = spawnPosition;
        
        var behaviour = newGhost.GetComponent<GhostBehaviour>();
        behaviour.SetPool(_enemyPool);
        
    }

    // public void RandomVisualGhost()
    // {
    //     int randomIndexSprites = Random.Range(0, _ghostSprites.Length);
    //     Sprite ghostSprite = _ghostSprites[randomIndexSprites];
    //     _ghostRenderer.sprite = ghostSprite;
    // }

    IEnumerator GhostSpawnTime()
    {
        while (currentGhostTrapCount < _ghostTrapCountInLevel)
        {
            GhostSpawn();
            // RandomVisualGhost();
            int randomDelay = Random.Range(2, 5);
            yield return new WaitForSeconds(randomDelay);
        }
    }

    private void CountGhostInTrap(GameObject ghost) // Добавляется кол-во пойманных врагов
    {
        currentGhostTrapCount++;
    }
    
    private void OnEnable()
    {
        _trapController.OnLoot += CountGhostInTrap;
    }
    
    private void OnDisable()
    {
        _trapController.OnLoot -= CountGhostInTrap;
    }

}
