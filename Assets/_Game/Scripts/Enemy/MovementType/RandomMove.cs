using UnityEngine;

public class RandomMove : IGhostMovement
{
    private Ghost _ghost;

    private Vector2 _startPos;
    private Vector2 _dir;

    private RandomMoveSO _setting;

    private float _segmentDistance;

    public RandomMove(RandomMoveSO setting)
    {
        _setting = setting;
    }

    // Начальные свойства типа движения.
    public void Init(Ghost ghost)
    {
        _ghost = ghost;
        StartNewSegment();
    }

    // Движение за кадр
    public void Tick()
    {
        float step = _setting.speed * Time.deltaTime;
        _ghost.transform.position += (Vector3)(_dir * step);

        float moved = Vector2.Distance(_ghost.transform.position, _startPos);
        if (moved >= _segmentDistance)
        {
            StartNewSegment();
        }
    }

    // Установка направления движения
    private void StartNewSegment()
    {
        _startPos = _ghost.transform.position;
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