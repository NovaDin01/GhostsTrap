using System;
using _Game.Scripts.Data;
using UnityEngine;

public class Enemy : MonoBehaviour, IObjectAttracted, ITakingDamage
{
    [SerializeField] protected EnemyData enemyData;
    [SerializeField] protected MovementSettingsSO settingsMovement;
    protected Player _player;
    protected IEnemyMovement _movement;
    
    private int _hp;
    private bool _hasArmor;
    protected bool _isCaught = false;
    
    public TypeAward AwardType => TypeAward.Money;
    public int AwardValue => enemyData.Award;
    
    public event Action<Enemy> OnCollectedEnemy;

    public virtual void Awake()
    {
        _hasArmor = enemyData.HasArmor;
        _isCaught = false;
        _hp = enemyData.Hp;
    }

    public virtual void Init(Player player)
    {
        _player = player;
        ChooseMovementType();
    }

    public virtual void Update()
    {
        if (_isCaught) return;
        _movement.Tick();
    }

    protected void ChooseMovementType()
    {
        switch (enemyData.MoveType)
        {
            case MovementType.PanicType:
                _movement = new PanicMove();
                _movement.Init(this, _player, settingsMovement);
                break;
            
            case MovementType.LongRangeType:
                _movement = new LongRangeMove();
                _movement.Init(this, _player, settingsMovement);
                break;
            
            case MovementType.MeleeType:
                _movement = new MeleeMove();
                _movement.Init(this, _player, settingsMovement);
                break;
        }
    }

    public virtual CatchResult TryCatch(Transform catcher)
    {
        if (_isCaught) return CatchResult.AlreadyCaught;
        if (_hp > 0) return CatchResult.Resisted;
        
        if (_hasArmor)
         {
             BreakArmor();
             return CatchResult.Resisted;
        }
        
        AttachToCatcher(catcher);
        _isCaught = true;
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
        Debug.Log("Броня уничтожена");
    }

    public void ApplyDamage(int amount)
    {
        _hp -= amount;
        // Событие для визуала
    }

    public void OnCollected()
    {
        //TODO: Событие для визуала
        OnCollectedEnemy?.Invoke(this);
        Destroy(gameObject);
    }
    
}