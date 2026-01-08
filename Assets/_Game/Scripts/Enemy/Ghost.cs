using System;
using UnityEngine;

public abstract class Ghost : MonoBehaviour, IObjectAttracted
{
    [Header("НАСТРОЙКИ ГЕЙМДИЗАЙНА")] 
    
    [SerializeField, Tooltip("Время жизни призрака")]
    private float lifeTime;
    
    [Header("Компоненты")] 
    //private IGhostAbility _ability;
    private IEnemyMovement _movement;

    public IEnemyMovement Movement => _movement;

    [Header("")] 
    public float currentLifeTime;
    private bool _isCaught = false;
    public event Action<Ghost> OnDespawnRequested;


    
    // Установка типа движения
    // public void SetMovement(IEnemyMovement movement)
    // {
    //     _movement = movement;
    //     _movement.Init(this);
    // }

    private void Update()
    {
        if (_isCaught) return;

        _movement?.Tick();

        currentLifeTime += Time.deltaTime;
        if (currentLifeTime >= lifeTime)
        {
            RequestDespawn();
        }
    }

    // Проверка на: поймали ли призрака
    public void OnCatch()
    {
        _isCaught = true;
    }
    
    public void ResetForPool()
    {
        _isCaught = false;
        currentLifeTime = 0f;
        transform.SetParent(null);
    }

    public bool IsCaught()
    {
        return _isCaught;
    }
    
    // Событие возвращающее в пул
    private void RequestDespawn()
    {
        OnDespawnRequested?.Invoke(this);
    }

}
