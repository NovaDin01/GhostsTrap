using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    private readonly Queue<Enemy> _pool = new();
    private Enemy _prefab;

    public void Init(Enemy prefab, int count)
    {
        _prefab = prefab;

        for (int i = 0; i < count; i++)
        {
            Enemy g = Instantiate(_prefab, transform);
            g.gameObject.SetActive(false);
            _pool.Enqueue(g);
        }
    }

    public Enemy Get()
    {
        if (_pool.Count == 0)
        {
            Enemy g = Instantiate(_prefab, transform);
            g.gameObject.SetActive(false);
            _pool.Enqueue(g);
        }

        Enemy obj = _pool.Dequeue();
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Return(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
        _pool.Enqueue(enemy);
    }
}