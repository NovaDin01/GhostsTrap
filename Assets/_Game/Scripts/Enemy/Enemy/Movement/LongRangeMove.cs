using System;
using _Game.Scripts.Data;
using UnityEngine;

enum MovementState
{
    Move2Target,
    MoveAroundTarget
}

public class LongRangeMove : IEnemyMovement
{
    private Enemy _enemy;
    private Player _player;

    private LongRangeMoveSO _setting;
    private MovementState _state;
    private float angle;

    private float _segmentDistance;

    // Начальные свойства типа движения.
    public void Init(Enemy enemy, Player player, MovementSettingsSO settingsSo)
    {
        _player = player;
        _enemy = enemy;
        _setting = settingsSo as LongRangeMoveSO;
        
        if (_setting == null)
            throw new ArgumentException("PanicMove requires PanicMoveSO");
        
        _segmentDistance = _setting.Distance;
    }

    public void Tick()
    {
        Vector2 enemyPos  = _enemy.transform.position;
        Vector2 playerPos = _player.transform.position;

        Vector2 diff = enemyPos - playerPos;
        float distSqr  = diff.sqrMagnitude;
        float orbitSqr = _segmentDistance * _segmentDistance;

        var newState = distSqr > orbitSqr
            ? MovementState.Move2Target
            : MovementState.MoveAroundTarget;

        if (_state != newState)
        {
            _state = newState;
            
            if (_state == MovementState.MoveAroundTarget)
            {
                Vector2 dir = enemyPos - playerPos;
                angle = Mathf.Atan2(dir.y, dir.x);
            }
        }
        switch (_state)
        {
            case MovementState.Move2Target:
                LinearMove();
                break;

            case MovementState.MoveAroundTarget:
                RoundMove();
                break;
        }
    }


    private void LinearMove() // Движение к цели
    {
        float step = _setting.Speed * Time.deltaTime;
        _enemy.transform.position = Vector3.MoveTowards(
            _enemy.transform.position,
            _player.transform.position,
            step
        );
    }

    private void RoundMove() // Движение вокруг цели
    {
        angle += _setting.AngularSpeed * Time.deltaTime;

        var x = Mathf.Cos(angle) * _segmentDistance;
        var y = Mathf.Sin(angle) * _segmentDistance;

        Vector2 center = _player.transform.position;
        _enemy.transform.position = center + new Vector2(x, y);
    }

    
}