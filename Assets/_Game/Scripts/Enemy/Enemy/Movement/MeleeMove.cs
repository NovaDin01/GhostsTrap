using System;
using _Game.Scripts.Data;
using UnityEngine;

public class MeleeMove : IEnemyMovement
{
    private Enemy _enemy;
    private Player _player;

    private MeleeMoveSO _setting;

    
    public void Init(Enemy enemy, Player player, MovementSettingsSO settingsSo)
    {
        _player = player;
        _enemy = enemy;
        _setting = settingsSo as MeleeMoveSO;
        
        if (_setting == null)
            throw new ArgumentException("PanicMove requires PanicMoveSO");
    }

    public void Tick()
    {
        float step = _setting.Speed * Time.deltaTime;
        _enemy.transform.position = Vector3.MoveTowards(_enemy.transform.position,
            _player.transform.position, step);
    }
}