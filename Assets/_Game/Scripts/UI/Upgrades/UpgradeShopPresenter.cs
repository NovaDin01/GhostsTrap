using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UpgradeShopPresenter : MonoBehaviour
{
    [SerializeField] private UpgradeShopConfig config;

    private readonly List<UpgradeDefinition> _definitions = new List<UpgradeDefinition>();

    public event Action<IReadOnlyList<UpgradeShopItemData>> Changed;

    private void OnEnable()
    {
        Initialize();
        Subscribe();
        NotifyChanged();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    public void Buy(string upgradeId)
    {
        var definition = _definitions.FirstOrDefault(item => item != null && item.Id == upgradeId);
        if (definition == null || definition.Upgrade == null)
        {
            return;
        }

        definition.Upgrade.Get();
        NotifyChanged();
    }

    public void Refresh()
    {
        NotifyChanged();
    }

    private void Initialize()
    {
        _definitions.Clear();
        if (config == null)
        {
            return;
        }

        foreach (var definition in config.Upgrades)
        {
            if (definition != null)
            {
                _definitions.Add(definition);
            }
        }
    }

    private void Subscribe()
    {
        foreach (var definition in _definitions)
        {
            if (definition != null && definition.Upgrade != null)
            {
                definition.Upgrade.Changed += HandleUpgradeChanged;
            }
        }

        if (MoneySystem.Instance != null)
        {
            MoneySystem.Instance.OnMoneyChanged += HandleMoneyChanged;
        }
    }

    private void Unsubscribe()
    {
        foreach (var definition in _definitions)
        {
            if (definition != null && definition.Upgrade != null)
            {
                definition.Upgrade.Changed -= HandleUpgradeChanged;
            }
        }

        if (MoneySystem.Instance != null)
        {
            MoneySystem.Instance.OnMoneyChanged -= HandleMoneyChanged;
        }
    }

    private void HandleUpgradeChanged(Upgrade upgrade)
    {
        NotifyChanged();
    }

    private void HandleMoneyChanged()
    {
        NotifyChanged();
    }

    private void NotifyChanged()
    {
        var items = new List<UpgradeShopItemData>(_definitions.Count);
        foreach (var definition in _definitions)
        {
            if (definition == null || definition.Upgrade == null)
            {
                continue;
            }

            var upgrade = definition.Upgrade;
            items.Add(new UpgradeShopItemData(
                definition.Id,
                definition.DisplayName,
                definition.Description,
                definition.Icon,
                upgrade.Cost,
                upgrade.CostIncrease,
                upgrade.Lvl,
                upgrade.MaxLvl,
                upgrade.CanBuy()));
        }

        Changed?.Invoke(items);
    }
}
