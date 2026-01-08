using System;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IObjectAttracted
{
    [SerializeField] protected EnemyData enemyData;
    
    private int _hp;
    private int _award;
    
    protected bool _isCaught = false;
    
    public event Action<Enemy> OnDespawnRequested;
    
    protected virtual void Awake()
    {
        ResetStatsFromData();
    }

    protected virtual void OnEnable()
    {
        _isCaught = false;
    }

    protected virtual void Update()
    {
        if (_isCaught) return;
        Tick();
    }
    
    public abstract void Move();

    public abstract void Tick();

    public virtual CatchResult TryCatch(Transform catcher)
    {
        if (_isCaught) return CatchResult.AlreadyCaught;
        
        AttachToCatcher(catcher);
        _isCaught = true;
        return CatchResult.Caught;
    }
    
    protected void AttachToCatcher(Transform catcher)
    {
        transform.SetParent(catcher);
        transform.position = catcher.position;
    }
    
    public bool IsCaught() => _isCaught;
    
    private void RequestDespawn()
    {
        OnDespawnRequested?.Invoke(this);
    }
    
    public virtual void ResetForPool()
    {
        _isCaught = false;
        transform.SetParent(null);
        ResetStatsFromData();
    }
    
    protected void ResetStatsFromData()
    {
        _hp = enemyData.hp;
        _award = enemyData.award;
    }
    
}