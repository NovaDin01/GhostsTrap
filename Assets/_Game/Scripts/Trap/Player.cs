using System;
using UnityEngine;

public class Player : MonoBehaviour, ITakingDamage
{
    [Header("Стартовые характеристики щупалец")]
    [SerializeField] private float startGridSpeed = 8f;
    [SerializeField] private float startGridRadius = 1.5f;
    [SerializeField] private int startGridCount = 3;
    [SerializeField] private int hp = 5;
    
    [Header("СИСТЕМНЫЕ НАСТРОЙКИ")]
    [SerializeField] private EnemyPool enemyPool;
    
    [Header("Ссылки")]
    [SerializeField] private GridNet gridPrefab;
    
    [Header("Поиск врагов")]
    [SerializeField] private LayerMask enemiesMask;
    
    private Vector2 _targetPosition;
    private Vector2 _trapPosition;
    public int _currentGridCount;
    
    [Header("Итоговые переменные после улучшений")]
    private int _gridCount;
    private float _gridSpeed;
    private float _gridRadius;
    
    private void Awake()
    {
        _trapPosition = transform.position;

        _currentGridCount = 0;
        _gridCount = startGridCount;
        _gridSpeed = startGridSpeed;
        _gridRadius = startGridRadius;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _currentGridCount < _gridCount)
            GetCoordinates();
    }

    private void GetCoordinates() // Получение координат после нажатия ЛКМ
    {
        _targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GridCreate();
    }

    private void GridCreate()
    {
        GridNet grid = Instantiate(gridPrefab);
        _currentGridCount++;
        
        grid.OnLoot += HandleLoot;
        grid.onBack += GridBack;

        grid.Init(_gridSpeed, _gridRadius, enemiesMask, _trapPosition, _targetPosition);
    }
    
    private void HandleLoot(GameObject enemyGo)
    {
        if (enemyGo == null) return;

        if (enemyGo.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.ResetForPool(); 
            enemyPool.Return(enemy);
        }
        else
        {
            enemyGo.SetActive(false);
        }
    }

    private void GridBack(GameObject gridObj)
    {
        _currentGridCount--;
        Destroy(gridObj);
    }

    public void ApplyDamage(int amount)
    {
        hp--;
        if (hp <= 0)
        {
            Debug.Log("Death"); // Потом переделать в метод + событие
        }
        
    }
}
