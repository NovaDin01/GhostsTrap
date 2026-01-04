using UnityEngine;

public class BaseMove : IGhostMovement
{
    private Ghost _ghost;

    private Vector2 _startPos;
    private Vector2 _dir;

    private float _segmentDistance;
    
    // Переделать под SO
    private float _minDistance = 1f;
    private float _maxDistance = 3f;

    public void Init(Ghost ghost)
    {
        _ghost = ghost;
        
        _startPos = _ghost.transform.position;
        _segmentDistance = Random.Range(_minDistance, _maxDistance);
        _dir = RandomDir();
    }

    public void Tick()
    {
        float step = _ghost.Speed * Time.deltaTime;
        _ghost.transform.position += (Vector3)(_dir * step);

        float moved = Vector2.Distance(_ghost.transform.position, _startPos);
        if (moved >= _segmentDistance)
        {
            _dir *= -1;
        }
    }

    private Vector2 RandomDir()
    {
        Vector2[] dirs = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };
        return dirs[Random.Range(0, dirs.Length)];
    } 
}