using System;
using _Game.Scripts.Data;
using UnityEngine;

public class Enemy : MonoBehaviour, IObjectAttracted, ITakingDamage
{
    [SerializeField] protected EnemyData enemyData;
    [SerializeField] protected MovementSettingsSO settingsMovement;
    [SerializeField] private EnemyFeedbacks feedbacks;

    protected Player _player;
    protected IEnemyMovement _movement;

    private int _hp;
    private bool _hasArmor;
    protected bool _isCaught;
    private bool _isDefeated;

    private bool despawnNotified;

    public TypeAward AwardType => TypeAward.Money;
    public int AwardValue => enemyData.Award;

    public event Action<Enemy> OnCollectedEnemy; // для наград/эффектов
    public event Action<Enemy> OnDespawned;      // для спавнера (лимит живых)
    public EnemyFeedbacks Feedbacks => feedbacks;

    public virtual void Awake()
    {
        _hasArmor = enemyData.HasArmor;
        _hp = enemyData.Hp;
        _isDefeated = false;
        CacheFeedbacks();
    }

    public virtual void OnEnable()
    {
        _isCaught = false;
        despawnNotified = false;

        _hasArmor = enemyData.HasArmor;
        _hp = enemyData.Hp;
        _isDefeated = false;
        CacheFeedbacks();
        feedbacks?.PlaySpawn();
    }

    public virtual void Init(Player player)
    {
        _player = player;
        ChooseMovementType();
    }

    public virtual void Update()
    {
        if (_isCaught) return;
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
            if (!Player.Instance.IsAbilityActive) 
                return CatchResult.Resisted;
            
            AttachToCatcher(catcher);
            _isCaught = true;
            feedbacks?.PlayCaught();
            return CatchResult.Caught;
        }

        AttachToCatcher(catcher);
        _isCaught = true;
        feedbacks?.PlayCaught();
        return CatchResult.Caught;
    }

    private void CacheFeedbacks()
    {
        if (feedbacks == null)
        {
            feedbacks = GetComponentInChildren<EnemyFeedbacks>();
        }

        if (feedbacks != null && enemyData != null && enemyData.FeedbackConfig != null)
        {
            feedbacks.ApplyConfig(enemyData.FeedbackConfig);
        }
    }

    protected void AttachToCatcher(Transform catcher)
    {
        transform.SetParent(catcher);
        transform.position = catcher.position;
    }

    protected void BreakArmor()
    {
        _hasArmor = false;
        feedbacks?.PlayArmorBreak();
    }

    public void ApplyDamage(int amount)
    {
        _hp -= amount;
        feedbacks?.PlayHit();

        if (_hp <= 0 && !_isDefeated)
        {
            _isDefeated = true;
            feedbacks?.PlayDeath();
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
