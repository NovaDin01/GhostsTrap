// Player.cs
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, ITakingDamage
{
    public static Player Instance;

    [Header("Start stats")]
    [SerializeField] private float startGridSpeed = 8f;
    [SerializeField] private float startGridRadius = 1.5f;
    [SerializeField] private int startGridCount = 1;
    [SerializeField] private float startTime2Attack = 0.8f;
    [SerializeField] private int startMaxHp = 10;
    [SerializeField] private float damageInvulnerabilityDuration = 0.4f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Refs")]
    [SerializeField] private GridNet gridPrefab;

    [Header("Loot/Enemies mask")]
    [SerializeField] private LayerMask lootsMask;

    private int currentHp;
    private int _maxHp;

    private float timerAttack;
    private float timerAbility;
    private float damageInvulnerabilityTimer;
    private bool _isDead;

    private int _gridCount;
    private float _gridSpeed;
    private float _gridRadius;
    private float _gridTime2Attack;

    private Vector2 _targetPosition;
    private Vector2 _trapPosition;

    public int _currentGridCount;

    public int MaxHp => _maxHp;
    public int CurrentHp => currentHp;
    public bool IsAbilityActive => timerAbility > 0f;
    public bool IsDead => _isDead;

    // UI should listen to this only (model -> view)
    public event Action<int, int> OnHpChanged; // (current, max)

    public event Action OnDied;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _trapPosition = transform.position;

        _gridCount = startGridCount;
        _gridSpeed = startGridSpeed;
        _gridRadius = startGridRadius;
        _gridTime2Attack = startTime2Attack;

        _maxHp = Mathf.Max(1, startMaxHp);
        currentHp = _maxHp;

        timerAttack = startTime2Attack;
        _currentGridCount = 0;

        timerAbility = 0f;
        damageInvulnerabilityTimer = 0f;

        _isDead = false;

        RaiseHpChanged();
    }

    private void Update()
    {
        timerAttack -= Time.deltaTime;
        timerAbility -= Time.deltaTime;
        damageInvulnerabilityTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0)
            && _currentGridCount < _gridCount
            && timerAttack <= 0f
            && !EventSystem.current.IsPointerOverGameObject())
        {
            GetCoordinates();
        }
    }

    private void GetCoordinates()
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

        if (animator != null) animator.SetTrigger("Throw");
        if (VisualEffects.Instance != null) VisualEffects.Instance.PlayThrowSfx(transform.position);
    }

    private void HandleCaught(GameObject caughtObj)
    {
        if (caughtObj == null) return;

        var enemyAnim = caughtObj.GetComponentInChildren<Animator>();
        if (enemyAnim != null) enemyAnim.SetTrigger("Caught");
    }

    private void HandleLoot(GameObject loot)
    {
        if (loot == null) return;

        if (VisualEffects.Instance != null) VisualEffects.Instance.PlayCatchSfx(transform.position);
        if (animator != null) animator.SetTrigger("Catch");

        if (!loot.TryGetComponent<IObjectAttracted>(out var a)) return;

        switch (a.AwardType)
        {
            case TypeAward.Money:
                if (MoneySystem.Instance != null) MoneySystem.Instance.Add(a.AwardValue);
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
            if (animator != null) animator.SetTrigger("GetLoot");
            enemy.OnCollected();
        }
        else
        {
            if (animator != null) animator.SetTrigger("GetLoot");
            Destroy(loot);
        }
    }

    private void GridBack(GameObject gridObj)
    {
        _currentGridCount--;

        if (gridObj != null && gridObj.TryGetComponent<GridNet>(out var grid))
        {
            grid.OnLoot -= HandleLoot;
            grid.OnBack -= GridBack;
            grid.OnCaught -= HandleCaught;
        }

        if (gridObj != null) Destroy(gridObj);
    }

    public void ActivateAbilities(float time)
    {
        timerAbility = Mathf.Max(0f, time);
    }

    public void Heal(int amount)
    {
        if (_isDead) return;
        if (amount <= 0) return;
        if (currentHp >= _maxHp) return;

        int canHeal = _maxHp - currentHp;
        int healed = Mathf.Min(amount, canHeal);

        currentHp += healed;
        RaiseHpChanged();
    }

    public void ApplyDamage(int amount)
    {
        if (_isDead) return;
        if (amount <= 0) return;
        if (damageInvulnerabilityTimer > 0f) return;

        if (currentHp <= 0)
        {
            Die();
            return;
        }

        if (VisualEffects.Instance != null) VisualEffects.Instance.PlayPlayerHit(transform.position);

        currentHp = Mathf.Max(0, currentHp - amount);
        damageInvulnerabilityTimer = damageInvulnerabilityDuration;

        RaiseHpChanged();

        if (currentHp <= 0)
        {
            Die();
            return;
        }

        if (animator != null) animator.SetTrigger("Damage");
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;
        OnDied?.Invoke();
    }


    public void Revive(int hpAfterRevive)
    {
        if (!_isDead) return;

        _isDead = false;
        damageInvulnerabilityTimer = damageInvulnerabilityDuration;

        int targetHp = Mathf.Max(1, hpAfterRevive);
        currentHp = Mathf.Min(targetHp, _maxHp);

        RaiseHpChanged();
    }

    private void RaiseHpChanged()
    {
        OnHpChanged?.Invoke(currentHp, _maxHp);
    }

    // Upgrades (your existing “quick hacks”)
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
        if (_isDead) return;

        _maxHp = Mathf.Max(1, _maxHp + delta);

        // Optional: also heal by 1 when max hp increases
        currentHp = Mathf.Min(currentHp, _maxHp);
        Heal(1);

        // Heal() calls RaiseHpChanged()
    }
}
