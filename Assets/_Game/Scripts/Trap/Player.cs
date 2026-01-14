using System;
using UnityEngine;

public class Player : MonoBehaviour, ITakingDamage
{
    [Header("Стартовые характеристики щупалец")]
    [SerializeField] private float startGridSpeed = 8f;
    [SerializeField] private float startGridRadius = 1.5f;
    [SerializeField] private int startGridCount = 3;
    [SerializeField] private int startMaxHp = 10;
    [SerializeField] private int hp = 5;
    
    [Header("Ссылки")]
    [SerializeField] private GridNet gridPrefab;
    
    [Header("Поиск врагов")]
    [SerializeField] private LayerMask enemiesMask;
    
    private Vector2 _targetPosition;
    private Vector2 _trapPosition;
    public int _currentGridCount;
    
    [Header("Итоговые переменные после улучшений")]
    private int _gridCount;
    private float _gridSpeed;
    private float _gridRadius;
    
    private int maxHp;
    
    private void Awake()
    {
        _trapPosition = transform.position;

        _currentGridCount = 0;
        _gridCount = startGridCount;
        _gridSpeed = startGridSpeed;
        _gridRadius = startGridRadius;
        maxHp = startMaxHp;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _currentGridCount < _gridCount)
            GetCoordinates();
    }

    private void GetCoordinates() // Получение координат после нажатия ЛКМ
    {
        _targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GridCreate();
    }

    private void GridCreate()
    {
        GridNet grid = Instantiate(gridPrefab);
        _currentGridCount++;
        
        grid.OnLoot += HandleLoot;
        grid.onBack += GridBack;

        grid.Init(_gridSpeed, _gridRadius, enemiesMask, _trapPosition, _targetPosition);
    }
    
    private void HandleLoot(GameObject loot)
    {
        if (loot == null) return;

        if (loot.TryGetComponent<IObjectAttracted>(out var a))
        {
            switch (a.AwardType)
            {
                case TypeAward.Money:
                    MoneySystem.Instance.Add(a.AwardValue);
                    break;
                
                case TypeAward.Hp:
                    Heal(a.AwardValue);
                    break;
                
                case TypeAward.Ability:
                    // abilities.ActivateForSeconds(a.AwardValue);
                    break;
            }
            
            if (loot.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.OnCollected();
            }
            else
            {
                Destroy(loot); // запасной вариант, если это не Enemy
            }

          
        }
        
    }

    private void GridBack(GameObject gridObj)
    {
        _currentGridCount--;

        if (gridObj.TryGetComponent<GridNet>(out var grid))
        {
            grid.OnLoot -= HandleLoot;
            grid.onBack -= GridBack;
        }

        Destroy(gridObj);
    }


    public void Heal(int amount)
    {
        hp += amount;
        if (hp >= maxHp)
        {
            hp = maxHp;
        }
    }
    

    public void ApplyDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            Debug.Log("Death"); // Потом переделать в метод + событие
        }
        
    }
}
