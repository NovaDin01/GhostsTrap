using System;
using UnityEngine;

public enum EnemyType
{
    Basic,
    Rare
}

public class Ghost : MonoBehaviour
{
    [Header("Характеристики")] 
    private float _speed;
    private float _money;
    private EnemyType _enemyType;
    
    public float Speed => _speed;
    public float Money => _money;
    public EnemyType EnemyType => _enemyType;

    [Header("Компоненты")] 
    private IGhostAbility _ability;
    private IGhostMovement _movement;

    [SerializeField] private EnemyData enemyData;
    private bool canMove = true;

    private void Awake() // Задаем характеристики через SO
    {
        _speed = enemyData.speed;
        _money = enemyData.money;
        _enemyType = enemyData.enemyType;
    }
    public void Spawn(IGhostMovement movement)
    {
        _movement = movement;
        _movement.Init(this);
    }

    private void Update()
    {
        if(canMove) _movement?.Tick();
    }

    public void OnCatch()
    {
        canMove = false;
    }
}
