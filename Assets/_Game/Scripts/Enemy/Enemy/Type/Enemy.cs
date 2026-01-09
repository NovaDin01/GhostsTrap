using System;
using _Game.Scripts.Data;
using UnityEngine;

public class Enemy : MonoBehaviour, IObjectAttracted, ITakingDamage
{
    [SerializeField] protected EnemyData enemyData;
    [SerializeField] protected MovementSettingsSO settingsMovement;
    [SerializeField] protected Player player;
    protected IEnemyMovement _movement;
    
    private int _hp;
    private bool _hasArmor;
    
    protected bool _isCaught = false;
    public bool IsCaught() => _isCaught;
    
    public event Action<Enemy> OnDespawnRequested;

    public virtual void Awake()
    {
        ChooseMovementType();
    }

    protected virtual void OnEnable()
    {
        _hasArmor = enemyData.HasArmor;
        _isCaught = false;
        _hp = enemyData.Hp;
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
                _movement.Init(this, player, settingsMovement);
                break;
            
            case MovementType.LongRangeType:
                _movement = new LongRangeMove();
                _movement.Init(this, player, settingsMovement);
                break;
            
            case MovementType.MeleeType:
                _movement = new MeleeMove();
                _movement.Init(this, player, settingsMovement);
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
    
    private void RequestDespawn()
    {
        OnDespawnRequested?.Invoke(this);
    }
    
    public virtual void ResetForPool()
    {
        _isCaught = false;
        transform.SetParent(null);
    }
}