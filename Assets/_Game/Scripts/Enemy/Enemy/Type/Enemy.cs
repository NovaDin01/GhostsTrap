using System;
using _Game.Scripts.Data;
using UnityEngine;



public class Enemy : MonoBehaviour, IObjectAttracted, ITakingDamage
{
    [SerializeField] protected EnemyData enemyData;
    [SerializeField] protected MovementSettingsSO settingsMovement;
    [SerializeField] private ParticleSystem armorParticleSystem;
    [SerializeField] private GameObject armorEffect;

    protected Player _player;
    protected IEnemyMovement _movement;

    private int _hp;
    private bool _hasArmor;
    protected bool _isCaught;
    private bool _isDead;

    private bool despawnNotified;
    
    public event Action OnAttack;
    public void RaiseAttack() => OnAttack?.Invoke();


    public TypeAward AwardType => TypeAward.Money;
    public int AwardValue => enemyData.Award;

    public event Action<Enemy> OnCollectedEnemy; // для наград/эффектов
    public event Action<Enemy> OnDespawned;      // для спавнера (лимит живых)
    public event Action OnArmorBroken;
    public event Action OnCaught;
    public event Action<int> OnDamaged;
    public event Action OnDeath;

    public static bool IsFrozen { get; private set; }

    public static void SetFrozen(bool frozen)
    {
        IsFrozen = frozen;
    }

    public virtual void Awake()
    {
        _hasArmor = enemyData.HasArmor;
        _hp = enemyData.Hp;
    }

    public virtual void OnEnable()
    {
        _isCaught = false;
        despawnNotified = false;
        _isDead = false;
        armorEffect.SetActive(true);

        _hasArmor = enemyData.HasArmor;
        _hp = enemyData.Hp;
    }

    public virtual void Init(Player player)
    {
        _player = player;
        ChooseMovementType();
    }

    public virtual void Update()
    {
        if (_isCaught || IsFrozen) return;
        _movement?.Tick();
    }

    protected void ChooseMovementType()
    {
        switch (enemyData.MoveType)
        {
            case MovementType.PanicType:
                _movement = new PanicMove();
                break;

            case MovementType.LongRangeType:
                _movement = new LongRangeMove();
                break;

            case MovementType.MeleeType:
                _movement = new MeleeMove();
                break;
        }

        _movement?.Init(this, _player, settingsMovement);
    }

    public virtual CatchResult TryCatch(Transform catcher)
    {
        if (_isCaught) return CatchResult.AlreadyCaught;
        if (_hp > 0) return CatchResult.Resisted;

        if (_hasArmor)
        {
            BreakArmor();
            OnArmorBroken?.Invoke();
            if (!Player.Instance.IsAbilityActive) 
                return CatchResult.Resisted;
            
            AttachToCatcher(catcher);
            _isCaught = true;
            OnCaught?.Invoke();
            return CatchResult.Caught;
        }

        AttachToCatcher(catcher);
        _isCaught = true;
        OnCaught?.Invoke();
        return CatchResult.Caught;
    }

    protected void AttachToCatcher(Transform catcher)
    {
        transform.SetParent(catcher);
        transform.position = catcher.position;
    }

    protected void BreakArmor()
    {
        _hasArmor = false;
        //if(armorParticleSystem != null) armorParticleSystem.Stop();
        
        if(armorEffect != null) armorEffect.SetActive(false);
    }

    public void ApplyDamage(int amount)
    {
        if (_isDead) return;

        _hp -= amount;
        OnDamaged?.Invoke(amount);
        if (_hp <= 0 && !_isDead)
        {
            _isDead = true;
            OnDeath?.Invoke();
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("SideWall") && _movement is PanicMove)
        {
            Destroy(gameObject);
        }
    }

    public void OnCollected()
    {
        if (despawnNotified) return;

        OnCollectedEnemy?.Invoke(this);
        VisualEffects.Instance.PlayPeopleEat(transform.position);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        NotifyDespawnOnce();
    }

    private void OnDisable()
    {
        NotifyDespawnOnce();
    }

    private void NotifyDespawnOnce()
    {
        if (despawnNotified) return;
        despawnNotified = true;
        OnDespawned?.Invoke(this);
    }
}
