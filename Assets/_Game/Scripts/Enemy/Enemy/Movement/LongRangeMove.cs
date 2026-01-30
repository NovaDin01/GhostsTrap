using System;
using _Game.Scripts.Data;
using UnityEngine;

public enum MovementState
{
    Move2Target,
    MoveAroundTarget
}

/// <summary>
/// Дальнобойный враг:
/// - если дальше чем Distance -> идет к игроку
/// - если на Distance и ближе -> орбитит вокруг игрока на радиусе Distance
/// ВАЖНО: во время орбиты флип делаем НЕ по vel.x (он постоянно меняет знак),
/// а по направлению НА игрока (стабильно, без дерганий).
/// </summary>
public class LongRangeMove : IEnemyMovement
{
    private Enemy _enemy;
    private Player _player;

    private LongRangeMoveSO _setting;

    private MovementState _state;

    private float _angle;
    private float _segmentDistance;

    private SpriteRenderer _sprite;
    private Vector2 _prevPos;

    public void Init(Enemy enemy, Player player, MovementSettingsSO settingsSo)
    {
        _player = player;
        _enemy = enemy;

        _setting = settingsSo as LongRangeMoveSO;
        if (_setting == null)
            throw new ArgumentException("LongRangeMove requires LongRangeMoveSO");

        _segmentDistance = _setting.Distance;

        _sprite = _enemy.GetComponentInChildren<SpriteRenderer>();
        _prevPos = _enemy.transform.position;

        // стартовое состояние
        Vector2 enemyPos = _enemy.transform.position;
        Vector2 playerPos = _player.transform.position;

        float distSqr = (enemyPos - playerPos).sqrMagnitude;
        float orbitSqr = _segmentDistance * _segmentDistance;

        _state = distSqr > orbitSqr ? MovementState.Move2Target : MovementState.MoveAroundTarget;

        if (_state == MovementState.MoveAroundTarget)
        {
            Vector2 dir = enemyPos - playerPos;
            _angle = Mathf.Atan2(dir.y, dir.x);
        }
    }

    public void Tick()
    {
        if (_enemy == null || _player == null) return;

        Vector2 enemyPos = _enemy.transform.position;
        Vector2 playerPos = _player.transform.position;

        float distSqr = (enemyPos - playerPos).sqrMagnitude;
        float orbitSqr = _segmentDistance * _segmentDistance;

        var newState = distSqr > orbitSqr
            ? MovementState.Move2Target
            : MovementState.MoveAroundTarget;

        if (_state != newState)
        {
            _state = newState;

            if (_state == MovementState.MoveAroundTarget)
            {
                // фиксируем угол, чтобы вход в орбиту был без скачка
                Vector2 dir = enemyPos - playerPos;
                _angle = Mathf.Atan2(dir.y, dir.x);
                _prevPos = enemyPos;
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

        // флип по направлению к игроку (стабильно)
        if (Mathf.Abs(delta.x) > 0.01f)
            ApplyFlipByX(delta.x);

        _enemy.transform.position = Vector3.MoveTowards(enemyPos, targetPos, step);
        _prevPos = _enemy.transform.position;
    }

    private void RoundMove()
    {
        _angle += _setting.AngularSpeed * Time.deltaTime;

        float x = Mathf.Cos(_angle) * _segmentDistance;
        float y = Mathf.Sin(_angle) * _segmentDistance;

        Vector2 center = _player.transform.position;
        Vector2 newPos = center + new Vector2(x, y);

        // ВАЖНО: при орбите флип по скорости (vel.x) будет дергаться,
        // потому что знак vel.x постоянно меняется около верх/низ точки круга.
        // Поэтому флип делаем по направлению НА игрока — стабильно.
        Vector2 toPlayer = center - (Vector2)_enemy.transform.position;
        if (_sprite != null && Mathf.Abs(toPlayer.x) > 0.01f)
            ApplyFlipByX(toPlayer.x);

        _enemy.transform.position = newPos;
        _prevPos = newPos;
    }

    /// <summary>
    /// Унифицированный flip: учитывает, куда "смотрит" спрайт по умолчанию.
    /// Ожидается поле FacingRightByDefault в LongRangeMoveSO:
    /// - true  => спрайт без flip смотрит вправо
    /// - false => спрайт без flip смотрит влево
    /// </summary>
    private void ApplyFlipByX(float x)
    {
        if (_sprite == null) return;

        bool movingRight = x > 0f;

        // если спрайт смотрит вправо по умолчанию -> flip когда "смотрим" влево
        // если спрайт смотрит влево по умолчанию -> flip когда "смотрим" вправо
        _sprite.flipX = _setting.FacingRightByDefault ? !movingRight : movingRight;
    }
}
