using System;
using UnityEngine;

public abstract class Upgrade : MonoBehaviour
{
    [SerializeField] private UpgradeData upgradeData;

    private int _cost;
    private int _costIncrease;
    private int _lvl;
    private int _maxLvl;

    private bool _inited;

    public event Action<Upgrade> Changed;

    public int Cost { get { EnsureInit(); return _cost; } }
    public int CostIncrease { get { EnsureInit(); return _costIncrease; } }
    public int Lvl { get { EnsureInit(); return _lvl; } }
    public int MaxLvl { get { EnsureInit(); return _maxLvl; } }

    private void EnsureInit()
    {
        if (_inited) return;
        _inited = true;

        if (upgradeData == null)
        {
            Debug.LogError($"{name}: UpgradeData is NULL", this);
            _cost = 0; _costIncrease = 0; _lvl = 0; _maxLvl = 0;
            return;
        }

        _cost = upgradeData.cost;
        _costIncrease = upgradeData.costIncrease;
        _lvl = upgradeData.lvl;
        _maxLvl = upgradeData.maxLvl;
    }

    public virtual void Get()
    {
        EnsureInit();
        if (!CanBuy()) return;

        MoneySystem.Instance.Buy(_cost);
        ChangeInfo();
    }

    protected void ChangeInfo()
    {
        _cost += _costIncrease;
        _lvl += 1;
        Changed?.Invoke(this);
    }

    public bool CanBuy()
    {
        EnsureInit();
        if (MoneySystem.Instance == null) return false;
        return _cost <= MoneySystem.Instance.Wallet && _lvl < _maxLvl;
    }
}