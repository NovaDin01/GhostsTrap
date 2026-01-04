using System.Xml.Serialization;
using TMPro;
using UnityEngine;
// Класс поведения призрака: жизнь, движение, захват и возможность вырваться
public class GhostBehaviour : MonoBehaviour
{
    [Header("Параметры жизни призрака")]
    [SerializeField] private float _minLifeTime = 1f;   // минимальное время жизни
    [SerializeField] private float _maxLifeTime = 4f;   // максимальное время жизни
    private float _lifeTimer;                           // таймер жизни

    [Header("Состояние призрака")]
    private bool _isCatched = false;                     // флаг пойман или нет
    private Transform _trap;                           // ссылка на ловушку

    [Header("Движение призрака")]
    private float _moveSpeed;                           // скорость движения (рандомная)
    private Vector2 _direction;                         // направление (вверх/вниз/влево/вправо)
    private float _walkDistance = 3f;                   // расстояние прогулки
    private Vector2 _startPosition;                     // точка спавна для отсчёта
    [SerializeField] private float _minSpeed = 1f;      // минимальная скорость прогулки
    [SerializeField] private float _maxSpeed = 3f;      // максимальная скорость прогулкиы
    [SerializeField] private Transform _ghostPortal;    // Место, куда призрак прячется когда сбегает от ловушки

    [Header("Ссылки")] 
    private EnemyPool _enemyPool;


    private void OnEnable() // Заменил Start -> OnEnable, так как Start вызывается при создании лишь. У нас объекты вкл и откл для оптимизации.
    {
        _isCatched = false;
        _trap = null;
        _startPosition = transform.position;
        _lifeTimer = Random.Range(_minLifeTime, _maxLifeTime);
        _moveSpeed = Random.Range(_minSpeed, _maxSpeed);

        int walkDirection = Random.Range(0, 4);
        if (walkDirection == 0) _direction = Vector2.up;
        if (walkDirection == 1) _direction = Vector2.down;
        if (walkDirection == 2) _direction = Vector2.left;
        if (walkDirection == 3) _direction = Vector2.right;
    }

    private void Update()
    {
        if (_isCatched)
            return;

        _lifeTimer -= Time.deltaTime;
        if (_lifeTimer <= 0)
        {
            Dissapear();
            return;
        }
        
        GhostWalk();
    
    }
    
    public void SetPool(EnemyPool pool)
    {
        _enemyPool = pool;
    }

    public void OnCatch(Transform trapTransform)
    {
        _isCatched = true;
        _trap = trapTransform;
    }
    private void GhostWalk() //метод прогулки призрака
    {
        transform.Translate(_direction * _moveSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, _startPosition) >= _walkDistance)
        {
            _direction = -_direction;
            _startPosition = transform.position;
        }
        
    }
    public void Dissapear() // Возврат в пул
    {
        if (_enemyPool == null)
        {
            gameObject.SetActive(false);
            return;
        }
        _enemyPool.ReturnObject(gameObject);
    }
}
