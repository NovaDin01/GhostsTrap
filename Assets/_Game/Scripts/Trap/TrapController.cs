using System;
using UnityEngine;

public class TrapController : MonoBehaviour
{
    [Header("Характеристики ловушки")]
    [SerializeField] private float gridSpeed = 8f;
    [SerializeField] private float gridRadius = 1.5f;

    [Header("Ссылки")]
    [SerializeField] private GameObject gridPrefab;

    [Header("Поиск врагов")]
    [SerializeField] private LayerMask enemiesMask;

    private GameObject gridObject;
    private Vector2 targetPosition;

    private bool isThrow;
    private bool isReturn;
    private bool canClick = true;

    private Collider2D[] enemies = Array.Empty<Collider2D>();

    public event Action<GameObject> OnLoot;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canClick)
            GetCoordinates();
        
        if(isThrow)
            GridThrow();
        
        if(isReturn)
            GridReturn();
    }

    private void GetCoordinates() // Получение координат после нажатия ЛКМ
    {
        targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GridCreate();
        canClick = false;
    }

    private void GridCreate() // Создание сетки и задание координат
    {
        gridObject = Instantiate(gridPrefab);
        gridObject.transform.position = transform.position;
        isThrow = true;
        isReturn = false;
    }

    private void GridThrow() // Перевижение сетки к таргету
    {
        if (gridObject == null) return;

        GridMove(gridObject, targetPosition);
        if (Vector2.Distance(gridObject.transform.position, targetPosition) < 0.1f)
        {
            GetCaught();
            
            isThrow = false;
            isReturn = true;
        }
    }

    private void GridReturn() // Перевижение сетки к ловушке
    {
        if (gridObject == null) return;

        GridMove(gridObject, transform.position);
        if (Vector2.Distance(gridObject.transform.position, transform.position) < 0.1f)
        {
            
            if(enemies.Length > 0)
                GetLoot();
            
            Destroy(gridObject);
            gridObject = null;

            isReturn = false;
            canClick = true;   
        }
    }

    private void GetCaught() // Анализ объектов вокруг сетки и забор всех врагов
    {
        enemies = Physics2D.OverlapCircleAll(gridObject.transform.position, gridRadius, enemiesMask);

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;

            enemy.transform.SetParent(gridObject.transform);

            var ghost = enemy.GetComponent<GhostBehaviour>();
            if (ghost != null)
                ghost.OnCatch(gridObject.transform);
        }
    }


    private void GetLoot() // Вызывает всю логику после забора врагов
    {
        Debug.Log($"[Trap] Loot {enemies.Length} enemies");

        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;

            enemy.transform.SetParent(null);

            OnLoot?.Invoke(enemy.gameObject);
        }

        enemies = Array.Empty<Collider2D>(); // Очищаем список врагов в сетке
    }

    private void GridMove(GameObject grid, Vector2 target) // Метод движения ловушки
    {
        grid.transform.position = Vector2.MoveTowards(grid.transform.position, target, gridSpeed * Time.deltaTime);
    }
}
