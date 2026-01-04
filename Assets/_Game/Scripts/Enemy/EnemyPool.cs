using System;
using System.Collections.Generic;
using UnityEngine;
// Пул для врагов. Враги не создаются и удаляются все время, а при начале создаются и повторно используются.
// Он работает пока для одного префаба, после расширю
public class EnemyPool : MonoBehaviour
{
    private Queue<GameObject> _objectsPool = new();
    private GameObject _enemyPrefab;
    private int _startCount;

    [SerializeField] private TrapController _trapController;

    // Происходит заполнение пула и создаются враги.
    public void Init(GameObject prefab, int count)
    {
        _enemyPrefab = prefab;
        _startCount = count;

        for (int i = 0; i < _startCount; i++) 
        {
            var obj = Instantiate(_enemyPrefab); 
            obj.SetActive(false); // Отключаем пока не понадобятся
            _objectsPool.Enqueue(obj); // Добавялем в спискок пула
        }
    }

    // Метод взятия объекта. Вместо спавна - мы просто его включаем.
    public GameObject GetObject()
    {
        if (_objectsPool.Count == 0) // Если в списке больше не осталось врагов - создаем еще и добавляем в список.
        {
            var obj = Instantiate(_enemyPrefab);
            obj.SetActive(false);
            _objectsPool.Enqueue(obj);
        }

        var objPool = _objectsPool.Dequeue(); // Достаем объект из списка не активных объектов и удаляем его из списка
        objPool.SetActive(true); 
        return objPool;
    }

    public void ReturnObject(GameObject returnObject) // Возвращаем обратно (после поимки или исчезновения)
    {
        
        returnObject.SetActive(false);
        _objectsPool.Enqueue(returnObject);
    }

    
    // OnEnable и OnDisable необходимы для событий
    // private void OnEnable()
    // {
    //     _trapController.OnLoot += ReturnObject;
    // }
    //
    // private void OnDisable()
    // {
    //     _trapController.OnLoot -= ReturnObject;
    // }
}