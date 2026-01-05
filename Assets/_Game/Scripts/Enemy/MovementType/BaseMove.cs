using UnityEngine;

public class BaseMove : IGhostMovement
{
    private Ghost _ghost;

    private Vector2 _startPos;
    private Vector2 _dir;

    private BaseMoveSO _settings;
    private float _segmentDistance;

    public BaseMove(BaseMoveSO settings)
    {
        _settings = settings;
    }
    
    // Начальные свойства типа движения.
    public void Init(Ghost ghost)
    {
        _ghost = ghost;
        
        _startPos = _ghost.transform.position;
        _segmentDistance = Random.Range(_settings.minDistance, _settings.maxDistance);
        _dir = RandomDir();
    }

    // Движение за кадр
    public void Tick()
    {
        float step = _settings.speed * Time.deltaTime;
        _ghost.transform.position += (Vector3)(_dir * step);

        float moved = Vector2.Distance(_ghost.transform.position, _startPos);
        if (moved >= _segmentDistance)
        {
            _dir *= -1;
        }
    }

    // Рандомизация вектора движения
    private Vector2 RandomDir()
    {
        Vector2[] dirs = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };
        return dirs[Random.Range(0, dirs.Length)];
    } 
}