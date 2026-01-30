using _Game.Scripts.Data;
using UnityEngine;

public class LongRangeAttack : IEnemyAttacker
{
    private Player _player;
    private Enemy _enemy;
    private LongRangeAttackSO _setting;
    
    private Vector3 diff;
    private float distSqr;
    private float _attackTimer;
    private float _rangeFireSqr;
    
    private int _bulletsLeft;
    private float _shotDelayTimer;

    
    public void Init(Player player, Enemy enemy, AttackSettingSO attackSettingSo)
    {
        _enemy = enemy;
        _player = player;
        _setting = attackSettingSo as LongRangeAttackSO;
        _rangeFireSqr = _setting.RangeFire * _setting.RangeFire;
        
        _attackTimer = 0f;
        _bulletsLeft = 0;
        _shotDelayTimer = 0f;
        
    }
    
    public void Tick()
    {
        _attackTimer -= Time.deltaTime;

        if (_bulletsLeft > 0)
        {
            _shotDelayTimer -= Time.deltaTime;

            if (_shotDelayTimer <= 0f)
            {
                FireOneBullet();
                _bulletsLeft--;
                _shotDelayTimer = _setting.BulletDelay;
            }
        }

        Vector3 diff = _player.transform.position - _enemy.transform.position;
        distSqr = diff.sqrMagnitude;

        if (distSqr <= _rangeFireSqr && _attackTimer <= 0f)
        {
            Shot();
        }
    }
    
    public void Shot()
    {
        _bulletsLeft = _setting.BulletCount;
        _shotDelayTimer = 0f; 
        _attackTimer = _setting.SpeedFire;
    }
    
    private void FireOneBullet()
    {
        _enemy.Feedbacks?.PlayAttack();
        var bullet = UnityEngine.Object.Instantiate(_setting.Bullet);
        bullet.Init(_player, _enemy, _setting);
    }

    
    public bool IsNearTheObject()
    {
        Vector3 diff = _player.transform.position - _enemy.transform.position;
        return diff.sqrMagnitude <= _rangeFireSqr;
    }
}
