using _Game.Scripts.Data;
using UnityEngine;

public class BatonAttack : IEnemyAttacker
{
    private Player _player;
    private Enemy _enemy;
    private BatonAttackSO _setting;

    private Vector3 diff;
    private float distSqr;
    private float _attackTimer;
    private float _rangeFireSqr;
    
    public void Init(Player player, Enemy enemy, AttackSettingSO attackSettingSo)
    {
        _enemy = enemy;
        _player = player;
        _setting = attackSettingSo as BatonAttackSO;
        _rangeFireSqr = _setting.RangeFire * _setting.RangeFire;
    }
    

    public void Tick()
    {
        _attackTimer -= Time.deltaTime;

        Vector3 diff = _player.transform.position - _enemy.transform.position;
        distSqr = diff.sqrMagnitude;

        // атакуем только если близко И кулдаун прошёл
        if (distSqr <= _rangeFireSqr && _attackTimer <= 0f)
        {
            _enemy.Feedbacks?.PlayAttack();
            _player.ApplyDamage(_setting.Damage);
            _attackTimer = _setting.SpeedFire;
        }
    }

    public bool IsNearTheObject()
    {
        Vector3 diff = _player.transform.position - _enemy.transform.position;
        return diff.sqrMagnitude <= _rangeFireSqr;
    }

}
