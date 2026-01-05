using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : MonoBehaviour
{
    private readonly Queue<Ghost> _pool = new();
    private Ghost _prefab;

    public void Init(Ghost prefab, int count)
    {
        _prefab = prefab;

        for (int i = 0; i < count; i++)
        {
            Ghost g = Instantiate(_prefab, transform);
            g.gameObject.SetActive(false);
            _pool.Enqueue(g);
        }
    }

    public Ghost Get()
    {
        if (_pool.Count == 0)
        {
            Ghost g = Instantiate(_prefab, transform);
            g.gameObject.SetActive(false);
            _pool.Enqueue(g);
        }

        Ghost obj = _pool.Dequeue();
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Return(Ghost ghost)
    {
        ghost.gameObject.SetActive(false);
        _pool.Enqueue(ghost);
    }
}