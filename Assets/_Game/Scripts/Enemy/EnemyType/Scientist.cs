using UnityEngine;

public class Scientist : Enemy
{
    [SerializeField] private PanicMoveSO setting;
    
    private Vector2 _startPos;
    private Vector2 _dir;

    private float _segmentDistance;
    
    public override void Tick()
    {
        Move();
    }
    
    public override void Move()
    {
        float step = setting.speed * Time.deltaTime;
        transform.position += (Vector3)(_dir * step);

        float moved = Vector2.Distance(transform.position, _startPos);
        if (moved >= _segmentDistance)
        {
            StartNewSegment();
        }
    }
    
    
    // Установка направления движения
    private void StartNewSegment()
    {
        _startPos = transform.position;
        _segmentDistance = Random.Range(setting.minDistance, setting.maxDistance);
        _dir = RandomDir();
    }

    // Рандомизация вектора движения
    private Vector2 RandomDir()
    {
        Vector2[] dirs = { Vector2.left, Vector2.right, Vector2.up, Vector2.down };
        return dirs[Random.Range(0, dirs.Length)];
    } 
}