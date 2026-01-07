using System;
using UnityEngine;

public enum GridState
{
    Throwing,
    Stopping,
    Returning
}

public class GridNet : MonoBehaviour
{
    private float _speed;
    private float _radius;
    private LayerMask _enemiesMask;
    private Vector2 _trap;
    private Vector2 _target;
    
    private GridState _state;
    
    private Collider2D[] _enemies = Array.Empty<Collider2D>();
    public event Action<GameObject> OnLoot;
    public event Action<GameObject> onBack;

    private void Update()
    {
        State();
    }

    // Состояния ловушки
    private void State()
    {
        switch (_state)
        {
            case GridState.Throwing:
                MoveToTarget();
                break;
            
            case GridState.Returning:
                MoveToTrap();
                break;
            
            case GridState.Stopping:
                break;
        }
    }

    // Инициализация ловушки
    public void Init(float speed, float radius, LayerMask enemiesMask, Vector2 trap, Vector2 target)
    {
        _target = target;
        _trap = trap;
        _speed = speed;
        _radius = radius;
        _enemiesMask = enemiesMask;

        SetDefaultSettings();
    }

    // Установка дефолтных настроек
    private void SetDefaultSettings()
    {
        transform.position = _trap;

        _state = GridState.Throwing;
    }

    // Движение к цели
    private void MoveToTarget()
    {
        GridMove(_target);
        if (ClosingOnTarget(_target))
        {
            _state = GridState.Stopping;
            GetCaught();
        }
    }

    // Движение к ловушке
    private void MoveToTrap()
    {
        GridMove(_trap);
        if (ClosingOnTarget(_trap))
        {
            _state = GridState.Stopping;
            GetLoot();
        }
    }

    // Логика поимки
    private void GetCaught()
    {
        _enemies = Physics2D.OverlapCircleAll(transform.position, _radius, _enemiesMask);
        
        foreach (var enemy in _enemies)
        {
            if (enemy == null) continue;
            
            if (enemy.TryGetComponent<Ghost>(out Ghost ghost)) // Позже переделать под интерфейс
            {
                if (ghost.IsCaught) continue;
                
                ghost.transform.SetParent(transform);
                ghost.transform.position = transform.position;
                ghost.OnCatch();
            }
        }

        _state = GridState.Returning;
    }

    // Получение лута
    private void GetLoot()
    {
        foreach (var enemy in _enemies)
        {
            if (enemy == null) continue;

            enemy.transform.SetParent(null);
            OnLoot?.Invoke(enemy.gameObject);
        }
        onBack?.Invoke(gameObject);
        
        _enemies = Array.Empty<Collider2D>();
    }



    // Метод - помощник. Проверяет находится ли сеть рядом с point
    private bool ClosingOnTarget(Vector2 point)
    {
        return Vector2.Distance(transform.position, point) < 0.1f;
    }

    // Метод - помощник. Метод движения ловушки к target
    private void GridMove(Vector2 target) // Метод движения ловушки
    {
        transform.position = Vector2.MoveTowards(transform.position, target, _speed * Time.deltaTime);
    }
}
