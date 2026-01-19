using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, ITakingDamage
{
    public static Player Instance;
    
    [Header("Стартовые характеристики щупалец")]
    [SerializeField] private float startGridSpeed = 8f;
    [SerializeField] private float startGridRadius = 1.5f;
    [SerializeField] private int startGridCount = 1;
    [SerializeField] private float startTime2Attack = 0.8f;
    [SerializeField] private int startMaxHp = 10;
    
    [Header("Animation")]
    [SerializeField] private Animator animator;
    
    [SerializeField] private LayerMask groundMask;

    
    private int currentHp;
    private float timerAttack;
    private float timerAbility;
    private bool isAbilityActive;
    
    [Header("Ссылки")]
    [SerializeField] private GridNet gridPrefab;
    
    [Header("Поиск врагов")]
    [SerializeField] private LayerMask lootsMask;
    
    
    private Vector2 _targetPosition;
    private Vector2 _trapPosition;
    public int _currentGridCount;
    
    
    [Header("Итоговые переменные после улучшений")]
    private int _gridCount;
    private float _gridSpeed;
    private float _gridRadius;
    private float _gridTime2Attack;
    private int _maxHp;

    public int MaxHp => _maxHp;
    public int CurrentHp => currentHp;
    public bool IsAbilityActive => isAbilityActive;

    public event Action OnApplyDamage;
    public event Action OnApplyHeal;
    
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        currentHp = startMaxHp;
        timerAttack = startTime2Attack;
        
        _trapPosition = transform.position;

        _currentGridCount = 0;
        _gridCount = startGridCount;
        _gridSpeed = startGridSpeed;
        _gridRadius = startGridRadius;
        _gridTime2Attack = startTime2Attack;
        _maxHp = startMaxHp;
    }

    private void Update()
    {
        timerAttack -= Time.deltaTime;
        timerAbility -= Time.deltaTime;

        isAbilityActive = timerAbility > 0;

        if (Input.GetMouseButtonDown(0)
            && _currentGridCount < _gridCount
            && timerAttack <= 0
            && !EventSystem.current.IsPointerOverGameObject())
        {
            GetCoordinates();
        }
    }


    private void GetCoordinates() // Получение координат после нажатия ЛКМ
    {
        timerAttack = _gridTime2Attack;
        _targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GridCreate();
    }

    private void GridCreate()
    {
        GridNet grid = Instantiate(gridPrefab);
        _currentGridCount++;
        
        grid.OnLoot += HandleLoot;
        grid.onBack += GridBack;

        grid.Init(_gridSpeed, _gridRadius, lootsMask, _trapPosition, _targetPosition);
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
                    ActivateAbilities(a.AwardValue);
                    break;
            }
            
            if (loot.TryGetComponent<Enemy>(out var enemy))
            {
                animator.SetTrigger("GetLoot");
                enemy.OnCollected();
            }
            else
            {
                animator.SetTrigger("GetLoot");
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

    public void ActivateAbilities(float time)
    {
        timerAbility = time;
    }

    public void Heal(int amount)
    {
        if (currentHp >= _maxHp) return;
        currentHp += amount;
        OnApplyHeal?.Invoke();
    }

    public void ApplyDamage(int amount)
    {
        VisualEffects.Instance.PlayPlayerHit(transform.position);
        
        if (currentHp <= 0)
        {
            SceneController.Instance.LoadMenu();
            return;
        }
        currentHp -= amount;
        
        
        animator.SetTrigger("Damage");

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
    
    public void UpgradeGridTime2Attack(float delta)
    {
        _gridTime2Attack = Mathf.Max(0f, _gridTime2Attack - delta);
    }

    public void UpgradeMaxHp(int delta)
    {
        _maxHp = Mathf.Max(1, _maxHp + delta);
        currentHp = Mathf.Min(currentHp, _maxHp);
        PlayerHealth.Instance.PlusMaxHeart();
    }

}
