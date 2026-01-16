using System;
using _Game.Scripts.Data;
using UnityEngine;

public class MeleeMove : IEnemyMovement
{
    private Enemy _enemy;
    private Player _player;
    private SpriteRenderer _sprite;

    private MeleeMoveSO _setting;

    
    public void Init(Enemy enemy, Player player, MovementSettingsSO settingsSo)
    {
        _player = player;
        _enemy = enemy;
        _setting = settingsSo as MeleeMoveSO;

        if (_setting == null)
            throw new ArgumentException("MeleeMove requires MeleeMoveSO");

        _sprite = enemy.GetComponentInChildren<SpriteRenderer>();
    }

    public void Tick()
    {
        float step = _setting.Speed * Time.deltaTime;

        Vector3 enemyPos = _enemy.transform.position;
        Vector3 targetPos = _player.transform.position;

        Vector3 delta = targetPos - enemyPos; // направление к игроку

        // flip по X (минимально)
        if (_sprite != null && Mathf.Abs(delta.x) > 0.01f)
            _sprite.flipX = delta.x > 0;

        _enemy.transform.position = Vector3.MoveTowards(enemyPos, targetPos, step);
    }
}