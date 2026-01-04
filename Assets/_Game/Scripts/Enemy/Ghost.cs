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
    
    [SerializeField] private EnemyData enemyData;
    private bool canMove = true;

    private void Awake() // Задаем характеристики через SO
    {
        _speed = enemyData.speed;
        _money = enemyData.money;
        _enemyType = enemyData.enemyType;
    }

    public void ApplyMovement()
    {
        
    }

    public void ApplyAbility()
    {
        
    }

    public void ApplyVisual()
    {
        
    }

    private void OnEnable()
    {
        
    }

    public void OnCatch()
    {
        canMove = false;
    }
}
