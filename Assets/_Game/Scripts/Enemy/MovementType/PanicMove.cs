using UnityEngine;

public class PanicMove : IEnemyMovement
{
    private Enemy _enemy;

    private Vector2 _startPos;
    private Vector2 _dir;

    private PanicMoveSO _setting;

    private float _segmentDistance;

    public PanicMove(PanicMoveSO setting)
    {
        _setting = setting;
    }

    // Начальные свойства типа движения.
    public void Init(Enemy enemy)
    {
        _enemy = enemy;
        StartNewSegment();
    }

    // Движение за кадр
    public void Tick()
    {
        float step = _setting.speed * Time.deltaTime;
        _enemy.transform.position += (Vector3)(_dir * step);

        float moved = Vector2.Distance(_enemy.transform.position, _startPos);
        if (moved >= _segmentDistance)
        {
            StartNewSegment();
        }
    }

    // Установка направления движения
    private void StartNewSegment()
    {
        _startPos = _enemy.transform.position;
        _segmentDistance = Random.Range(_setting.minDistance, _setting.maxDistance);
        _dir = RandomDir();
    }

    // Рандомизация вектора движения
    private Vector2 RandomDir()
    {
        Vector2[] dirs = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };
        return dirs[Random.Range(0, dirs.Length)];
    } 
}