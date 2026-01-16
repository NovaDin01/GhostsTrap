using System;
using _Game.Scripts.Data;
using UnityEngine;
using Random = UnityEngine.Random;

public class PanicMove : IEnemyMovement
{
    private Enemy _enemy;

    private Vector2 _startPos;
    private Vector2 _dir;

    private PanicMoveSO _setting;
    
    private SpriteRenderer sprite;


    private float _segmentDistance = 1;

    // Начальные свойства типа движения.
    
    public void Init(Enemy enemy, Player player, MovementSettingsSO settingsSo)
    {
        _enemy = enemy;
        _setting = settingsSo as PanicMoveSO;

        if (_setting == null)
            throw new ArgumentException("PanicMove requires PanicMoveSO");

        _segmentDistance = _setting.MinDistance;
        sprite = enemy.GetComponentInChildren<SpriteRenderer>(); 

        StartNewSegment();
    }


    // Движение за кадр
    public void Tick()
    {
        float step = _setting.Speed * Time.deltaTime;
        _enemy.transform.position += (Vector3)(_dir * step);
        
        Vector3 diff = (Vector2)_enemy.transform.position - _startPos;
        float distSqr = diff.sqrMagnitude;
        float segmentDistSqr = _segmentDistance * _segmentDistance;

        if (distSqr >= segmentDistSqr)
        {
            StartNewSegment();
        }

    }

    // Установка направления движения
    private void StartNewSegment()
    {
        _startPos = _enemy.transform.position;
        _segmentDistance = Random.Range(_setting.MinDistance, _setting.MaxDistance);
        _dir = RandomDir();
        
        if (sprite != null && Mathf.Abs(_dir.x) > 0.01f)
            sprite.flipX = _dir.x > 0;
    }


    // Рандомизация вектора движения
    private Vector2 RandomDir()
    {
        return Random.insideUnitCircle.normalized;
    } 
}