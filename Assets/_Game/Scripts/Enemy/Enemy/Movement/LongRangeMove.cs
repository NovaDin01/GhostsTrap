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

    private SpriteRenderer _sprite;
    private Vector2 _prevPos; // для орбиты

    public void Init(Enemy enemy, Player player, MovementSettingsSO settingsSo)
    {
        _player = player;
        _enemy = enemy;
        _setting = settingsSo as LongRangeMoveSO;

        if (_setting == null)
            throw new ArgumentException("LongRangeMove requires LongRangeMoveSO");

        _segmentDistance = _setting.Distance;

        _sprite = enemy.GetComponentInChildren<SpriteRenderer>();
        _prevPos = _enemy.transform.position;
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
                _prevPos = enemyPos; // сброс, чтобы не было скачка флипа
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

    private void LinearMove()
    {
        float step = _setting.Speed * Time.deltaTime;

        Vector3 enemyPos = _enemy.transform.position;
        Vector3 targetPos = _player.transform.position;

        Vector3 delta = targetPos - enemyPos;

        if (_sprite != null && Mathf.Abs(delta.x) > 0.01f)
            _sprite.flipX = delta.x > 0;

        _enemy.transform.position = Vector3.MoveTowards(enemyPos, targetPos, step);
    }

    private void RoundMove()
    {
        angle += _setting.AngularSpeed * Time.deltaTime;

        var x = Mathf.Cos(angle) * _segmentDistance;
        var y = Mathf.Sin(angle) * _segmentDistance;

        Vector2 center = _player.transform.position;
        Vector2 newPos = center + new Vector2(x, y);

        // flip по фактическому движению
        Vector2 vel = newPos - _prevPos;
        if (_sprite != null && Mathf.Abs(vel.x) > 10f)
            _sprite.flipX = vel.x > 0;

        _enemy.transform.position = newPos;
        _prevPos = newPos;
    }
}
