using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Player _player;
    private Enemy _enemy;
    private LongRangeAttackSO _setting;
    
    public void Init(Player player, Enemy enemy, LongRangeAttackSO setting)
    {
        _enemy = enemy;
        _player = player;
        _setting = setting;

        transform.position = _enemy.transform.position;
    }

    private void Update()
    {
        float step = _setting.BulletSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, step);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent<Player>(out var player))
        {
            player.ApplyDamage(_setting.Damage);
            VisualEffects.Instance.PlayBulletHit(transform.position);
            Destroy(gameObject);
        }

        if (other.gameObject.TryGetComponent<Grid>(out var grid))
        {
            VisualEffects.Instance.PlayBulletHit(transform.position);
            Destroy(gameObject);
        }
    }
}