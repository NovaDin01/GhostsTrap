using System;
using UnityEngine;

public class TrapController : MonoBehaviour
{
    [Header("Характеристики ловушки")]
    [SerializeField] private float gridSpeed = 8f;
    [SerializeField] private float gridRadius = 1.5f;

    [Header("Ссылки")]
    [SerializeField] private GridNet gridPrefab;

    [Header("Поиск врагов")]
    [SerializeField] private LayerMask enemiesMask;
    
    [Header("СИСТЕМНЫЕ НАСТРОЙКИ")]
    [SerializeField] private EnemyPool enemyPool;
    
    private Vector2 targetPosition;
    private Vector2 trapPosition;
    
    private bool canClick = true;

    private void Awake()
    {
        trapPosition = transform.position;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            GetCoordinates();
    }

    private void GetCoordinates() // Получение координат после нажатия ЛКМ
    {
        targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GridCreate();
        canClick = false;
    }

    private void GridCreate()
    {
        GridNet grid = Instantiate(gridPrefab);
        
        grid.OnLoot += HandleLoot;

        grid.Init(gridSpeed, gridRadius, enemiesMask, trapPosition, targetPosition);
    }
    
    private void HandleLoot(GameObject enemyGo)
    {
        if (enemyGo == null) return;

        if (enemyGo.TryGetComponent<Ghost>(out var ghost))
        {
            ghost.ResetForPool(); 
            enemyPool.Return(ghost);
        }
        else
        {
            enemyGo.SetActive(false);
        }
    }



}
