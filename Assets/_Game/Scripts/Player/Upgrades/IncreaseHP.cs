// IncreaseHP.cs (unchanged in logic)
using UnityEngine;

public class IncreaseHP : Upgrade
{
    [SerializeField] private int amount;

    public override void Get()
    {
        if (!CanBuy()) return;
        base.Get();
        Player.Instance.UpgradeMaxHp(amount);
    }
}