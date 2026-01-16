using System;
using UnityEngine;

public class Player : MonoBehaviour, ITakingDamage
{
    public static Player Instance;
    
    [Header("Стартовые характеристики щупалец")]
    [SerializeField] private float startGridSpeed = 8f;
    [SerializeField] private float startGridRadius = 1.5f;
    [SerializeField] private int startGridCount = 1;
    [SerializeField] private int startMaxHp = 3;
    private int currentHp = 3;
    
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
    private int _maxHp;

    public int MaxHp => _maxHp;
    public int CurrentHp => currentHp;

    public event Action OnApplyDamage;
    public event Action OnApplyHeal;
    
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
        
        _trapPosition = transform.position;

        _currentGridCount = 0;
        _gridCount = startGridCount;
        _gridSpeed = startGridSpeed;
        _gridRadius = startGridRadius;
        _maxHp = startMaxHp;
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
        if (currentHp >= _maxHp) return;
        currentHp += amount;
        OnApplyHeal?.Invoke();
    }
    

    public void ApplyDamage(int amount)
    {
        if (currentHp <= 0)
        {
            Debug.Log("Death"); // Потом переделать в метод + событие
            return;
        }
        
        Debug.Log("DAMAGE EVENT");
        currentHp -= amount;
        OnApplyDamage?.Invoke();
    }

    
    // Костыль из-за нехватки времени
    
    public void UpgradeGridCount()
    {
        _gridCount = Mathf.Max(0, _gridCount + 1);
    }

    public void UpgradeGridSpeed(float delta)
    {
        _gridSpeed = Mathf.Max(0f, _gridSpeed + delta);
    }

    public void UpgradeGridRadius(float delta)
    {
        _gridRadius = Mathf.Max(0f, _gridRadius + delta);
    }

    public void UpgradeMaxHp(int delta)
    {
        _maxHp = Mathf.Max(1, _maxHp + delta);
        currentHp = Mathf.Min(currentHp, _maxHp);
        PlayerHealth.Instance.PlusMaxHeart();
    }

}
