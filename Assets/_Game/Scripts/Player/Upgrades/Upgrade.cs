using System;
using UnityEngine;

public abstract class Upgrade : MonoBehaviour
{
    [SerializeField] private UpgradeData upgradeData;
    
    private int _cost;
    private int _costIncrease;
    private int _lvl;
    private int _maxLvl;

    public int Cost => _cost;
    public int CostIncrease => _costIncrease;
    public int Lvl => _lvl;
    public int MaxLvl => _maxLvl;

    private void Awake()
    {
        _cost = upgradeData.cost;
        _costIncrease = upgradeData.costIncrease;
        _lvl = upgradeData.lvl;
        _maxLvl = upgradeData.maxLvl;
    }
    
    public virtual void Get()
    {
        if (!CanBuy()) return;

        MoneySystem.Instance.Buy(_cost);
        ChangeInfo();             
    }


    protected void ChangeInfo()
    {
        _cost += _costIncrease;
        _lvl += 1;
    }

    public bool CanBuy()
    {
        return _cost <= MoneySystem.Instance.Wallet && _lvl < _maxLvl;
    }
}
