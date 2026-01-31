using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, ITakingDamage
{
    public static Player Instance;
    
    [Header("Ñòàðòîâûå õàðàêòåðèñòèêè ùóïàëåö")]
    [SerializeField] private float startGridSpeed = 8f;
    [SerializeField] private float startGridRadius = 1.5f;
    [SerializeField] private int startGridCount = 1;
    [SerializeField] private float startTime2Attack = 0.8f;
    [SerializeField] private int startMaxHp = 10;
    [SerializeField] private float damageInvulnerabilityDuration = 0.4f;
    
 
    
    [Header("Animation")]
    [SerializeField] private Animator animator;
    
    [SerializeField] private LayerMask groundMask;

    
    private int currentHp;
    private float timerAttack;
    private float timerAbility;
    private bool isAbilityActive;
    private float damageInvulnerabilityTimer;
    private bool _isDead;

    
    [Header("Ññûëêè")]
    [SerializeField] private GridNet gridPrefab;
    
    [Header("Ïîèñê âðàãîâ")]
    [SerializeField] private LayerMask lootsMask;
    
    
    private Vector2 _targetPosition;
    private Vector2 _trapPosition;
    public int _currentGridCount;
    
    
    [Header("Èòîãîâûå ïåðåìåííûå ïîñëå óëó÷øåíèé")]
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
    public event Action OnDied;
    
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
        _isDead = false;
    }

    private void Update()
    {
        timerAttack -= Time.deltaTime;
        timerAbility -= Time.deltaTime;
        damageInvulnerabilityTimer -= Time.deltaTime;


        isAbilityActive = timerAbility > 0;

        if (Input.GetMouseButtonDown(0)
            && _currentGridCount < _gridCount
            && timerAttack <= 0
            && !EventSystem.current.IsPointerOverGameObject())
        {
            GetCoordinates();
        }
    }


    private void GetCoordinates() // Ïîëó÷åíèå êîîðäèíàò ïîñëå íàæàòèÿ ËÊÌ
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
        grid.OnBack += GridBack;

        grid.OnCaught += HandleCaught;

        grid.Init(_gridSpeed, _gridRadius, lootsMask, _trapPosition, _targetPosition);

        animator.SetTrigger("Throw");
        VisualEffects.Instance.PlayThrowSfx(transform.position);
    }
    
    private void HandleCaught(GameObject caughtObj)
    {
        if (caughtObj == null) return;


        // Или напрямую Animator врага:
        var enemyAnim = caughtObj.GetComponentInChildren<Animator>();
        if (enemyAnim != null)
            enemyAnim.SetTrigger("Caught"); // trigger Caught у врага
    }


    
    private void HandleLoot(GameObject loot)
    {
        if (loot == null) return;
        
        VisualEffects.Instance.PlayCatchSfx(transform.position);
        animator.SetTrigger("Catch");

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
                Destroy(loot); 
            }

          
        }
        
    }

    private void GridBack(GameObject gridObj)
    {
        _currentGridCount--;

        if (gridObj.TryGetComponent<GridNet>(out var grid))
        {
            grid.OnLoot -= HandleLoot;
            grid.OnBack -= GridBack;

            grid.OnCaught -= HandleCaught;
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
        if (_isDead)
        {
            return;
        }

        if (currentHp <= 0)
        {
            Die();
            return;
        }

        if (damageInvulnerabilityTimer > 0f) return;
        if (amount <= 0) return;

        VisualEffects.Instance.PlayPlayerHit(transform.position);

        currentHp = Mathf.Max(0, currentHp - amount);
        damageInvulnerabilityTimer = damageInvulnerabilityDuration;
        

        OnApplyDamage?.Invoke();

        if (currentHp <= 0)
        {
            Die();
            return;
        }

        animator.SetTrigger("Damage");
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;
        OnDied?.Invoke();
    }


    
    // Êîñòûëü èç-çà íåõâàòêè âðåìåíè
    
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
